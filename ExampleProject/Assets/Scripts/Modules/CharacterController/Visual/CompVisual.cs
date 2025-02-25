using Modules.CharacterVisualController_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterController
{
    public static class CompVisual
    {
        // *****************************
        // Onpdate
        // *****************************
        public static void Onpdate(State _state)
        {
            bool ignore = _state.dynamic.visual == null;
            if (ignore)
            {
                return;
            }

            _state.dynamic.visual.P_MovementVelocity = _state.dynamic.currentVelocity;
            _state.dynamic.visual.OnUpdate();
        }

        // *****************************
        // AttachVisual
        // *****************************
        public static void AttachVisual(State _state, ICharacterVisualController _visual)
        {
            _state.dynamic.visual = _visual;
            _state.dynamic.visual.InitModule();

            _state.dynamic.setupData.transform = _state.root.transform;
            _state.dynamic.setupData.maxForwardVelocity = _state.config.MaxSpeed;
            _state.dynamic.setupData.maxLateraLVelocity = _state.config.MaxSpeed;
            _state.dynamic.visual.Setup(_state.dynamic.setupData);
        }
    }
}