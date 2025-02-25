using Actions.Abilities_Public;
using GDTUtils;
using GDTUtils.Animation;
using Modules.ActionsManger_Public;
using Modules.CharacterAbilitiesManager_Public;
using Modules.CharacterFacade_Public;
using Modules.CharacterVisualController;
using Modules.CharacterVisualController_Public;
using Modules.DamageManager_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.CharacterAbilitiesManager
{
    /// <summary>
    /// Purpose: Start/Stop/Interrupt character abilities, imple,enmtes ad Actions.
    /// </summary>
    /// 
    /*   
    Abilities are supposed to be created once at CharacterFacade and all its components. Abilities should not be slept at actions pool while character exists.
    */
    public class CharacterAbilitiesManager : LogicBase, ICharacterAbilitiesManager
    {
        [SerializeField]
        State state;

        [Inject]
        IModuleManager moduleMgr;

        // *****************************
        // InitModule 
        // *****************************
        public void InitModule()
        {
            CompInit.Init(state, moduleMgr);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            if (!state.initialized)
            {
                return;
            }
        }

        // *****************************
        // StartAbility 
        // *****************************
        public void StartAbility(string _actionAlias, AbilityConfigurationData _data = null)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAbilityManagement.StartAbility(state, _actionAlias, _data);
        }

        // *****************************
        // InterruptAbility 
        // *****************************
        public void InterruptRunningAbility()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAbilityManagement.TryInterruptAbility(state);
        }

        // *****************************
        // TriggerFinishAbility 
        // *****************************
        public void TriggerFinishRunningAbility()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAbilityManagement.TriggerFinishAbility(state);
        }

        // *****************************
        // ResetCooldown 
        // *****************************
        public void ResetCooldown(string _actionAlias = null)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAbilityManagement.ResetCooldown(state, _actionAlias);
        }

        // *****************************
        // GetCooldownInfo 
        // *****************************
        public void GetCooldownInfo(out float total, out float remaining, string _alias = null)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            CompAbilityManagement.GetCooldownInfo(state, out total, out remaining, _alias = null);
        }

        // *****************************
        // HasAbility 
        // *****************************
        public bool HasAbility(string _actionAlias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return CompAbilityStatus.HasAbility(state, _actionAlias);
        }

        // *****************************
        // GetStatus 
        // *****************************
        public AbilityStatus GetAbilityStatus(string actionAlias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return CompAbilityStatus.GetRunningAbilityStatus(state);
        }

        // *****************************
        // GetRunningAbilityStatus 
        // *****************************
        public AbilityStatus GetRunningAbilityStatus(out string actionAlias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);

            actionAlias = state.dynamic.runningAbilityName;
            return CompAbilityStatus.GetRunningAbilityStatus(state);
        }

        // *****************************
        // IsRunningAbility 
        // *****************************
        public bool IsRunningAbility()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return state.dynamic.runningAbilityName != null;
        }

        // *****************************
        // CanRunAbility 
        // *****************************
        public bool CanRunAbility(string actionAlias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);

            var status = CompAbilityStatus.GetAbilityStatus(state, actionAlias);
            return status != AbilityStatus.AtCooldown && state.dynamic.runningAbilityName == null;
        }

        // *****************************
        // OnAdded 
        // *****************************
        public void OnAdded()
        {
        }

        // *****************************
        // OnAwake 
        // *****************************
        public void OnAwake()
        {
        }

        // *****************************
        // OnSlept 
        // *****************************
        public void OnSlept()
        {
            // TODO: reset all cooldowns
            CompInit.Reset(state);
        }

        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        [HideInInspector]
        public bool initialized = false;

        public ConfigCharacterAbilities                 config;
        public SerializedInterface<ICharacterFacade>    facade;

        public DynamicData dynamic = new();

        // *****************************
        // DynamicData
        // *****************************
        //[System.Serializable]
        public class DynamicData
        {
            public IActionsManager                  actionsMgr;
            public ITimeManager                     timeMgr;
            public IDamageManager                   damageMgr;
            public ReferenceDbAliasesConfig         referenceConfig;

            public Dictionary<string, IAbilityAction>           abilities = new();
            public Actions.Abilities.AbilityActionSharedData    abilitiesData;

            public string runningAbilityName = null;

            public System.Action<AnimEventType, int> animEventCallback;
        }
    }
}