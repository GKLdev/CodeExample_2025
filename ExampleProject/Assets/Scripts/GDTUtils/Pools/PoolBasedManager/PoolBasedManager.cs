using System;
using System.Collections;
using System.Collections.Generic;
using GDTUtils;
using GDTUtils.Patterns.Factory;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using IFactory = GDTUtils.Patterns.Factory.IFactory;

namespace GDTUtils
{
    public interface IElementTypeAccess<TElementsEnum>
    {
        TElementsEnum P_ElementType { get; set; }
    }
    
    public class PoolBasedManager<TElementsEnum, TElementIFace, TElementFactory> : IDisposable, IModuleUpdate
        where TElementsEnum : Enum
        where TElementIFace : class, IPoolable, IModuleUpdate, IElementTypeAccess<TElementsEnum>
        where TElementFactory : PbmFactoryBase, new()
    {
        private State state    = new();
        private bool  disposed = false;
        
        private List<PoolTypeSettings<TElementsEnum>> poolSettings;

        public PoolBasedManager(IModuleManager _moduleMng, List<PoolTypeSettings<TElementsEnum>> _poolSettings)
        {
            poolSettings       = _poolSettings;
            state.moduleMng    = _moduleMng;
            state.referenceDb  =  state.moduleMng.Container.Resolve<IReferenceDb>();

            Init();
        }

        // *****************************
        // Init 
        // *****************************
        private void Init()
        {
            if (state.initialised)
            {
                return;
            }
            
            // form factories
            for (int i = 0; i < poolSettings.Count; i++)
            {
                //Enum.GetValues(item.elementType)
                PoolTypeSettings<TElementsEnum> item     = poolSettings[i];
                DbEntryBase                     entry    = state.referenceDb.GetEntry<DbEntryBase>(Convert.ToInt32(item.elementType));
                TElementsEnum                   category = item.elementType;
                Transform                       parent   = item.root;
                
                var factory = new TElementFactory();
                factory.Init(entry, state.moduleMng.Container, category, parent);

                state.dictFactories.Add(category, new ObjectPoolFactory<TElementFactory>(factory));
            }

            // form pools
            state.dictElementPools.Clear();
            foreach (var item in poolSettings)
            {
                ObjectPoolFactory<TElementFactory> poolFactory = state.dictFactories[item.elementType];
                
                IObjectPool pool = poolFactory.Produce();
                pool.Init(item.prewarmElements);
                pool.SetLimitType(item.limitType);
                
                pool.EventEntryAwaken += OnEntryAwaken;
                pool.EventEntrySlept  += OnEntrySlept;

                state.dictElementPools.Add(item.elementType, pool);
            }

            state.initialised = true;
        }

        // *****************************
        // OnEntryAwaken 
        // *****************************
        private void OnEntryAwaken(IPoolable _poolable)
        {
            TElementIFace projectile = _poolable as TElementIFace;

            if (state.isRunningUpdate)
            {
                throw new System.Exception("ProjectileManager: Trying to add poolable object to update sequence while update is performing!");
            }
            else
            {
                state.updatableEntries.Add(projectile);
            }
        }

        // *****************************
        // OnEntrySlept 
        // *****************************
        private void OnEntrySlept(IPoolable _poolable)
        {
            TElementIFace projectile = _poolable as TElementIFace;

            if (state.isRunningUpdate)
            {
                state.queueForRemoveFromUpdate.Add(projectile);
            }
            else
            {
                state.updatableEntries.Remove(projectile);
            }
        }
        
        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            if (!state.initialised)
            {
                return;
            }
            
            state.isRunningUpdate = true;

            foreach (var item in state.updatableEntries)
            {
                bool ignore = state.queueForRemoveFromUpdate.Contains(item);
                if (ignore)
                {
                    continue;
                }
             
                item.OnUpdate();
            }

            // remove queued
            foreach (var item in state.queueForRemoveFromUpdate)
            {
                state.updatableEntries.Remove(item);
            }

            state.queueForRemoveFromUpdate.Clear();
            state.isRunningUpdate = false;
        }
        
        // *****************************
        // Awake 
        // *****************************
        public TElementIFace AwakeEntry(TElementsEnum _elementType)
        {
            var container = state.dictElementPools[_elementType];
            
            var result = state.dictElementPools[_elementType].AwakeEntry();
            return result as TElementIFace;
        }
        
        // *****************************
        // SleepEntry 
        // *****************************
        public void SleepEntry(TElementIFace _element)
        {
            TElementsEnum type = _element.P_ElementType;

            bool isActive = state.dictElementPools[type].IsEntryActive(_element);
            if (!isActive)
            {
                return;
            }

            state.dictElementPools[type].SleepEntry(_element);
        }
        
        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            state.referenceDb = null;
            state.moduleMng   = null;

            foreach (var item in state.dictElementPools)
            {
                item.Value.Dispose();
            }

            state.dictElementPools.Clear();
            state.dictFactories.Clear();
            state.updatableEntries.Clear();
            state.queueForAddToUpdate.Clear();
            state.queueForRemoveFromUpdate.Clear();

            state.initialised = false;
            disposed          = true;
        }
                
        // *****************************
        // State 
        // *****************************
        public class State
        {
             public IModuleManager moduleMng;
             public IReferenceDb   referenceDb;

             public Dictionary<TElementsEnum, IObjectPool>                        dictElementPools    = new();
             public Dictionary<TElementsEnum, ObjectPoolFactory<TElementFactory>> dictFactories       = new();

             public HashSet<TElementIFace> updatableEntries = new();
             public bool isRunningUpdate     = false;

             public HashSet<TElementIFace> queueForAddToUpdate      = new();
             public HashSet<TElementIFace> queueForRemoveFromUpdate = new();

             public bool initialised = false;
        }
    }

    // *****************************
    // PoolTypeSettings 
    // *****************************
    [System.Serializable]
    public class PoolTypeSettings<TElementsEnum>
        where TElementsEnum : Enum
    {
        public readonly int              prewarmElements = 0;
        public readonly TElementsEnum    elementType;
        public readonly Transform        root;
        public readonly PoolLimitType    limitType;

        public PoolTypeSettings(TElementsEnum _enum, Transform _root, int _prewarmElements, PoolLimitType _limitType)
        {
            prewarmElements = _prewarmElements;
            elementType     = _enum;
            root            = _root;
            limitType       = _limitType;
        }
    }

    // *****************************
    // PbmFactoryBase 
    // *****************************
    public class PbmFactoryBase : IConcreteFactory<IPoolable>
    {
        protected DiContainer container;
        protected Transform   parent;
        protected IPoolable   result;
        private   bool        disposed = false;
        
        public virtual void Init(DbEntryBase _entry, DiContainer _container, Enum _category, Transform _parent)
        {
            container = _container;
            parent    = _parent;
            
            OnCreated();
        }

        // *****************************
        // OnCreated 
        // *****************************
        protected virtual void OnCreated()
        {
            
        }

        // *****************************
        // Produce 
        // *****************************
        public virtual IPoolable Produce()
        {
            return result;
        }

        IFactoryProduct IFactory.Produce()
        {
            return Produce();
        }
        
        // *****************************
        // Dispose 
        // *****************************
        public virtual void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed  = true;
            container = null;
            parent    = null;
        }
    }
}