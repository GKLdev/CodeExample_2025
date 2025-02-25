using UnityEngine;
using Modules.ReferenceDb_Public;

namespace Modules.CharacterController_Public
{
    [CreateAssetMenu(fileName = "ConfigCharacterController", menuName = "Configs/Character/Controller")]
    public class ConfigPlayerController : DbEntryBase
    {
        [Tooltip("Max forward movement speed")]
        public float MaxSpeed;

        [Tooltip("Forward accleration")]
        public float Acceleration;

        [Tooltip("Multiplier applied when axis changes it sign")]
        public float ReverseAxisMultiplier = 3f;

        [Tooltip("Multiplier applied when axis target becomes zero")]
        public float InertiaModifier = 2f;

        [Tooltip("How fast rotation should complete 180 turn. In degrees")]
        public float AlignTime;

        [Tooltip("How fast align speed increases")]
        public float AlignAccelerateTime;

        [Tooltip("Minimum distance for look target. Target will be ignored withing that range")]
        public float MinimumLookTargetDistance = 0.01f;

        [Tooltip("Collision")]
        public CharacterController.CollisionSettingsContainer CollisionSettings;

        [Tooltip("Gravity for play collider - doesnt really matter so much since its a topdown game, but still")]
        public float DefaultGravityForce = 9.8f;

        [Tooltip("Time layer")]
        public TimeManager_Public.TimeLayerType timeLayer;

        [Tooltip("Distance to cast ground")]
        public float GroundTestDistance = 0.01f;

        [Tooltip("Max slope angle")]
        public float MaxSlopeAngle = 45f;

        [Tooltip("Default moveme mode")]
        public CharacterController.MovementMode DefaultMovementMode = CharacterController.MovementMode.DirectControl;

        [Tooltip("Precision")]
        public float floatPrecision = 0.001f;
    }
}

namespace Modules.CharacterController
{

        [System.Serializable]
    public class CollisionSettingsContainer
    {
        public LayerMask    CollisionMask;
        public int          CollidersBufferCount;

        [Tooltip("A distance for collision detector to gather other collider")]
        public float        ContactsGatherDistance;
    }
}