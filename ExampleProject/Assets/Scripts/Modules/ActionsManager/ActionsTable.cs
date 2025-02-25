using Actions;
using Actions.Abilities;
using Modules.ActionsManger;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger_Public
{
    /// <summary>
    /// Purpose:
    /// Config for actions which links action alias to action type and its settings
    /// </summary>
    
    // TODO: Make this auto generated
    public static class ActionsTable
    {
        const int expectedMaxEnemiesCount = 100 + 1; // 1 is player itself

        /// <summary>
        /// All actions definition
        /// </summary>
        public static Dictionary<string, (System.Type, int, int)> Actions = new()
        {
            // Default actions - DO NOT DELETE
            { ActionAliases.ExampleAction,          (typeof(Action_Example), (int)CATEGORY_ACTIONCONFIGS.Action_Example, -1) },
            { ActionAliases.ExampleActionPrewarm,   (typeof(Action_Example_Prewarm), (int)CATEGORY_ACTIONCONFIGS.Action_Example, 4) },

            // TODO: add custom actions here
            { ActionAliases.AbilitySwordAttack,     (typeof(Action_AttackSword), (int)CATEGORY_ABILITYCONFIGS.Ability_SwordAttack, 1) },
            { ActionAliases.AbilityMobAttack,       (typeof(Action_MobAttack), (int)CATEGORY_ABILITYCONFIGS.Ability_MobAttack, 1) }
        };

        /// <summary>
        /// Actions loaded by default on game start
        /// </summary>
        public static List<string> defaultActions = new()
        {
            ActionAliases.ExampleAction,
            ActionAliases.ExampleActionPrewarm
        };
    }
}