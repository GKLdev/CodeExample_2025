using GDTUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterController
{
    public static class CompNavigation
    {
        // *****************************
        // OnUpdate
        // *****************************
        public static void OnUpdate(State _state)
        {
            Debug.Assert(_state.navAgent != null, "Collision must be defined!");

            switch (_state.dynamic.movementMode)
            {
                case MovementMode.Undef:
                    break;
                case MovementMode.Path:
                    PathMovement(_state);
                    break;
                case MovementMode.NavmeshMovement:
                    NavmeshOffsetMovement(_state);
                    break;
                default:
                    break;
            }
        }

        // *****************************
        // NavmeshOffsetMovement
        // *****************************
        public static void NavmeshOffsetMovement(State _state)
        {
            float   velocityScalar  = CalculateVelocityScalar(_state) * _state.dynamic.currentDeltaTime;
            Vector3 offset          = _state.dynamic.navmeshMovementVelocity.normalized * velocityScalar;
            Vector3 desiredPos      = _state.root.position + offset;

            _state.navAgent.Move(offset);
            _state.dynamic.desiredPosition = desiredPos;

            CompMovement.SetLookAtPoint(_state, desiredPos);
            CompPhysics.ProcessRotation(_state);
        }

        // *****************************
        // PathMovement
        // *****************************
        public static void PathMovement(State _state)
        {
            bool pathEmpty = _state.navAgent.path.corners.Length == 0;
            if (pathEmpty)
            {
                ForceStopPath(_state);
                return;
            }

            DebugDrawPath(_state);

            Vector3 nextPathPos     = _state.navAgent.steeringTarget;
            Vector3 desiredDir      = (nextPathPos - _state.root.position).normalized;
            float   velocityScalar  = CalculateVelocityScalar(_state);

            _state.navAgent.velocity = desiredDir * velocityScalar;

            CompMovement.SetLookAtPoint(_state, nextPathPos);
            CompPhysics.ProcessRotation(_state);

            Vector3 distance = _state.navAgent.destination - _state.root.position;
            bool reachedDestination = GDTMath.LessOREqual(distance.magnitude, _state.config.floatPrecision);
            if (reachedDestination)
            {
                ForceStopPath(_state);
            }
        }

        // *****************************
        // ForceStopPath
        // *****************************
        static void ForceStopPath(State _state)
        {
            _state.navAgent.velocity = Vector3.zero;
            _state.navAgent.ResetPath();
            _state.dynamic.pathMovement.ResetAxis();
        }

        // *****************************
        // DebugDrawPath
        // *****************************
        static void DebugDrawPath(State _state)
        {
            bool skip = !_state.dynamic.isDebugMode;
            if (skip)
            {
                return;
            }

            var path = _state.navAgent.path.corners;
            if (path.Length == 1)
            {
                Debug.DrawLine(_state.dynamic.currentPosition, _state.dynamic.currentPosition + Vector3.up, Color.cyan, Time.deltaTime);
                return;
            }

            for (int i = 1; i < path.Length; i++)
            {
                Vector3 prevPoint = path[i - 1];
                Vector3 currPoint = path[i];

                Debug.DrawLine(prevPoint, currPoint, Color.cyan, Time.deltaTime);
            }
        }

        // *****************************
        // CalculateVelocityScalar
        // *****************************
        static float CalculateVelocityScalar(State _state)
        {
            float result = 0f;

            if (_state.dynamic.simplifiedNavmeshMovement)
            {
                result = _state.config.MaxSpeed;
            }
            else
            {
                _state.dynamic.pathMovement.UpdateAxis(_state.dynamic.currentDeltaTime);
                result = _state.dynamic.pathMovement.GetProgress();
            }

            return result;
        }

        // *****************************
        // ToggleNavAgent
        // *****************************
        public static void ToggleNavAgent(State _state, bool _val)
        {
            bool ignore = _state.navAgent.enabled == _val;
            if (ignore)
            {
                return;
            }

            _state.navAgent.enabled = _val;
        }
    }

    // *****************************
    // NavigationData
    // *****************************
    public class NavigationData
    {
        public Vector3 pathTarget;
        public bool isMovingAlongPath;
    }

    // *****************************
    // NavigationData
    // *****************************
    public enum MovementMode
    {
        Undef = -1,
        DirectControl = 1, //wasd movement
        Path, // path movement using navmesh
        NavmeshMovement
    }
}