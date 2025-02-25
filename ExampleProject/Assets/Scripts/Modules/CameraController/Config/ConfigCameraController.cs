using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CameraController
{
    [CreateAssetMenu(fileName = "ConfigCameraController", menuName = "Configs/Camera")]
    public class ConfigCameraController : ScriptableObject
    {
        [Tooltip("Maximum offset camera can take when following target")]
        public float MaxChaseOffset;

        [Tooltip("Maximum follow speed")]
        public float MaxFollowSpeed;

        [Tooltip("Default vamera height above ground level")]
        public float DefaultHeight;

        [Tooltip("Default camera angle")]
        public float DefaultAngle;

        public TimeLayerType TimeLayer;

        [Tooltip("Raycsting Mask")]
        public LayerMask RaycastingMask;

        [Tooltip("DefaultHeight + overcast")]
        public float RaycastingOvercast;
    }
}