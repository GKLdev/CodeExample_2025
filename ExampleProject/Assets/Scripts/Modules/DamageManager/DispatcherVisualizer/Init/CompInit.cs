using GDTUtils.Animation;
using System.Collections;
using UnityEngine;

namespace Modules.DispatcherVisualizer
{
    public static class CompInit
    {
        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state)
        {
            _state.alphaControl.Init();

            // prewarm for animator
            _state.alphaControl.alpha = 0f;
            _state.alphaControl.SetAlpha();

           _state.dynamic.animContainer = new AnimBehaviourInitContainer();
            _state.dynamic.animContainer.onAnimationEvent += (eventId, param) => { CompAnim.OnAnimEvent(_state, eventId, param); };
            _state.anim.keepAnimatorStateOnDisable = true;
            GDTAnimator.InitBehaviours(_state.anim, out _state.dynamic.animBhvs, _state.dynamic.animContainer);
        }

        // *****************************
        // Dispose 
        // *****************************
        public static void Dispose(State _state)
        {
            _state.initialized = false;
            _state.dynamic.animContainer.onAnimationEvent = null;
            _state.dynamic.target   = null;
            _state.dynamic.timeMgr  = null;
        }
    }
}