using GDTUtils;
using Modules.CharacterVisualController_Public;
using Modules.ModuleManager_Public;
using Modules.CharacterController_Public;
using Modules.TimeManager_Public;
using UnityEngine;
using Zenject;
using UnityEngine.AI;

namespace Modules.CharacterController
{
    public class CharacterController : LogicBase, ICharacterController
    {
        [SerializeField]
        public State state;

        [Inject]
        private IModuleManager  moduleMng;
        private bool            isDisposed;

        public Vector3      P_Position      { get => state.root.position; set => CompMovement.ForceChangePosition(state, value); }
        public Quaternion   P_Rotation      { get => state.root.rotation; set => CompMovement.ForceChangeRotation(state, value); }
        public Vector3      P_Orientation   { get => state.root.forward; set => CompMovement.ForceOrientation(state, value); }

        // *****************************
        // InitModule
        // *****************************
        public void InitModule()
        {
            if (state.initialized) {
                return;
            }

            CompInit.Init(state, moduleMng);
        }

        // *****************************
        // SetupConfig
        // *****************************
        public void SetupConfig(ConfigPlayerController _config)
        {
            if (state.initialized)
            {
                Debug.Assert(false, "Cant setup config if character controller is initialized!");
            }

            state.config = _config;
        }

        // *****************************
        // AttachVisual
        // *****************************
        public void AttachVisual(ICharacterVisualController _visual)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);

            _visual.P_GameObjectAccess.transform.position = P_Position;
            _visual.P_GameObjectAccess.transform.rotation = P_Rotation;
            _visual.P_GameObjectAccess.transform.SetParent(state.rootVisualController);

            state.dynamic.visual = _visual;
            state.dynamic.visual.InitModule();

            CompVisual.AttachVisual(state, _visual);
        }

        // *****************************
        // Move
        // *****************************
        public void Move(Vector3 _direction)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompMovement.SetMovementTargets(state, _direction);
        }

        // *****************************
        // MoveViaNavmesh
        // *****************************
        public void MoveViaNavmesh(Vector3 _direction)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompMovement.OrderMoveViaNavmesh(state, _direction);
        }

        // *****************************
        // ToggleSimplifiedPathMode
        // *****************************
        public void ToggleSimplifiedPathMode(bool _val)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.simplifiedNavmeshMovement = _val;
        }

        // *****************************
        // AlignToDirection
        // *****************************
        public void LookAt(Vector3 _wSpacePos)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);

            if (state.dynamic.movementMode != MovementMode.Path)
            {
                CompMovement.SetLookAtPoint(state, _wSpacePos);
            }
        }

        // *****************************
        // OnUpdate
        // *****************************
        public void OnUpdate()
        {
            if (!state.initialized) {
                return;
            }

            CompPhysics.OnUpdate(state);
        }

        // *****************************
        // ResetModule
        // *****************************
        public void ResetModule()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.Reset(state);
        }

        // *****************************
        // TogglePhysics
        // *****************************
        public void TogglePhysics(bool _val)
        {
            state.dynamic.updatePhysics = _val;

            if (state.dynamic.movementMode == MovementMode.Path)
            {
                state.navAgent.isStopped = !_val;
                
                if (_val)
                {
                    state.navAgent.ResetPath();
                }
                else
                {
                    state.navAgent.velocity = Vector3.zero;
                }
            }
        }

        // *****************************
        // SetNavigationTarget
        // *****************************
        public void SetNavigationTarget(Vector3 _targetPos)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompMovement.SetNavTarget(state, _targetPos);
        }

        // *****************************
        // SetDirectMovementMode
        // *****************************
        public void SetDirectMovementMode()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompMovement.SetupMovementMode(state, MovementMode.DirectControl);
        }

        // *****************************
        // GetVisualController
        // *****************************
        public ICharacterVisualController GetVisualController()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return state.dynamic.visual;
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

            isDisposed = true;
        }

        // *****************************
        // OnAdded
        // *****************************
        public void OnAdded()
        {
            //throw new System.NotImplementedException();
        }

        // *****************************
        // OnAwake
        // *****************************
        public void OnAwake()
        {
            state.root.gameObject.SetActive(true);
        }

        // *****************************
        // OnSlept
        // *****************************
        public void OnSlept()
        {
            CompInit.Reset(state);
            state.root.gameObject.SetActive(false);
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State {
        public bool         initialized = false;
        public DynamicData  dynamic = new();

        public Transform    root;
        public Transform    rootVisualController;
        public NavMeshAgent navAgent;
        public MeshCollider collision;

        public ConfigPlayerController config;

        // *****************************
        // DynamicData
        // *****************************
        [System.Serializable]
        public class DynamicData {
            public ITimeManager timeMng;

            public ICharacterVisualController visual;

            public IDynamicAxis fwdMovement;
            public IDynamicAxis horMovement;
            public IDynamicAxis lookRotationDelta;
            public IDynamicAxis pathMovement;

            public Vector3 direction            = Vector3.zero;
            public Vector3 currentVelocity      = Vector3.zero;
            public Vector3 desiredPosition      = Vector3.zero;
            public Vector3 currentPosition      = Vector3.zero;
            public Vector3 lastFramePosition    = Vector3.zero;
            public Vector3 lookAtTarget     = Vector3.zero;
            public Vector3 lookDirection    = Vector3.zero;
            public Vector3 screenFwd        = Vector3.forward;
            public Vector3 screenRight      = Vector3.right;
            public Vector3 surfaceNormal    = Vector3.zero;
            
            public Vector3 navmeshMovementVelocity = Vector3.zero;

            public NavigationData   navData         = new();
            public MovementMode     movementMode    = MovementMode.Undef;

            public float currentDeltaTime;

            public bool updatePhysics       = false;
            public bool useGravity          = false;
            public bool needLookRotation    = false;
            public bool isGrounded          = false;
            public bool isDebugMode         = false;

            public bool simplifiedNavmeshMovement = false;

            public GDTUtils.Collision.CollisionResolveData  collisionData;
            public VisualControllerSetupData                setupData = new VisualControllerSetupData();

            public float currentGravityForce = 0f;
            public float slopeAngle = 0f;
            public TimeManager_Public.TimeLayerType timeLayer;
        }
    }
}