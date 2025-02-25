using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.DynamicAxis
{
    public static class LibInitAxis
    {

        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state, float _min, float _max, float _upSpeed, float _downSpeed)
        {
            _state.dynamicData.axisProgress = 0f;
            _state.dynamicData.target       = 0f;
            _state.dynamicData.lowerLimit   = _min;
            _state.dynamicData.upperLimit   = _max;
            _state.dynamicData.upSpeed      = Mathf.Abs(_upSpeed);
            _state.dynamicData.downSpeed    = Mathf.Abs(_downSpeed);

            bool error = Mathf.Approximately(_state.dynamicData.lowerLimit, _state.dynamicData.upperLimit);
            if (error)
            {
                throw new System.Exception("DynamicAxis: lowerLimit equals to upperLimit! Those values must not be equal.");
            }

            error = _min > _max;
            if (error)
            {
                throw new System.Exception("DynamicAxis: lowerLimit exceeds upperLimit! MUST be lower that upper limit!");
            }
        }

        // *****************************
        // Reset 
        // *****************************
        public static void Reset(State _state)
        {
            _state.dynamicData.axisProgress = 0f;
            _state.dynamicData.target       = 0f;
        }
    }

}
