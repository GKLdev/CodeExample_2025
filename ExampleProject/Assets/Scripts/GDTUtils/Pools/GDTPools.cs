using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDTUtils.Extensions;
using GDTUtils.Patterns.Factory;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace GDTUtils
{
    /// <summary>
    /// Default object pool functionality
    /// Every pool entry can be Awaken/Slept
    /// All entries created on init are sleeping by default
    /// </summary>
    public interface IObjectPool : IFactoryProduct, IDisposable
    {
        void Init(int _defaultEntriesCount = 0);

        void SetLimitType(PoolLimitType _limitType);
        
        /// <remarks>
        /// O(1)
        /// </remarks>
        IPoolable AwakeEntry();
        
        /// <remarks>
        /// O(n)
        /// </remarks>
        List<IPoolable> AwakeEntries(int _count);
        
        /// <remarks>
        /// O(1)
        /// </remarks>
        void SleepEntry(IPoolable _entry);
        
        /// <remarks>
        /// O(n)
        /// </remarks>
        void SleepEntries(List<IPoolable> _entries);

        /// <summary>
        /// O(1)
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns></returns>
        bool IsEntryActive(IPoolable _entry);

        event Action<IPoolable> EventEntryCreated;
        event Action<IPoolable> EventEntryAwaken;
        event Action<IPoolable> EventEntrySlept;
    }
    
    public interface IPoolable : IFactoryProduct, IDisposable
    {
        int Id { get; set; }
        void Activate();
        void Deactivate();
        void OnAdded();
    }

    /// <summary>
    /// Use this factory to create GDT pools
    /// </summary>
    /// <typeparam name="TPoolEntryFactory"></typeparam>
    public class ObjectPoolFactory<TPoolEntryFactory> : IConcreteFactory<IObjectPool>
        where TPoolEntryFactory : IConcreteFactory<IPoolable>
    {
        private readonly TPoolEntryFactory entryFactory;
            
        public ObjectPoolFactory(TPoolEntryFactory _entryFactory)
        {
            entryFactory = _entryFactory;
        }
            
        public IObjectPool Produce()
        {
            return new GDTPools.ObjectPool(entryFactory);
        }

        IFactoryProduct IFactory.Produce() 
        {
            return Produce();
        }
    }
    
    /// <summary>
    /// <param name="None">No limits, pool can be expanded beyond prewarm elements count</param>
    /// /// <param name="Cycle">Pool can't be expanded. New elements will be spawned instead if ones at the beginning of the pool. Those objects will be auto destroyed first</param>
    /// </summary>
    public enum PoolLimitType
    {
        None,
        Cycle
    }
    
    public static class GDTPools
    {
        public class ObjectPool : IObjectPool
        {
            private Dictionary<int, IPoolable> idToEntry = new();

            private Dictionary<int, bool> idToState;
            private HashSet<int>          sleepingEntries;
            private Stack<int>            sleepingEntriesStack;
            
            private Dictionary<int, int>  idToAwakeCycle;
            private Dictionary<int, int>  awakeCycleToId;

            private int                                  lastAddedIndex = -1;
            private readonly IConcreteFactory<IPoolable> factory;

            private PoolLimitType limitType = PoolLimitType.None;

            public event Action<IPoolable> EventEntryAwaken;
            public event Action<IPoolable> EventEntryCreated;
            public event Action<IPoolable> EventEntrySlept;

            public ObjectPool(IConcreteFactory<IPoolable> _factory)
            {
                factory = _factory;
            }
            
            public class EntryWrap
            {
                public IPoolable entry;
            }
            
            private int maxAwakeCycle = -1;
            private int minCycle      = 0;
            
            //*****************************
            // Init
            //*****************************
            public void Init(int _defaultEntriesCount = 0) // TODO add limit above prewarmed mb?
            {
                idToState            = new(_defaultEntriesCount);
                idToAwakeCycle       = new(_defaultEntriesCount);
                awakeCycleToId       = new(_defaultEntriesCount);
                sleepingEntries      = new(_defaultEntriesCount);
                sleepingEntriesStack = new(_defaultEntriesCount);
                sleepingEntries      = new HashSet<int>(_defaultEntriesCount);
                
                bool addDefaultEntries = _defaultEntriesCount > 0;
                if (addDefaultEntries)
                {
                    for (int i = 0; i < _defaultEntriesCount; i++)
                    {
                        AddNewEntry(false);
                    }
                }
            }
            
            //*****************************
            // SetLimitType
            //*****************************
            public void SetLimitType(PoolLimitType _limitType)
            {
                limitType = _limitType;
            }

            //*****************************
            // AwakeEntry
            //*****************************
            public IPoolable AwakeEntry()
            {
                maxAwakeCycle++;
                
                bool sleepingEntryExists = sleepingEntries.Count > 0;
                
                // awake sleeping entry
                if (sleepingEntryExists)
                {
                    int id = AwakePeekEntry();
                    HandleCycleOnAwake(id);
                    
                    return idToEntry[id];
                }
                else
                {
                    // if no sleeping entries left and its cycle mode
                    if (limitType == PoolLimitType.Cycle)
                    {
                        int oldestElementId = awakeCycleToId[minCycle];

                        // make callbacks
                        MakeSleepCallbacks(oldestElementId);
                        MakeAwakeCallbacks(oldestElementId);

                        HandleCycleOnSleep(oldestElementId);
                        HandleCycleOnAwake(oldestElementId);

                        return idToEntry[oldestElementId];
                    }
                }
                
                // create new item
                return idToEntry[AddNewEntry(true)];
            }

            //*****************************
            // AwakeEntries
            //*****************************
            public List<IPoolable> AwakeEntries(int _count)
            {
                List<IPoolable> result = new();

                for (int i = 0; i < _count; i++)
                {
                    result.Add(AwakeEntry());
                }
                
                return result;
            }

            //*****************************
            // SleepEntry
            //*****************************
            public void SleepEntry(IPoolable _entry)
            {
                bool found = idToEntry.TryGetValue(_entry.Id, out _);
                if (!found)
                {
                    throw new System.Exception($"ObjectPool: Pool does not contain object {_entry.ToString()} id={_entry.Id}");
                }

                if (sleepingEntries.Contains(_entry.Id))
                {
                    Debug.LogWarning($"ObjectPool: Cant sleep entry which is already slept! entry={_entry.ToString()} id={_entry.Id}");
                    return;
                }

                HandleCycleOnSleep(_entry.Id);
                SleepEntry(_entry.Id);
            }

            //*****************************
            // IsEntryActive
            //*****************************
            public bool IsEntryActive(IPoolable _entry)
            {
                bool found = idToEntry.TryGetValue(_entry.Id, out _);
                if (!found)
                {
                    throw new System.Exception($"ObjectPool: Pool does not contain object {_entry.ToString()} id={_entry.Id}");
                }

                bool result = !sleepingEntries.Contains(_entry.Id);
                return result;
            }

            //*****************************
            // SleepEntries
            //*****************************
            public void SleepEntries(List<IPoolable> _entries)
            {
                _entries.ForEach(SleepEntry);
            }

            //*****************************
            // HandleCycleOnSleep
            //*****************************
            private void HandleCycleOnSleep(int _elementId)
            {
                // remove cycle info on sleep if present
                bool cycleFound = idToAwakeCycle.TryGetValue(_elementId, out int cycleId);
                if (cycleFound)
                {
                    idToAwakeCycle.Remove(_elementId);
                    awakeCycleToId.Remove(cycleId);
                    
                    // advance min cycle
                    // only runs of there is any awaken entry
                    bool awakenEntriesPresent   = idToAwakeCycle.Count > 0;
                    bool hiddenElementWasOldest = awakenEntriesPresent && cycleId == minCycle;
                    if (hiddenElementWasOldest)
                    {
                        bool searching = true;
                        while (searching)
                        {
                            minCycle++;
                            
                            bool error = minCycle > maxAwakeCycle;
                            if (error)
                            {
                                throw new System.Exception($"Min cycles exceeds max! Probably there is no awaken entries pas cycle={minCycle}");
                            }
                            
                            searching = !awakeCycleToId.ContainsKey(minCycle);
                        }
                    }
                }
            }

            //*****************************
            // HandleCycleOnAwake
            //*****************************
            private void HandleCycleOnAwake(int _elementId)
            {
                idToAwakeCycle.Add(_elementId, maxAwakeCycle);
                awakeCycleToId.Add(maxAwakeCycle, _elementId);
            }
            
            //*****************************
            // OnEntryAwaken
            //*****************************
            protected void OnEntryAwaken(int _id)
            {
                EventEntryAwaken?.Invoke(idToEntry[_id]);
            }
            
            //*****************************
            // OnEntrySlept
            //*****************************
            protected void OnEntrySlept(int _id)
            {
                EventEntrySlept?.Invoke(idToEntry[_id]);
            }
            
            //*****************************
            // AddNewEntry
            //*****************************
            private int AddNewEntry(bool _state)
            {
                IPoolable product = factory.Produce() as IPoolable;

                bool error = product is null;
                if (error)
                {
                    throw new System.Exception("ObjectPool: Factory failed to produce entry which implements 'IPoolable' interface");
                }

                product.Id = ++lastAddedIndex;
                idToEntry.Add(lastAddedIndex, product);
                idToState.Add(lastAddedIndex, _state);
                
                EventEntryCreated?.Invoke(product);
                product.OnAdded();

                if (_state)
                {
                    AwakePeekEntry(true);
                }
                else
                {
                    SleepEntry(lastAddedIndex);
                }

                return lastAddedIndex;
            }
            
            //*****************************
            // MarkEntrySlept
            //*****************************
            private void SleepEntry(int _id)
            {
                sleepingEntries.Add(_id);
                sleepingEntriesStack.Push(_id);
                
                MakeSleepCallbacks(_id);
            }
            
            //*****************************
            // MakeSleepCallbacks
            //*****************************
            private void MakeSleepCallbacks(int _id)
            {
                idToState[_id] = false;
                idToEntry[_id].Deactivate();
                OnEntrySlept(_id);
            }
            
            //*****************************
            // MarkPeekEntryAwaken
            //*****************************
            // TODO: error here
            private int AwakePeekEntry(bool _ignoreSleepingEntries = false)
            {
                int result = default;
                
                // if we want to awake entry but we dont have any slept entries
                // used at AddNewEntry if we need to add completely new entries (dont have enough slept)
                if (_ignoreSleepingEntries)
                {
                    result = lastAddedIndex;
                }
                else
                {
                    result = sleepingEntriesStack.Pop();
                    sleepingEntries.Remove(result);
                }
                
                MakeAwakeCallbacks(result);
                return result;
            }

            //*****************************
            // MakeAwakeCallbacks
            //*****************************
            private void MakeAwakeCallbacks(int _Id)
            {
                idToState[_Id] = true;
                idToEntry[_Id].Activate();
                OnEntryAwaken(_Id);
            }

            //*****************************
            // Dispose
            //*****************************
            public void Dispose()
            {
                foreach (var item in idToEntry)
                {
                    item.Value.Dispose();
                }
                
                idToEntry.Clear();
                idToState.Clear();
                sleepingEntries.Clear();
                sleepingEntriesStack.Clear();
                awakeCycleToId.Clear();
                idToAwakeCycle.Clear();
                lastAddedIndex = -1;

                EventEntryAwaken  = null;
                EventEntryCreated = null;
                EventEntrySlept   = null;
            }
        }
        
    }
}