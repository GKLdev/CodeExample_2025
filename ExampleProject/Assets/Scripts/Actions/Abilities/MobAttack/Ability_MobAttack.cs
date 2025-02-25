using Modules.ActionsManger_Public;
using Modules.DamageDispatcher_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.Abilities
{
    public class Action_MobAttack : AbilityActionBase
    {
        ConfigAbility_MobAttack cfg;

        bool waitForDispatcher = false;

        // *****************************
        // OnSetupSharedData 
        // *****************************
        protected override void OnSetupSharedData(ActionSharedDataBase _data)
        {
            base.OnSetupSharedData(_data);
            
            cfg = GetConfig<ConfigAbility_MobAttack>();
            doNotDestroyDispatcherOnDisable = !waitForDispatcher;
        }

        // *****************************
        // OnActionStarted 
        // *****************************
        protected override void OnActionStarted()
        {
            base.OnActionStarted();

            LibAbilityActions.PlayBlockingAnimation(data, cfg.animation);
            LibAbilityActions.ForceCharacterOrientation(data.controller, abilityConfigData);

            FillDispatcherData();
            ToggleDispatcher(ref dispatcher, true, LibAbilityActions.GetDispatcherFromAlias(data.aliasConfig, cfg.DispatcherAlias));
            dispatcher.StartDispatcher(dispatcherData);
        }

        // *****************************
        // FillDispatcherData 
        // *****************************
        void FillDispatcherData()
        {
            LibAbilityActions.FillDispatcherData(
                dispatcherData,
                data.owner.GetDamageSource(),
                cfg.FactionRestriction,
                data.controller.P_Position,
                data.controller.P_Orientation,
                cfg.DispatcherScale,
                cfg.DamageType,
                cfg.FollowCaster ? data.owner.P_Controller.GetVisualController().P_GameObjectAccess.transform : null,
                cfg.BaseDamage,
                waitForDispatcher);
        }

        // *****************************
        // OnAbilityInterrupted 
        // *****************************
        protected override void OnAbilityInterrupted()
        {
            base.OnAbilityInterrupted();

            LibAbilityActions.ForceStopAnimation(data);

            // if i will need stagger animation (like at Lost Ark) - should just call stagger action after finishing this one!
        }

        // *****************************
        // OnDealDamageEvent 
        // *****************************
        protected override void OnDealDamageEvent(int _param)
        {
            base.OnDealDamageEvent(_param);

        }

        // *****************************
        // OnAnimationFinishedEvent 
        // *****************************
        protected override void OnAnimationFinishedEvent(int _param)
        {
            base.OnAnimationFinishedEvent(_param);

            LibAbilityActions.ForceStopAnimation(data);

            TryFinishDispatcherBasedAbility(waitForDispatcher);
        }

        // *****************************
        // OnDispatcherFinished 
        // *****************************
        protected override void OnDispatcherFinished()
        {
            base.OnDispatcherFinished();

            TryFinishDispatcherBasedAbility();
        }

        // *****************************
        // OnActionFinished 
        // *****************************
        protected override void OnActionFinished()
        {
            base.OnActionFinished();
            LibAbilityActions.OnBlockingAnimEnd(data);
        }

        // *****************************
        // OnReset 
        // *****************************
        protected override void OnReset()
        {
            base.OnReset();
        }
    }
}