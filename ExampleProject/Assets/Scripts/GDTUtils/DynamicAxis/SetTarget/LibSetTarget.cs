using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.DynamicAxis
{
    public static class LibSetTarget 
    {

        // *****************************
        // SetTarget 
        // *****************************
        public static void SetTarget(State _state, float _tgt)
        {
            _state.dynamicData.target = Mathf.Clamp(_tgt, _state.dynamicData.lowerLimit, _state.dynamicData.upperLimit);
        }
    }
}
