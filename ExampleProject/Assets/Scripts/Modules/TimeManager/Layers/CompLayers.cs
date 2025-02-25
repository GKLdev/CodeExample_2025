using System.Collections;
using System.Collections.Generic;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Modules.TimeManager
{
    public static class CompLayers
    {
        // *****************************
        // GetLayer 
        // *****************************
        public static TimeLayer GetLayer(State _state, TimeLayerType _layer)
        {
            return _state.dynamicData.dictLayers[_layer];
        }

        // *****************************
        // GetDeltaTime 
        // *****************************
        public static float GetDeltaTime(State _state, TimeLayerType _layer)
        {
            var layer = GetLayer(_state, _layer);
            return layer.GetDeltaTime();
        }
        
        // *****************************
        // SetTimeScale 
        // *****************************
        public static void SetTimeScale(State _state, TimeLayerType _layer, float _scale)
        {
            var layer = GetLayer(_state, _layer);
            layer.SetScale(_scale);
        }
        
        // *****************************
        // GetTimeScale 
        // *****************************
        public static float GetTimeScale(State _state, TimeLayerType _layer)
        {
            var layer = GetLayer(_state, _layer);
            return layer.GetScale();
        }
        
        // *****************************
        // GetTimeSinceStartup 
        // *****************************
        public static float GetTimeSinceStartup(State _state, TimeLayerType _layer)
        {
            var layer = GetLayer(_state, _layer);
            return layer.GetTimeSinceStartup();
        }
    }
}