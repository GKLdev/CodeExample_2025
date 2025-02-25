using GDTUtils;
using UnityEngine;

namespace Modules.CharacterController
{
    public static class CompPhysics
    {
        // *****************************
        // OnUpdate
        // *****************************
        public static void OnUpdate(State _state)
        {
            UpdateDeltaTime(_state);

            bool skip = !_state.dynamic.updatePhysics;
            if (skip)
            {
                return;
            }

            _state.dynamic.currentPosition = _state.root.position;
            _state.dynamic.desiredPosition = _state.dynamic.currentPosition;

            bool navigationMode = 
                _state.dynamic.movementMode == MovementMode.Path ||
                _state.dynamic.movementMode == MovementMode.NavmeshMovement;

            if (navigationMode)
            {
                CompNavigation.OnUpdate(_state);
                UpdateVelocity(_state);
                CompVisual.Onpdate(_state);
                CompMovement.OnUpdateFinished(_state);
                return;
            }

            CastGround(_state, _state.dynamic.currentPosition);
            ProcessRotation(_state);
            ProcessMovementAxis(_state);
            ApplyGravity(_state);
            ApplyTranslation(_state);
            UpdateVelocity(_state);

            CompVisual.Onpdate(_state);
            CompMovement.OnUpdateFinished(_state);
        }

        // *****************************
        // ProcessMovementAxis
        // *****************************
        static void ProcessMovementAxis(State _state)
        {
            // apply modifiers
            HandleReverseMultiplier(_state.dynamic.fwdMovement);
            HandleReverseMultiplier(_state.dynamic.horMovement);

            // update axis
            _state.dynamic.fwdMovement.UpdateAxis(_state.dynamic.currentDeltaTime);
            _state.dynamic.horMovement.UpdateAxis(_state.dynamic.currentDeltaTime);

            // apply to desired pos
            Vector3 currentPos = _state.root.position;
            float forwardFactor = _state.dynamic.fwdMovement.GetProgress() * _state.dynamic.currentDeltaTime;
            float lateralfactor = _state.dynamic.horMovement.GetProgress() * _state.dynamic.currentDeltaTime;

            // apply to desired pos
            _state.dynamic.desiredPosition += _state.dynamic.screenFwd * forwardFactor + _state.dynamic.screenRight * lateralfactor;

            void HandleReverseMultiplier(IDynamicAxis _axis)
            {
                float currentSign = Mathf.Sign(_axis.GetProgress());
                float acceleration = _state.config.Acceleration;

                bool applyToZeroModifier = Mathf.Approximately(_axis.GetTarget(), 0f);
                if (applyToZeroModifier)
                {
                    acceleration *= _state.config.InertiaModifier;
                }
                else {
                    bool applyVertReverseMultiplier = currentSign != Mathf.Sign(_axis.GetTarget());
                    if (applyVertReverseMultiplier)
                    {
                        acceleration *= _state.config.ReverseAxisMultiplier;
                    }
                }

                _axis.SetParam(IDynamicAxis.AxisParamType.UpSpeed, acceleration);
                _axis.SetParam(IDynamicAxis.AxisParamType.DownSpeed, acceleration);
            }
        }

        // *****************************
        // ProcessRotation
        // *****************************
        public static void ProcessRotation(State _state)
        {
            Vector3 rotationDir = _state.dynamic.lookAtTarget - _state.dynamic.currentPosition;
            _state.dynamic.lookDirection = rotationDir;

            bool ignore = GDTMath.LessOREqual(rotationDir.magnitude, _state.config.MinimumLookTargetDistance, _state.config.floatPrecision);
            if (ignore)
            {
                return;
            }

            // debug
            Debug.DrawLine(_state.root.position, _state.dynamic.lookAtTarget, Color.red);

            float angle = Vector3.SignedAngle(_state.root.forward, rotationDir.normalized, Vector3.up);

            // reached target direction
            bool reachedTarget = GDTMath.LessOREqual(Mathf.Abs(angle), _state.config.floatPrecision);
            if (reachedTarget)
            {
                _state.dynamic.needLookRotation = false;
                _state.dynamic.lookRotationDelta.SetProgressPercent(0f);
                return;
            }

            // update axis
            _state.dynamic.lookRotationDelta.UpdateAxis(_state.dynamic.currentDeltaTime);

            // apply rotation
            float rotationFactor = 180f * _state.dynamic.lookRotationDelta.GetProgress() * _state.dynamic.currentDeltaTime;
            rotationFactor = Mathf.Clamp(rotationFactor, 0f, Mathf.Abs(angle));
            rotationFactor = rotationFactor * Mathf.Sign(angle);

            Quaternion rotationDelta = Quaternion.Euler(0f, rotationFactor, 0f);
            _state.root.rotation    *= rotationDelta;

            Debug.DrawLine(_state.root.position, _state.root.position + _state.root.forward * 2f, Color.blue);
        }

        // *****************************
        // ProcessMovementAxis
        // *****************************
        static void ApplyTranslation(State _state)
        {
            // TODO: support step resolve mode

            // collision
            _state.dynamic.desiredPosition = ProcessCollision(_state);

            // apply pos
            _state.root.position = _state.dynamic.desiredPosition;
        }

        // *****************************
        // UpdateVelocity
        // *****************************
        static void UpdateVelocity(State _state)
        {
            _state.dynamic.currentVelocity = _state.dynamic.desiredPosition - _state.dynamic.lastFramePosition; //_state.dynamic.desiredPosition - _state.dynamic.currentPosition;
        }

        // *****************************
        // ProcessCollision
        // *****************************
        static Vector3 ProcessCollision(State _state)
        {
            Vector3 desiredPos = GDTUtils.GDTCollision.ResolveCollisionForPosition(_state.dynamic.desiredPosition, ref _state.dynamic.collisionData);
            return desiredPos;
        }

        static Vector3 ProcessCollision(State _state, Vector3 _startingPos, Vector3 _translation)
        {
            Vector3 desiredPos = _startingPos + _translation;

            desiredPos = GDTUtils.GDTCollision.ResolveCollisionForPosition(desiredPos, ref _state.dynamic.collisionData);
            return desiredPos;
        }

        // *****************************
        // ApplyGravity
        // *****************************
        static void ApplyGravity(State _state)
        {
            bool skip = _state.dynamic.isGrounded || !_state.dynamic.useGravity;
            if (skip)
            {
                return;   
            }

            _state.dynamic.desiredPosition += -Vector3.up * _state.dynamic.currentGravityForce * _state.dynamic.currentDeltaTime;
        }

        // *****************************
        // CastGround
        // *****************************
        /// <summary>
        /// casts ground and sets 'isGrounded' , 'surfaceNormal', 'slopeAngle'
        /// </summary>
        /// <returns>Resolved position after trying to move down by cast distance. If no ground casted returns '_startingPos'</returns>
        static Vector3 CastGround(State _state, Vector3 _startingPos, float _customCastDistance = -1f)
        {
            // get casting distance
            float castingDistance = _customCastDistance < 0f ? _state.config.GroundTestDistance : _customCastDistance;

            // preprocess collision for ground testing
            Vector3 result = ProcessCollision(_state, _startingPos, -_state.root.transform.up * castingDistance);

            _state.dynamic.isGrounded = false;
            _state.dynamic.slopeAngle = -1;
            float maxSlopeAngle = -1;

            _state.dynamic.surfaceNormal = Vector3.zero;

            // find at least one contact with valid normal angle
            for (int i = 0; i < _state.dynamic.collisionData.sharedData.contactsCount; i++)
            {
                var contact = _state.dynamic.collisionData.sharedData.contactPoints[i];
                if (CheckIfGroundNormalValid(_state, contact.normal, out float normalAngle))
                {
                    _state.dynamic.isGrounded = true;
                    maxSlopeAngle = Mathf.Max(maxSlopeAngle, normalAngle);
                    _state.dynamic.slopeAngle = maxSlopeAngle;

                    _state.dynamic.surfaceNormal = (_state.dynamic.surfaceNormal + contact.normal).normalized;
                }
            }

            // if no ground found - set default value
            if (!_state.dynamic.isGrounded)
            {
                _state.dynamic.surfaceNormal = _state.root.up;
                return _startingPos;
            }

            return result;
        }

        // *****************************
        // CheckIfGroundNormalValid 
        // *****************************
        static bool CheckIfGroundNormalValid(State _state, Vector3 _normal, out float normalAngle)
        {
            float dot = Vector3.Dot(_state.root.up, _normal);

            normalAngle = Vector3.Angle(_state.root.up, _normal);

            bool validNormalAngle = GDTMath.MoreOREqual(dot, 0f);
            bool result = validNormalAngle && GDTMath.LessOREqual(normalAngle, _state.config.MaxSlopeAngle);

            return result;
        }

        // *****************************
        // UpdateDeltaTime
        // *****************************
        static void UpdateDeltaTime(State _state)
        {
            _state.dynamic.currentDeltaTime = _state.dynamic.timeMng.GetDeltaTime(_state.dynamic.timeLayer);
        }
    }
}