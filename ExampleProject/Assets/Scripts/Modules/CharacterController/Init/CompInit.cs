using GDTUtils;
using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Modules.CharacterController
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMng)
        {
            Debug.Assert(_state.collision != null, "Collision must be defined!");

            // init axis logic
            FactoryDynamicAxis factoryDynamicAxis = new FactoryDynamicAxis();

            _state.dynamic.fwdMovement          = factoryDynamicAxis.Produce();
            _state.dynamic.horMovement          = factoryDynamicAxis.Produce();
            _state.dynamic.lookRotationDelta    = factoryDynamicAxis.Produce();
            _state.dynamic.pathMovement         = factoryDynamicAxis.Produce(); 

            // copy config
            Debug.Assert(_state.config != null, "Default config must be defined!");
            _state.config = ScriptableObject.Instantiate(_state.config);

            // collision
            _state.dynamic.collisionData = GDTCollision.GenerateCdtResolveData(
                _state.root, 
                _state.collision, 
                _state.config.CollisionSettings.CollisionMask, 
                _state.config.CollisionSettings.CollidersBufferCount, 
                _state.config.CollisionSettings.ContactsGatherDistance);

            // nav agent params
            Debug.Assert(_state.navAgent != null, "NavAgent must be defined!");
            _state.navAgent.acceleration    = _state.config.Acceleration;
            _state.navAgent.speed           = _state.config.MaxSpeed;
            _state.navAgent.updateRotation  = false;

            // dependencies
            _state.dynamic.timeMng = _moduleMng.Container.Resolve<ITimeManager>();

            // time
            _state.dynamic.timeLayer = _state.config.timeLayer;

            // reset all daat to default
            Reset(_state);

            // finish
            _state.initialized = true;
        }

        // *****************************
        // Reset
        // *****************************
        public static void Reset(State _state)
        {
            // axis
            SetupAxis(_state.dynamic.fwdMovement, -_state.config.MaxSpeed, _state.config.MaxSpeed, _state.config.Acceleration, _state.config.Acceleration);
            SetupAxis(_state.dynamic.horMovement, -_state.config.MaxSpeed, _state.config.MaxSpeed, _state.config.Acceleration, _state.config.Acceleration);

            float alignMaxSpeed = Mathf.Approximately(_state.config.AlignTime, 0f) ? float.MaxValue : 1f / _state.config.AlignTime;
            float alignAcceleration = Mathf.Approximately(_state.config.AlignAccelerateTime, 0f) ? float.MaxValue : alignMaxSpeed / _state.config.AlignAccelerateTime;
            
            SetupAxis(_state.dynamic.lookRotationDelta, 0f, alignMaxSpeed, alignAcceleration, 0f);
            SetupAxis(_state.dynamic.pathMovement, 0f, _state.config.MaxSpeed, _state.config.Acceleration, _state.config.Acceleration);

            _state.dynamic.fwdMovement.ResetAxis();
            _state.dynamic.horMovement.ResetAxis();
            _state.dynamic.lookRotationDelta.ResetAxis();
            _state.dynamic.pathMovement.ResetAxis();

            CompMovement.SetupMovementMode(_state, _state.config.DefaultMovementMode);

            // values
            _state.dynamic.direction            = Vector3.zero;
            _state.dynamic.currentVelocity      = Vector3.zero;
            _state.dynamic.desiredPosition      = _state.root.position;
            _state.dynamic.useGravity           = true;
            _state.dynamic.currentGravityForce  = _state.config.DefaultGravityForce;
            _state.dynamic.currentPosition      = _state.root.position;
            _state.dynamic.lookAtTarget         = Vector3.zero;
            _state.dynamic.lookDirection        = Vector3.zero;
            _state.dynamic.screenFwd            = Vector3.forward;
            _state.dynamic.screenRight          = Vector3.right;
            _state.dynamic.isGrounded           = false;
            _state.dynamic.slopeAngle           = 0f;
            _state.dynamic.surfaceNormal        = Vector3.zero;
            _state.dynamic.lastFramePosition    = _state.root.position;

            _state.dynamic.navmeshMovementVelocity      = Vector3.zero;
            _state.dynamic.simplifiedNavmeshMovement    = false;

            _state.dynamic.movementMode                 = MovementMode.Undef;
            _state.dynamic.navData.pathTarget           = Vector3.zero;
            _state.dynamic.navData.isMovingAlongPath    = false;

            // settings
            _state.dynamic.updatePhysics = false;
        }

        // *****************************
        // SetupAxis
        // *****************************
        static void SetupAxis(IDynamicAxis _axis, float speedMin, float speedMax, float accelerationUp, float accelerationDown) {
            _axis.SetParam(IDynamicAxis.AxisParamType.Min, speedMin);
            _axis.SetParam(IDynamicAxis.AxisParamType.Max, speedMax);
            _axis.SetParam(IDynamicAxis.AxisParamType.UpSpeed, accelerationUp);
            _axis.SetParam(IDynamicAxis.AxisParamType.DownSpeed, accelerationDown);
        }
    }
}