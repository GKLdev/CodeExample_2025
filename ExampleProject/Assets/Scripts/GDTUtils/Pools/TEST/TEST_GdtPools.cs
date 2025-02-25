using System;
using System.Collections;
using System.Collections.Generic;
using GDTUtils.Patterns.Factory;
using UnityEngine;

namespace GDTUtils.TEST
{
    public class TEST_GdtPools : MonoBehaviour
    {

        public int poolInitialEntries;
        public int hideById;

        private ObjectPoolFactory<EntryFactory> fac;
        private EntryFactory entryFac;
        private IObjectPool pool;

        private List<IPoolable> entries;
        
        //*****************************
        // Awake
        //*****************************
        private void Awake()
        {
            Debug.Log("GDTPool Test: init started...");
            InitPool();
            pool.Init(poolInitialEntries);
            Debug.Log("GDTPool Test: ...init finished");
        }

        //*****************************
        // InitPool
        //*****************************
        void InitPool()
        {
            entryFac   = new EntryFactory();
            fac        = new ObjectPoolFactory<EntryFactory>(entryFac);
            pool       = fac.Produce();
            
            pool.EventEntrySlept += OnSlept;
            pool.EventEntryAwaken += OnActivated;
        }

        //*****************************
        // Update
        //*****************************
        private void Update()
        {
            if (Input.GetKeyDown("t"))
            {
                RunTests();
            }

            if (Input.GetKeyDown("1"))
            {
                pool.SetLimitType(PoolLimitType.Cycle);
                entries = pool.AwakeEntries(poolInitialEntries);
            }

            if (Input.GetKeyDown("2"))
            {
                var item = entries[hideById];
                pool.SleepEntry(item);
            }

            if (Input.GetKeyDown("3"))
            {
                pool.AwakeEntry();
            }
        }

        //*****************************
        // OnActivated
        //*****************************
        private void OnActivated(IPoolable _entry)
        {
            Debug.Log($"GDTPool Test: Event OnActivated id={(_entry as PoolEntry)?.Id}");
        }

        //*****************************
        // OnSlept
        //*****************************
        private void OnSlept(IPoolable _entry)
        {
            Debug.Log($"GDTPool Test: Event OnSlept id={(_entry as PoolEntry)?.Id}");
        }

        //*****************************
        // RunTests
        //*****************************
        private void RunTests()
        {
            Debug.Log("GDTPool Test: Begin...");
            var entries = pool.AwakeEntries(poolInitialEntries + 2);
            pool.SleepEntries(entries);
            entries = pool.AwakeEntries(poolInitialEntries);
            pool.Dispose();
            Debug.Log("GDTPool Test: ...Finished");
        }

        //--------------------------------------------------------------------------------------------------------------
        
        //*****************************
        // PoolEntry
        //*****************************
        private class PoolEntry : IPoolable, IDisposable
        {
            public bool isActive = false;

            public int Id { get; set; }

            public void Activate()
            {
                isActive = true;
                Debug.Log($"Entry={Id} is Activated");
            }

            public void Deactivate()
            {
                isActive = false;
                Debug.Log($"Entry={Id} is Deativated");
            }

            public void OnAdded()
            {
                Debug.Log($"Entry={Id} is Added");
            }

            public void Dispose()
            {
                Debug.Log($"Entry={Id} is Disposed");
            }
        }
        
        //*****************************
        // EntryFactory
        //*****************************
        private class EntryFactory : IConcreteFactory<IPoolable>
        {
            public IPoolable Produce()
            {
                PoolEntry entry = new PoolEntry();
                return entry;
            }

            IFactoryProduct IFactory.Produce()
            {
                return Produce();
            }
        }
    }
}