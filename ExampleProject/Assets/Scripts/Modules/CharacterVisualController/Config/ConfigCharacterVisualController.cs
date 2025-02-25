using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController
{
    [CreateAssetMenu(fileName = "ConfigCharacterVisualController", menuName = "Configs/Character/Visual")]
    public class ConfigCharacterVisualController : DbEntryBase
    {
        [Header("Animation")]
        public string AVar_ForwardAxis;
        public string AVar_HorizontalAxis;
        public string AVar_LocomotionDir;

        public string AVar_TriggerAction;
        public string AVar_ActionId;

        public string AVar_DefaultStateName;

        public float BlendTreeBlendingThreshold = 0.05f;
    }
}