using System;
using System.Collections;
using System.Collections.Generic;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Modules.TimeManager
{
    public static class CompInit
    {
        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state)
        {
            // setup time layers
            var elements = Enum.GetValues(typeof(TimeLayerType));
            _state.dynamicData.dictLayers.Clear();
            float timeNow = Time.unscaledTime;
            
            for (int i = 0; i < elements.Length; i++)
            {
                _state.dynamicData.dictLayers.Add((TimeLayerType)i, new TimeLayer(timeNow));
            }

            _state.dynamicData.initialised = true;
        }
    }
}