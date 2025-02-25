using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CameraController
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr)
        {
            Debug.Assert(_state.root != null, "Camera root must be defined!");
            Debug.Assert(_state.cameraHolder != null, "Camera holder must be defined!");
            Debug.Assert(_state.camera != null, "Camera must be defined!");

            // copy config
            Debug.Assert(_state.config != null, "Camera config must be defined!");
            _state.config = ScriptableObject.Instantiate(_state.config);

            // dependencies
            _state.dynamic.timeMgr = _moduleMgr.Container.Resolve<ITimeManager>();

            // finish
            _state.initialized = true;
        }

        // *****************************
        // Reset
        // *****************************
        public static void Reset(State _state)
        {
            _state.dynamic.needToFollowTarget   = false;
            _state.dynamic.lastFrameTargetPos   = Vector3.zero;
            _state.dynamic.currentTargetPos     = Vector3.zero;
            _state.dynamic.desiredPos           = Vector3.zero;
            _state.dynamic.target               = null;
        
        }
    }
}