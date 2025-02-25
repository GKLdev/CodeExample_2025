using GDTUtils;
using GDTUtils.Extensions;
using GDTUtils.Patterns.Factory;
using Modules.ActionsManger_Public;
using Modules.CharacterVisualController;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Modules.ActionsManger
{
    public class ActionsManager : LogicBase, IActionsManager
    {
        [SerializeField]
        State state;

        [Inject]
        IModuleManager moduleMgr;

        // *****************************
        // InitModule 
        // *****************************
        public void InitModule()
        {
            if (state.initialized) 
            {
                return;
            }

            CompInit.Init(state, moduleMgr);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            if (!state.initialized)
            {
                return;
            }

            state.dynamic.aliasToAction.ForEach(x => x.Value.Update());
        }

        // *****************************
        // InitializeActionsStorage 
        // *****************************
        public void InitializeActionsStorage(string _alias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.PrepareStorage(state, _alias);
        }


        // *****************************
        // DisposeActionsStorage 
        // *****************************
        public void DisposeActionsStorage(string _alias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.DisposeStorage(state, _alias);
        }

        // *****************************
        // DisposeAllActions 
        // *****************************
        public void DisposeAllActions(bool _exceptDefault = true)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.DisposeAll(state, _exceptDefault);
        }

        // *****************************
        // AddAction 
        // *****************************
        public IAction AddAction(string _alias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return CompActionManagement.AddAction(state, _alias);
        }

        // *****************************
        // RemoveAction 
        // *****************************
        public void RemoveAction(IAction _action)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompActionManagement.RemoveAction(state, _action);
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        public bool initialized = false;

        public DynamicData dynamic = new();

        // *****************************
        // DynamicData
        // *****************************
        public class DynamicData
        {
            public IModuleManager   moduleMgr;
            public IReferenceDb     reference;
            public Dictionary<string, IActionsStorage> aliasToAction = new(); // id from actions list
            
            public ActionCreationData actionCreationData = new();

            // *****************************
            // ActionCreationData
            // *****************************
            public class ActionCreationData
            {
                public StringBuilder    stringBuilder               = new StringBuilder();
                public string           replacementPattern          = typeof(Action_Example).ToString();
                public string           replacementConfigPattern    = typeof(ConfigActionExample).ToString();
            }
        }
    }

    // *****************************
    // IActionsStorage
    // *****************************
    public interface IActionsStorage : IDisposable
    {
        void Setup(State _state, ConfigActionBase _config, int _prewarmPoolElements = -1);
        void Update();
        bool CheckIfContains(ActionBase _Action);
        ActionBase AddAction();
        void RemoveAction(ActionBase _entry);

        bool P_IsDefaultAction { get; set; }

        TConfig GetConfig<TConfig>() where TConfig : ConfigActionBase;
    }

    // *****************************
    // ActionsStorage
    // *****************************
    public class ActionsStorage<TAction, TConfig> : IActionsStorage
        where TAction : ActionBase, new()
        where TConfig : ConfigActionBase, new()
    {
        public bool P_IsDefaultAction { get; set; }

        private HashSet<IActionInternal>    updatableActions    = new();
        private List<ActionBase>            addQueue            = new();
        private List<ActionBase>            removeQueue         = new();
        private IObjectPool                 actionsPool;

        private bool                            enablePooling = false;
        private ObjectPoolFactory<EntryFactory> poolFactory;
        private EntryFactory                    entryFactory;
        private State                           state;

        private bool addQueueTriggered      = false;
        private bool removalQueueTriggered  = false;

        public TConfig config;

        // *****************************
        // Setup
        // *****************************
        /// <summary>
        /// Called only on module initialization
        /// </summary>
        public void Setup(State _state, ConfigActionBase _config, int _prewarmPoolElements = -1)
        {
            entryFactory = new(_state, this);
            
            enablePooling = _prewarmPoolElements >= 0;
            if (enablePooling)
            {
                poolFactory     = new(entryFactory);
                actionsPool     = poolFactory.Produce();
                actionsPool.SetLimitType(PoolLimitType.None);
                actionsPool.Init(_prewarmPoolElements);
            }

            state   = _state;
            config  = _config as TConfig;
        }

        // *****************************
        // CheckIfContains
        // *****************************
        public bool CheckIfContains(ActionBase _action)
        {
            return updatableActions.Contains(_action);
        }

        // *****************************
        // AddAction
        // *****************************
        public ActionBase AddAction()
        {
            ActionBase result;

            if (enablePooling)
            {
                GDTUtils.IPoolable entry = actionsPool.AwakeEntry();

                result = entry as ActionBase;
            }
            else
            {
                result = entryFactory.Produce() as ActionBase;
            }

            QueueForAdd(result);
            return result;
        }

        // *****************************
        // RemoveAction
        // *****************************
        /// <summary>
        /// Remove and reset data set (will be exclude from updatable list)
        /// </summary>
        /// <param name="_entry"></param>
        public void RemoveAction(ActionBase _entry)
        {
            QueueForRemoval(_entry);

            if (enablePooling) 
            {
                actionsPool.SleepEntry(_entry);
            }
            else
            {
                _entry.Dispose();
            }
        }

        // *****************************
        // Update
        // *****************************
        public void Update() {
            HandleQueues();

            foreach (var item in updatableActions)
            {
                item.Update(Time.deltaTime);
            }
        }

        // *****************************
        // GetConfig
        // *****************************
        public T GetConfig<T>() where T : ConfigActionBase
        {
            T result = config as T;

            if (result == null)
            {
                Debug.Assert(false, $"Failed to cast config={config.name} to target type={typeof(T)}");
            }

            return result;
        }

        // *****************************
        // QueueForAdd
        // *****************************
        void QueueForAdd(ActionBase _action) {
            addQueue.Add(_action);
            addQueueTriggered = true;
        }

        // *****************************
        // QueueForRemoval
        // *****************************
        void QueueForRemoval(ActionBase _action)
        {
            removeQueue.Add(_action);
            removalQueueTriggered = true;
        }


        // *****************************
        // HandleQueues
        // *****************************
        void HandleQueues()
        {
            if (addQueueTriggered)
            {
                addQueueTriggered = false;
                addQueue.ForEach(x => updatableActions.Add(x));
                addQueue.Clear();
            }

            if (removalQueueTriggered)
            {
                removalQueueTriggered = false;
                removeQueue.ForEach(x => updatableActions.Remove(x));
                removeQueue.Clear();
            }
        }

        // *****************************
        // Dispose
        // *****************************
        public void Dispose()
        {
            actionsPool.Dispose();

            addQueue.Clear();
            removeQueue.Clear();

            poolFactory     = null;
            entryFactory    = null;
            state           = null;
        }

        // *****************************
        // EntryFactory
        // *****************************
        class EntryFactory : IConcreteFactory<GDTUtils.IPoolable>
        {
            public State state;
            public IActionsStorage actionContainer;
            // *****************************
            // Init
            // *****************************
            public EntryFactory(State _state, IActionsStorage _container)
            {
                state           = _state;
                actionContainer = _container;
            }

            // *****************************
            // Produce
            // *****************************
            public GDTUtils.IPoolable Produce()
            {
                var result = new TAction();
                result.Setup(state, actionContainer);
                return result;
            }

            IFactoryProduct GDTUtils.Patterns.Factory.IFactory.Produce()
            {
                return Produce();
            }
        }
    }
}