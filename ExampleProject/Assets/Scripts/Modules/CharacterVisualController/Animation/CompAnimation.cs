using GDTUtils;
using Modules.CharacterVisualController_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController
{
    public static class CompAnimation
    {
        // *****************************
        // SetMovementParams 
        // *****************************
        // movement blend tree control 
        public static void SetMovementParams(State _state, bool _suppress = false) {

            float paramHor              = 0f;
            float paramFwd              = 0f;
            float locomotionDir         = 1f;
            float lateralVelocitySign   = 0f;

            var transf = _state.dynamic.setupData.transform;


            // assign params
            if (!_suppress)
            {
                Vector3 dir = _state.dynamic.velocity.normalized;
                Vector3 prj = Vector3.ProjectOnPlane(_state.dynamic.velocity, transf.up);

                Vector3 fwdComponent = Vector3.Project(prj, transf.forward);
                Vector3 horComponent = Vector3.Project(prj, transf.right);

                float dot = Vector3.Dot(transf.forward, prj.normalized);
                locomotionDir   = GDTMath.LessOREqual(dot, Mathf.Abs(_state.config.BlendTreeBlendingThreshold)) ? dot : Mathf.Sign(dot);

                //// sing is forced to 1f because locomotionDir splits locomotion to forward adn backward alraeady
                paramFwd = GetNormalizedVelocityValue(fwdComponent.magnitude, 1f, _state.dynamic.setupData.maxForwardVelocity);

                dot = Vector3.Dot(transf.right, horComponent.normalized);
                lateralVelocitySign = Mathf.Approximately(dot, 0f)  ? 1f : Mathf.Sign(dot);
                paramHor            = GetNormalizedVelocityValue(horComponent.magnitude, lateralVelocitySign, _state.dynamic.setupData.maxLateraLVelocity);
            }
 
            _state.animation.SetFloat(_state.config.AVar_ForwardAxis, paramFwd);
            _state.animation.SetFloat(_state.config.AVar_HorizontalAxis, paramHor);
            _state.animation.SetFloat(_state.config.AVar_LocomotionDir, locomotionDir);

            float GetNormalizedVelocityValue(float _magnitude, float _sign, float _maxValue)
            {
                return Mathf.Clamp01(_magnitude / (_maxValue * _state.dynamic.deltaTime)) * _sign;
            }
        }

        // *****************************
        // StartAnimation 
        // *****************************
        public static void StartAnimation(State _state, AnimationType _type)
        {
            var triggerName = CharacterAnimations.GetAnimation(_type);
            _state.animation.SetTrigger(triggerName);
        }

        // *****************************
        // ForceDefaultAnimationState 
        // *****************************
        public static void ForceDefaultAnimationState(State _state)
        {
            _state.animation.SetFloat(_state.config.AVar_LocomotionDir, 1f);
            _state.animation.SetFloat(_state.config.AVar_ForwardAxis, 0f);
            _state.animation.SetFloat(_state.config.AVar_HorizontalAxis, 0f);

            _state.animation.Play(_state.config.AVar_DefaultStateName, 0);
        }

        // *****************************
        // OnAnimationEvent 
        // *****************************
        public static void OnAnimationEvent(State _state, CharacterVisualController _controller,  int _eventId, int _eventParam)
        {
            switch (_eventId)
            {
                // anim end event
                case 0:
                    // _eventId == 0 is for anim end
                    // _eventParam == -1 - is for final animation in queue
                    _controller.RaiseAnimFinishedEvent(AnimEventType.Finished, _eventParam);
                    break;
                case 1:
                    _controller.RaiseAnimFinishedEvent(AnimEventType.DealDamage, _eventParam);
                    break;
                default:
                    break;
            }
        }
    }
}