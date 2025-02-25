using GDTUtils;
using GDTUtils.Animation;
using Modules.CharacterVisualController_Public;
using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

// TODO:
// actions mgr and acnimation actions!
// mb even movement anims as action (activates when vlocity is not zero and deactivates when it becomes zero + by other actions)
// callbacks from animation (with params)

namespace Modules.CharacterVisualController
{
    public class CharacterVisualController : LogicBase, ICharacterVisualController
    {
        public Vector3 P_MovementVelocity { get => state.dynamic.velocity; set => state.dynamic.velocity = value; }

        public GameObject P_GameObjectAccess => gameObject;

        public event System.Action<AnimEventType, int> OnAnimationFinished;

        [SerializeField]
        State state;

        [Inject]
        IModuleManager moduleMgr;

        private bool isDisposed = false;
        // *****************************
        // InitModule
        // *****************************
        public void InitModule()
        {
            if (state.initialized) {
                return;
            }

            CompInit.Init(state, moduleMgr, this);
        }

        // *****************************
        // Setup
        // *****************************
        public void Setup(VisualControllerSetupData _data)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.Setup(state, _data);
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

            CompUpdate.Update(state);
        }

        // *****************************
        // PlayAction
        // *****************************
        public void PlayAnimation(AnimationType _animation)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAnimation.StartAnimation(state, _animation);
        }

        // *****************************
        // ForceStopAnimation
        // *****************************
        public void ForceStopAnimation()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAnimation.ForceDefaultAnimationState(state);
        }

        // *****************************
        // RaiseAnimFinishedEvent
        // *****************************
        public void RaiseAnimFinishedEvent(AnimEventType _eventId, int _param)
        {
            OnAnimationFinished?.Invoke(_eventId, _param);
        }

        // *****************************
        // Dispose
        // *****************************
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            CompInit.Dispose(state);
            OnAnimationFinished     = null;
            isDisposed              = true;
        }

        // *****************************
        // OnAdded
        // *****************************
        public void OnAdded()
        {
        }

        // *****************************
        // OnAwake
        // *****************************
        public void OnAwake()
        {
            bool needSetActive = !gameObject.activeInHierarchy;
            if (needSetActive)
            {
                gameObject.SetActive(true);
            }
        }

        // *****************************
        // OnSlept
        // *****************************
        public void OnSlept()
        {
            CompInit.Reset(state);

            bool needSetInnactive= gameObject.activeInHierarchy;
            if (needSetInnactive)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        public bool initialized = false;

        public ConfigCharacterVisualController  config;
        public Animator                         animation;
        public DynamicData  dynamic     = new();

        // *****************************
        // DynamicData
        // *****************************
        [System.Serializable]
        public class DynamicData
        {
            public ITimeManager timeMgr;

            public Vector3      velocity;
            public TimeManager_Public.TimeLayerType timeLayer;

            public float deltaTime;

            public VisualControllerSetupData setupData;

            public AnimBhvBase[] animBehaviours;
            public AnimBehaviourInitContainer animatorInitContainer;
        }
    }
}


namespace Modules.CharacterVisualController_Public
{
    // *****************************
    // VisualControllerSetupData
    // *****************************
    public class VisualControllerSetupData
    {
        public Transform    transform;
        public float        maxForwardVelocity;
        public float        maxLateraLVelocity;
    }
}