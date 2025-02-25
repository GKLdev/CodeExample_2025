using Actions.Abilities_Public;
using Modules.CharacterFacade_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterAbilitiesManager_Public
{
    public interface ICharacterAbilitiesManager : IModuleInit, IModuleUpdate, ICharacterFacadeCallbacks
    {
        /// <summary>
        /// Starts ability using action id. Any currently running ability will be interrupted.
        /// </summary>
        /// <param name="_actionAlias"></param>
        void StartAbility(string _actionAlias, AbilityConfigurationData _data = null);

        /// <summary>
        /// Interrupts current ability.
        /// </summary>
        /// <param name="_actionAlias"></param>
        void InterruptRunningAbility();

        void ResetCooldown(string _actionAlias = null);
        void GetCooldownInfo(out float total, out float remaining, string _alias = null);

        void TriggerFinishRunningAbility();

        /// <summary>
        /// Check if unit has ability.
        /// </summary>
        bool HasAbility(string _actionAlias);

        /// <summary>
        /// Returns status of given runing ability. None if not running or foesnt have such ability.
        /// </summary>
        AbilityStatus GetAbilityStatus(string actionAlias);

        /// <summary>
        /// Returns status of running ability and its name.
        /// </summary>
        AbilityStatus GetRunningAbilityStatus(out string actionAlias);

        /// <summary>
        /// Checks if active avility is currently running.
        /// </summary>
        bool IsRunningAbility();

        /// <summary>
        /// Checks if ability can be started - manager is not running any other ability and desired ability is not at cooldown.
        /// </summary>
        /// <param name="actionAlias"></param>
        /// <returns></returns>
        bool CanRunAbility(string actionAlias);
    }
}