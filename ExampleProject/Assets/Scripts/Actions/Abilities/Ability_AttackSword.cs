using Modules.ActionsManger_Public;
using Modules.CharacterVisualController_Public;

namespace Actions.Abilities
{
    public class Action_AttackSword : AbilityActionBase
    {
        ConfigAbility_AttackSword cfg;

        // *****************************
        // OnSetupData 
        // *****************************
        protected override void OnSetupSharedData(ActionSharedDataBase _data)
        {
            base.OnSetupSharedData(_data);

            cfg = GetConfig<ConfigAbility_AttackSword>();
        }

        // *****************************
        // OnActionStarted 
        // *****************************
        protected override void OnActionStarted()
        {
            base.OnActionStarted();

            LibAbilityActions.PlayBlockingAnimation(data, AnimationType.SwordAttack);
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
            ReportFinished();
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
        // OnCleanup 
        // *****************************
        protected override void OnCleanup()
        {
            base.OnCleanup();
        }
    }
}