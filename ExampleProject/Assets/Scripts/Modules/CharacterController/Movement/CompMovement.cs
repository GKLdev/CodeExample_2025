using GDTUtils;
using UnityEngine;

namespace Modules.CharacterController
{
    public static class CompMovement
    {
        // *****************************
        // SetMovementTargets 
        // *****************************
        public static void SetMovementTargets(State _state, Vector3 _direction) {
            _direction.Normalize();
            _state.dynamic.horMovement.SetTargetPercent(_direction.x);
            _state.dynamic.fwdMovement.SetTargetPercent(_direction.z);
        }

        // *****************************
        // OnUpdateFinished 
        // *****************************
        public static void OnUpdateFinished(State _state)
        {
            _state.dynamic.horMovement.SetTargetPercent(0f);
            _state.dynamic.fwdMovement.SetTargetPercent(0f);

            _state.dynamic.lastFramePosition = _state.root.position;
        }

        // *****************************
        // SetLookAtPoint 
        // *****************************
        public static void SetLookAtPoint(State _state, Vector3 _wSpacePos)
        {
            Vector3 dir = _wSpacePos - _state.root.position;

            dir         = Vector3.ProjectOnPlane(dir, _state.root.up);
            _wSpacePos  = _state.root.position + dir;
            _state.dynamic.needLookRotation = GDTMath.MoreNotEqual((_wSpacePos - _state.dynamic.lookAtTarget).magnitude, 0f, _state.config.floatPrecision);

            if (_state.dynamic.needLookRotation)
            {
                _state.dynamic.lookAtTarget = _wSpacePos;
                _state.dynamic.lookRotationDelta.SetTargetPercent(1f);
            }
        }

        // *****************************
        // SetNavTarget 
        // *****************************
        public static void SetNavTarget(State _state, Vector3 _targetPos)
        {
            SetupMovementMode(_state, MovementMode.Path);

            _state.dynamic.navData.pathTarget = _targetPos;
            _state.navAgent.SetDestination(_state.dynamic.navData.pathTarget);
            _state.dynamic.pathMovement.SetTargetPercent(1f);
        }

        // *****************************
        // OrderMoveViaNavmesh 
        // *****************************
        public static void OrderMoveViaNavmesh(State _state, Vector3 _direction)
        {
            if (_state.dynamic.movementMode == MovementMode.Path)
            {
                _state.navAgent.ResetPath();
            }

            _state.dynamic.navmeshMovementVelocity = _direction.normalized;
            SetupMovementMode(_state, MovementMode.NavmeshMovement);
            _state.dynamic.pathMovement.SetTargetPercent(1f);
        }

        // *****************************
        // SetupMovementMode 
        // *****************************
        public static void SetupMovementMode(State _state, MovementMode _mode)
        {
            bool resetPathAxis = 
                _state.dynamic.movementMode != MovementMode.Path && _mode == MovementMode.Path ||
                _state.dynamic.movementMode != MovementMode.NavmeshMovement && _mode == MovementMode.NavmeshMovement;
            if (resetPathAxis)
            {
                _state.dynamic.pathMovement.ResetAxis();
            }

            _state.dynamic.movementMode = _mode;

            switch (_mode)
            {
                case MovementMode.Undef:
                    break;
                case MovementMode.DirectControl:
                    CompNavigation.ToggleNavAgent(_state, false);
                    _state.collision.enabled = true;
                    break;
                case MovementMode.Path:
                    CompNavigation.ToggleNavAgent(_state, true);
                    _state.collision.enabled = false;
                    break;
                case MovementMode.NavmeshMovement:
                    CompNavigation.ToggleNavAgent(_state, true);
                    _state.collision.enabled = false;
                    break;
                default:
                    break;
            }
        }

        // *****************************
        // ForceChangePosition 
        // *****************************
        public static void ForceChangePosition(State _state, Vector3 _pos)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.initialized);
            _state.root.position                = _pos;
            _state.dynamic.currentPosition      = _pos;
            _state.dynamic.lastFramePosition    = _pos;
        }

        // *****************************
        // ForceChangeRotation 
        // *****************************
        public static void ForceChangeRotation(State _state, Quaternion _rotation)
        {
            _state.root.rotation = _rotation;

            StopRotation(_state);
        }

        // *****************************
        // ForceOrientation 
        // *****************************
        public static void ForceOrientation(State _state, Vector3 _direction)
        {
            _direction = Vector3.ProjectOnPlane(_direction.normalized, _state.root.up);
            _state.root.rotation = Quaternion.LookRotation(_direction, Vector3.up);

            StopRotation(_state);
        }

        // *****************************
        // StopRotation 
        // *****************************
        public static void StopRotation(State _state)
        {
            _state.dynamic.needLookRotation = false;
            _state.dynamic.lookRotationDelta.SetProgressPercent(0f);
        }
    }
}