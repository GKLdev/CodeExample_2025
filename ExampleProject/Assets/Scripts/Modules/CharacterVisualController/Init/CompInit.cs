using GDTUtils.Animation;
using Modules.CharacterVisualController_Public;
using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController
{
    public static class CompInit
    {
        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr, CharacterVisualController _controller)
        {
            // dependencies
            _state.dynamic.timeMgr = _moduleMgr.Container.Resolve<ITimeManager>();

            // animation
            AnimBehaviourInitContainer initContainer = new AnimBehaviourInitContainer();

            initContainer.onAnimationEvent = (id, param) => { CompAnimation.OnAnimationEvent(_state, _controller, id, param); };
            GDTAnimator.InitBehaviours(_state.animation, out _state.dynamic.animBehaviours, initContainer);

            //_state.animation.writeDefaultValuesOnDisable = true;
            _state.animation.keepAnimatorStateOnDisable = true;

            // finish
            _state.initialized = true;
        }

        // *****************************
        // Reset 
        // *****************************
        public static void Reset(State _state)
        {
            // set to default state and reset locomotion
            CompAnimation.ForceDefaultAnimationState(_state);

            // reset all triggers
            foreach (var param in _state.animation.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    _state.animation.ResetTrigger(param.name);
                }
            }
        }

        // *****************************
        // Setup 
        // *****************************
        public static void Setup(State _state, VisualControllerSetupData _data)
        {
            _state.dynamic.setupData = _data;
        }

        // *****************************
        // Dispose 
        // *****************************
        public static void Dispose(State _state)
        {
            GDTAnimator.DisposeBehaviours(_state.dynamic.animBehaviours);
            _state.dynamic.animatorInitContainer.Dispose();
            _state.dynamic.setupData = null;
        }
    }
}