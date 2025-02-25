using Actions.Abilities_Public;
using Modules.CharacterAbilitiesManager_Public;
using System.Collections;
using UnityEngine;

namespace Modules.CharacterAbilitiesManager
{
    public static class CompAbilityStatus
    {
        // *****************************
        // HasAbility 
        // *****************************
        public static bool HasAbility(State _state, string _actionAlias)
        {
            return _state.dynamic.abilities.ContainsKey(_actionAlias);
        }

        // *****************************
        // GetRunningAbilityStatus 
        // *****************************
        public static AbilityStatus GetRunningAbilityStatus(State _state)
        {
            return GetAbilityStatus(_state, _state.dynamic.runningAbilityName);
        }

        // *****************************
        // GetAbilityStatus 
        // *****************************
        public static AbilityStatus GetAbilityStatus(State _state, string _alias) {
            AbilityStatus result = AbilityStatus.None;

            if (HasAbility(_state, _alias))
            {
                var ability = _state.dynamic.abilities[_alias];
                result = ability.GetStatus();
            }

            return result;
        }
    }
}