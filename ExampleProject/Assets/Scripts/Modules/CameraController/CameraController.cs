using Modules.CameraController_Public;
using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.CameraController
{
    public class CameraController : LogicBase, ICameraController
    {
        public Vector3 P_Position { get => state.root.position; set => state.root.position = value; }

        [SerializeField]
        State state;

        [Inject]
        IModuleManager moduleMgr;

        // *****************************
        // InitModule
        // *****************************
        public void InitModule()
        {
            if (state.initialized)
            {
                return;
            }

            CompInit.Init(state, moduleMgr);
            ToggleCamera(false);
        }

        // *****************************
        // ResetModule
        // *****************************
        public void ResetModule()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompInit.Reset(state);
        }

        // *****************************
        // ToggleCamera
        // *****************************
        public void ToggleCamera(bool _val)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.isActive  = _val;
            state.camera.enabled    = _val;
        }

        // *****************************
        // RaycastPosition
        // *****************************
        public bool RaycastPosition(out Vector3 _point)
        {
            bool result = false;
            RaycastHit hit;
            Ray ray = state.camera.ScreenPointToRay(Input.mousePosition);

            _point = Vector3.zero;

            if (Physics.Raycast(ray, out hit, state.config.DefaultHeight + state.config.RaycastingOvercast, state.config.RaycastingMask))
            {
                _point = hit.point;
                result = true;
            }

            return result;
        }

        // *****************************
        // OnLateUpdate
        // *****************************
        public void OnLateUpdate()
        {
            if (!state.initialized || !state.dynamic.isActive) 
            {
                return;
            }

            CompUpdate.Update(state);
        }

        // *****************************
        // PlayImpulse
        // *****************************
        public void PlayImpulse(CameraImpulseType _type, Vector3 _direction, float _magnitude)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
        }

        // *****************************
        // SetFollowTarget
        // *****************************
        public void SetFollowTarget(Transform _target, bool _forceSnapPosition = false)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.target = _target;

            if (_forceSnapPosition)
            {
                state.root.position = state.dynamic.target.position;
            }

            ToggleFollow(state.dynamic.target != null);
        }

        // *****************************
        // ToggleFollow
        // *****************************
        public void ToggleFollow(bool _val)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.needToFollowTarget = _val;
        }


        // *****************************
        // FrustumPointsContainer
        // *****************************
        public FrustumProjectionContainer GetFrustrumSurfaceProjection()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return CompFrustrumUtility.GetFrustrumSurfaceProjection(state);
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State 
    {
        public bool initialized = false;

        public ConfigCameraController config;
        public Transform    root;
        public Transform    cameraHolder;
        public Camera       camera;

        public bool                     isDebug = false;
        public FrustrumPointsOutputMode frustrumProjectionMode;

        public DynamicData dynamic = new();

        // *****************************
        // DynamicData
        // *****************************
        public class DynamicData {

            public ITimeManager timeMgr;

            public Transform    target;
            public bool         needToFollowTarget = false;

            public Vector3  lastFrameTargetPos   = Vector3.zero;
            public Vector3  currentTargetPos     = Vector3.zero;
            public Vector3  desiredPos           = Vector3.zero;
            public float    currentDeltaTime = 0;

            public Vector3[] projectedFrustrumPoints = new Vector3[4];

            // CV order, starting from uper left
            public Vector3[] pointDirections = new Vector3[4];
            public Vector3[] pointOrigins    = new Vector3[4];

            public bool isActive = false;
        }
    }

    // *****************************
    // FrustrumPointsOutputMode
    // *****************************
    public enum FrustrumPointsOutputMode
    {
        ToUpper = 0,
        ToLower = 1
    }
}