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
        void SetFollowTarget(Transform _target, bool _forceSnapPosition = false);

        /// <summary>
        /// Follow or not follow target. Sets true automatically when calling 'SetFollowTarget'
        /// </summary>
        void ToggleFollow(bool _val);

        /// <summary>
        /// Play camera impulse like shake e t c
        /// </summary>
        void PlayImpulse(CameraImpulseType _type, Vector3 _direction, float _magnitude);

        /// <summary>
        /// Toggles camera logic and render
        /// </summary>
        void ToggleCamera(bool _val);

        /// <summary>
        /// Casts position from camera center to ground
        /// </summary>
        bool RaycastPosition(out Vector3 _point);

        /// <summary>
        /// reset module values
        /// </summary>
        void ResetModule();

        /// <summary>
        /// Returns frustrum rectangle projected on camera's surface
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