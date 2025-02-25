using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CameraController_Public
{
    public interface ICameraController : IModuleInit, IModuleLateUpdate
    {
        Vector3 P_Position { get; set; }

        /// <summary>
        /// Define target to follow
        /// </summary>
        /// <param name="_target"></param>
        void SetFollowTarget(Transform _target, bool _forceSnapPosition = false);

        /// <summary>
        /// Follow or not follow target. Sets true autotically when calling 'SetFollowTarget'
        /// </summary>
        /// <param name="_val"></param>
        void ToggleFollow(bool _val);

        /// <summary>
        /// Play camera impulse like shake e t c
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_direction"></param>
        /// <param name="_magnitude"></param>
        void PlayImpulse(CameraImpulseType _type, Vector3 _direction, float _magnitude);

        /// <summary>
        /// Toggles camera logic and render
        /// </summary>
        /// <param name="_val"></param>
        void ToggleCamera(bool _val);

        // casts position from camera center to ground
        bool RaycastPosition(out Vector3 _point);

        /// <summary>
        /// reset module values
        /// </summary>
        void ResetModule();

        /// <summary>
        /// Returns frustrum rectangle propjected on camera's surface
        /// </summary>
        /// <returns></returns>
        FrustumProjectionContainer GetFrustrumSurfaceProjection();
    }


    // *****************************
    // FrustumProjectionContainer
    // *****************************
    public struct FrustumProjectionContainer
    {
        public Vector3 up;
        public Vector3 right;

        // points, CV starting from upper left
        public Vector3[] points;

        public Vector3 center;
        public Vector3 normal;

        public bool isValid;
    }


    // *****************************
    // CameraImpulseType
    // *****************************
    public enum CameraImpulseType
    {
        Undef   = -1,
        Shake   = 0,
        Recoild
    }
}