using GDTUtils;
using GDTUtils.Animation;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager;
using Modules.DamageManager_Public;
using Modules.DispatcherVisualizer_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.DispatcherVisualizer
{
    // _state.anim.keepAnimatorStateOnDisable = true;
    public class DispatcherVisualizer : LogicBase, IDispatcherVisualizer
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
            state.dynamic.self      = this;
            state.dynamic.timeMgr   = moduleMgr.Container.Resolve<ITimeManager>(); 
            
            CompInit.Init(state);

            state.initialized = true;
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate(float _delta)
        {
            if (!state.initialized || state.dynamic.target == null)
            {
                return;
            }
            CompAnim.OnUpdate(state, _delta);
        }

        // *****************************
        // Setup 
        // *****************************
        public void Setup(IDamageDispatcher _target, TimeLayerType _layer)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.target    = _target;
            state.dynamic.timeLayer = _layer;
        }

        // *****************************
        // StartVisualizer 
        // *****************************
        public void StartVisualizer(bool _loopMode = false)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CheckTarget();
            CompAnim.Start(state, _loopMode);
        }

        // *****************************
        // Stop 
        // *****************************
        public void Stop()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CheckTarget();
            CompAnim.Stop(state);
        }

        // *****************************
        // OrderStopLoop 
        // *****************************
        public void OrderStopLoop()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CheckTarget();
            CompAnim.OrderStopLoop(state);
        }

        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
            CompInit.Dispose(state);
            moduleMgr = null;
        }

        // *****************************
        // CheckTarget 
        // *****************************
        void CheckTarget()
        {
            if (state.dynamic.target == null)
            {
                Debug.Assert(false, $"Dispatcher visualizer={this} does have specified target!");
            }
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        public bool                         initialized = false;
        public Animator                     anim;
        public ConfigDispatcherVisualizer   config;
        public MaterialAlphaControl         alphaControl;
        public DynamicData                  dynamic     = new();

        // *****************************
        // DynamicData
        // *****************************
        [System.Serializable]
        public class DynamicData
        {
            public ITimeManager timeMgr;

            public AnimBhvBase[]                animBhvs;
            public AnimBehaviourInitContainer   animContainer;
            public IDamageDispatcher            target;

            public float progressPlaybackSpeed           = 0f;
            public float desiredAnimSpeed   = 1f;
            public bool  isStopped          = false;
            public bool  isLoopMode         = false;

            public DispatcherVisualizer self;
            public TimeLayerType        timeLayer;
        }
    }
}