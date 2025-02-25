using Actions.Abilities_Public;
using Modules.ActionsManger_Public;
using Modules.CharacterVisualController_Public;
using Modules.DamageManager_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using UnityEngine;

namespace Modules.CharacterAbilitiesManager
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr)
        {
            if (_state.initialized)
            {
                return;
            }

            Debug.Assert(_state.facade.Value != null, $"Facade field must be defined!");

            _state.dynamic.actionsMgr           = _moduleMgr.Container.Resolve<IActionsManager>();
            _state.dynamic.timeMgr              = _moduleMgr.Container.Resolve<ITimeManager>();
            _state.dynamic.damageMgr            = _moduleMgr.Container.Resolve<IDamageManager>();
            _state.dynamic.referenceConfig      = _moduleMgr.Container.Resolve<ReferenceDbAliasesConfig>();

            _state.dynamic.abilitiesData = new(_state.facade.Value, _state.dynamic.timeMgr, _state.dynamic.damageMgr, _state.dynamic.referenceConfig, _state);
            CreateAllAbilitiesFromConfig(_state);

            // animation event callback
            var visual                          = _state.facade.Value.P_Controller.GetVisualController();
            _state.dynamic.animEventCallback    = (eventType, param) => { CompAbilityManagement.OnAnimEvent(_state, eventType, param); };
            visual.OnAnimationFinished          += _state.dynamic.animEventCallback;

            _state.initialized = true;
        }

        // *****************************
        // _alias
        // *****************************
        public static void AddAbility(State _state, string _alias)
        {
            Debug.Assert(!_state.dynamic.abilities.ContainsKey(_alias), $"Trying to add duplicate ability={_alias}");

            IAction abilityAction = _state.dynamic.actionsMgr.AddAction(_alias); // cast to IAbilityAction : IAction
            IAbilityAction casted = abilityAction as IAbilityAction;

            Debug.Assert(casted != null, $"Action={_alias} is not an ability!");

            casted.SetSharedData(_state.dynamic.abilitiesData);
            _state.dynamic.abilities.Add(_alias, casted);
        }


        // *****************************
        // CreateAllAbilitiesFromConfig
        // *****************************
        public static void CreateAllAbilitiesFromConfig(State _state)
        {
            foreach (var item in _state.config.Abilities)
            {
                AddAbility(_state, item.Alias);
            }
        }

        // *****************************
        // DisposeAbilities
        // *****************************
        public static void DisposeAllAbilities(State _state)
        {
            foreach (var item in _state.dynamic.abilities)
            {
                _state.dynamic.actionsMgr.RemoveAction(item.Value);
            }

            _state.dynamic.abilities.Clear();
        }

        // *****************************
        // Reset
        // *****************************
        public static void Reset(State _state)
        {
            CompAbilityManagement.TryInterruptAbility(_state);
        }
    }
}