using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController
{
    public static class CompUpdate
    {
        // *****************************
        // Update 
        // *****************************
        public static void Update(State _state)
        {
            GetDeltaTIme(_state);
            ProcessLocomotion(_state);
        }

        // *****************************
        // ProcessLocomotion 
        // *****************************
        static void ProcessLocomotion(State _state)
        {
            bool ignore = _state.dynamic.setupData == null || _state.dynamic.setupData.transform == null;
            if (ignore)
            {
                return;
            }

            bool suppress = Mathf.Approximately(_state.dynamic.velocity.magnitude, 0f);
            CompAnimation.SetMovementParams(_state, suppress);
        }

        // *****************************
        // GetDeltaTIme 
        // *****************************
        static void GetDeltaTIme(State _state)
        {
            _state.dynamic.deltaTime = _state.dynamic.timeMgr.GetDeltaTime(_state.dynamic.timeLayer);
        }
    }
}