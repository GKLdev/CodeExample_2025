using GDTUtils;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager_Public;
using Modules.DispatcherVisualizer_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.DamageDispatcher
{
    public class DamageDispatcher : LogicBase, IDamageDispatcher
    {
        public int Id { get; set; }

        public CATEGORY_DAMAGEDISPATCHERS P_ElementType { get; set; }

        public GameObject P_GameObjectAccess => gameObject;

        [Inject]
        IModuleManager moduleMgr;

        [SerializeField]
        State state;


        // *****************************
        // Init
        // *****************************
        public void Init(ConfigDamageDispatcher _config)
        {
            if (state.initialized)
            {
                return;
            }

            state.config = _config;
            CompInit.Init(state, moduleMgr);
            state.dynamic.self = this;
            state.visualizer.Value?.Setup(this, state.dynamic.timeLayer);
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

            CompUpdate.OnUpdate(state);
            CompUpdate.UpdateVisualizer(state);
        }

        // *****************************
        // Setup
        // *****************************
        public void StartDispatcher(DamageDispatcherData _data)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.Reset(state);
            CompUpdate.ApplyData(state, _data);
        }

        // *****************************
        // StopDispatcher
        // *****************************
        public void StopDispatcher()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompUpdate.StopDispatcher(state);
        }

        // *****************************
        // Modify
        // *****************************
        public void Modify()
        {
            throw new System.NotImplementedException();
        }

        // *****************************
        // GetVisualData
        // *****************************
        public DamageDispatcherVisualizationData GetVisualData()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return state.dynamic.visualizationData;
        }

        // *****************************
        // ResetData
        // *****************************
        public void ResetData()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.Reset(state);
        }

        // *****************************
        // OnRelayTriggerEnter
        // *****************************
        public void OnRelayTriggerEnter(Collider _other)
        {
            if (!state.initialized || state.dynamic.data == null)
            {
                return;
            }

            CompRegisterDamageable.TryRegisterTarget(state, _other);
        }

        // *****************************
        // OnRelayTriggerExit
        // *****************************
        public void OnRelayTriggerExit(Collider _other)
        {
            if (!state.initialized)
            {
                return;
            }

            CompRegisterDamageable.TryUnregisterTarget(state, _other);
        }

        // IPoolable:

        // *****************************
        // Activate
        // *****************************
        public void Activate()
        {
            state.dynamic.justAddedToPool = false;
            state.root.gameObject.SetActive(true);
        }

        // *****************************
        // Deactivate
        // *****************************
        public void Deactivate()
        {
            CompInit.Reset(state);
            state.root.gameObject.SetActive(false);

            if (!state.dynamic.justAddedToPool)
            {
                state.visualizer.Value?.Stop();
            }
        }

        // *****************************
        // Dispose
        // *****************************
        public void Dispose()
        {
            moduleMgr = null;

            state.visualizer.Value?.Dispose();
            CompInit.Dispose(state);
        }

        // *****************************
        // OnAdded
        // *****************************
        public void OnAdded()
        {
            state.dynamic.justAddedToPool = true;
        }

        // *****************************
        // OnVisualizerFinished
        // *****************************
        public void OnVisualizerFinished()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompUpdate.OnVisualizerFinished(state);
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        public bool initialized = false;

        public Transform                root;
        public Transform                triggerRoot;
        public ConfigDamageDispatcher   config;
        public DynamicData              dynamic = new();
        public Rigidbody                rBody;
        public Collider[]               trigger;

        public SerializedInterface<IDispatcherVisualizer> visualizer;

        public bool debug = false;

        // *****************************
        // DynamicData
        // *****************************
        [System.Serializable]
        public class DynamicData
        {
            public IDamageDispatcher        self;
            public DamageDispatcherData     data = new();
            public bool                     visualizerReportedFinished  = false;

            public DispatchStage            stage           = DispatchStage.Innactive;
            public float deltaTime = 0f;

            public int chargeCooldown   = -1;
            public int tickCooldown     = -1;
            public int ticksCount;

            public Vector3 defaultScale;

            public HashSet<IDamageable> registeredTargets = new();

            public ITimeManager     timeMgr;
            public IDamageManager   damageMgr;
            public TimeManager_Public.TimeLayerType timeLayer;

            public DamageDispatcherVisualizationData visualizationData = new();

            public bool justAddedToPool = false;
        }
    }

    // *****************************
    // DispatchStage
    // *****************************
    public enum DispatchStage
    {
        Innactive = 0,
        Charging,
        ReadyForDispatch,
        AwaitingForTick,
        AwaitingForVisualizer
    }
}