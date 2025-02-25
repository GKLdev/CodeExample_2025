using Actions.Abilities_Public;
using Modules.CharacterVisualController_Public;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;

namespace Modules.CharacterAbilitiesManager
{
    public static class CompAbilityManagement
    {
        // *****************************
        // StartAbility
        // *****************************
        public static void StartAbility(State _state, string _alias, AbilityConfigurationData _data)
        {
            bool hasAbility = CompAbilityStatus.HasAbility(_state, _alias);
            if (!hasAbility)
            {
                CompInit.AddAbility(_state, _alias);
            }

            TryInterruptAbility(_state);

            _state.dynamic.runningAbilityName = _alias;
            
            var ability =_state.dynamic.abilities[_state.dynamic.runningAbilityName];

            ability.SetupData(_data);
            ability.Start();
        }

        // *****************************
        // TryInterruptAbility
        // *****************************
        public static void TryInterruptAbility(State _state)
        {
            bool isRunningAbility = _state.dynamic.runningAbilityName != null;
            if (!isRunningAbility)
            {
                return;
            }

            var ability = _state.dynamic.abilities[_state.dynamic.runningAbilityName];
            ability.Interrupt();
            _state.dynamic.runningAbilityName = null;
        }

        // *****************************
        // TriggerFinishAbility
        // *****************************
        public static void TriggerFinishAbility(State _state)
        {
            bool hasAbility = _state.dynamic.runningAbilityName == null;
            if (!hasAbility)
            {
                Debug.Assert(false, "Tried to finish ability, but no ability is currently running!");
                return;
            }

            var ability = _state.dynamic.abilities[_state.dynamic.runningAbilityName];
            ability.TriggerFinishAction();
        }

        // *****************************
        // ResetCooldown
        // *****************************
        public static void ResetCooldown(State _state, string _alias = null)
        {
            string ability = _alias;

            bool runningAbility = _alias == null;
            if (runningAbility)
            {
                ability = _state.dynamic.runningAbilityName;
            }

            var  status = CompAbilityStatus.GetRunningAbilityStatus(_state);
            bool atCd   = status == Actions.Abilities_Public.AbilityStatus.AtCooldown;
            if (atCd)
            {
                _state.dynamic.abilities[ability].ResetCooldown();
            }
        }

        // *****************************
        // GetCooldownInfo
        // *****************************
        public static void GetCooldownInfo(State _state, out float total, out float remaining, string _alias = null)
        {
            total       = -1;
            remaining   = -1;

            string abilityName = _alias;

            bool runningAbility = _alias == null;
            if (runningAbility)
            {
                abilityName = _state.dynamic.runningAbilityName;
            }

            bool hasAbility = CompAbilityStatus.HasAbility(_state, _alias);
            if (!hasAbility)
            {
                return;
            }

            var ability = _state.dynamic.abilities[_state.dynamic.runningAbilityName];
            ability.GetCooldownDuration(out total, out remaining);
        }

        // *****************************
        // OnAnimEvent
        // *****************************
        public static void OnAnimEvent(State _state, AnimEventType _eventType, int _param)
        {
            bool runningAbility = _state.dynamic.runningAbilityName != null;
            if (!runningAbility)
            {
                return;
            }

            _state.dynamic.abilities[_state.dynamic.runningAbilityName].OnAnimEvent(_eventType, _param);
        }
    }
}