using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController_Public
{
    /// <summary>
    /// Purpose:
    /// Animations config. Store animation trigger names for AnimationController
    /// </summary>

    // TODO: make auto generated like AliasesConfig
    public static class CharacterAnimations
    {
        // *****************************
        // GetAnimation 
        // *****************************
        public static string GetAnimation(AnimationType _type)
        {
            return Animations[_type];
        }

        // Enum to animationName
        private static Dictionary<AnimationType, string> Animations = new()
        {
            { AnimationType.TestAnimation, TestAnimation },

            // TODO: add custom animations here
            { AnimationType.SwordAttack, SwordAttack }
        };

        // Animations

        // default
        private const string TestAnimation = "Test";

        // TODO: add custom animations here
        private const string SwordAttack = "Sword_Attack";
    
        
    }

    // *****************************
    // AnimationType 
    // *****************************
    public enum AnimationType
    {
        TestAnimation = 0,

        // TODO: add custom animations here
        SwordAttack
    }

    // *****************************
    // CharacterAnimActionData
    // *****************************
    public enum AnimEventType
    {
        Finished = 0,
        DealDamage,
        SpawnFX,
        DespawnFX
    }
}