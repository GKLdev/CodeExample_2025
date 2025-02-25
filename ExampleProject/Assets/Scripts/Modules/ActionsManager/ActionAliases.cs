using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger_Public
{
    // TODO: make scriptable object config and code-generate those aliases
    /// <summary>
    /// Purpose:
    /// Stores action aliases. Aliases provide access to actions from ActionsManger.
    /// </summary>
    public static class ActionAliases
    {
        // Default actions - DO NOT DELETE
        public const string ExampleAction           = "Example_Action";
        public const string ExampleActionPrewarm    = "Example_Action_Prewarm";

        // Animations
        public const string AnimLocomotion      = "Anim_Locomotion";
        public const string AnimSwordAttack0    = "Anim_Sword_Attack0";
        
        // Abilities
        public const string AbilityMobAttack        = "Ability_MobAttack";
    }
}