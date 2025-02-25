using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.DynamicAxis
{
    public static class LibUpdateAxis
    {

        // *****************************
        // UpdateAxis 
        // *****************************
        public static void UpdateAxis(State _state, float _deltaTime)
        {
            ProgressAxis(_state, _deltaTime);
        }

        // *****************************
        // ProgressAxis 
        // *****************************
        static void ProgressAxis(State _state, float _deltaTime)
        {
            float sign  = _state.dynamicData.axisProgress < _state.dynamicData.target ? 1f : -1f;
            float speed = sign > 0f ? _state.dynamicData.upSpeed : _state.dynamicData.downSpeed;

            _state.dynamicData.axisProgress += sign * speed * _deltaTime;
            _state.dynamicData.axisProgress = Mathf.Clamp(_state.dynamicData.axisProgress, _state.dynamicData.lowerLimit, _state.dynamicData.upperLimit);

            // *** reached before movement *** //
            bool reachedTarget = 
                sign > 0f && GDTMath.MoreOREqual(_state.dynamicData.axisProgress, _state.dynamicData.target, speed * _deltaTime) ||
                sign < 0f && GDTMath.LessOREqual(_state.dynamicData.axisProgress, _state.dynamicData.target, speed * _deltaTime);

            if (reachedTarget)
            {
                _state.dynamicData.axisProgress = _state.dynamicData.target;
            }
        }
    }
}
