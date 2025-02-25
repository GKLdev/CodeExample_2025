using Actions.Abilities_Public;
using Modules.ActionsManger_Public;
using Modules.CharacterController_Public;
using Modules.CharacterFacade_Public;
using Modules.CharacterVisualController_Public;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using State = Modules.CharacterAbilitiesManager.State;

namespace Actions.Abilities_Public
{
    // *****************************
    // IAbilityAction 
    // *****************************
    public interface IAbilityAction : IAction
    {
        void ResetCooldown();
        void GetCooldownDuration(out float total, out float remaining);
        AbilityStatus GetStatus();
        void OnAnimEvent(AnimEventType _eventType, int _param);
        void SetupData(AbilityConfigurationData _data);
    }

    // *****************************
    // AbilityStatus 
    // *****************************
    public enum AbilityStatus
    {
        None = 0,
        RunningAbility,
        FinishingAbility,
        AtCooldown
    }

    // *****************************
    // AbilityConfigurationData 
    // *****************************
    public class AbilityConfigurationData
    {
        public Vector3  startDirection;
        public bool     applyStartingDirection;
    }
}

namespace Actions.Abilities
{
    // *****************************
    // AbilityActionSharedData 
    // *****************************
    public class AbilityActionSharedData : ActionSharedDataBase
    {
        public readonly ICharacterFacade            owner;
        public readonly ICharacterController        controller;
        public readonly ICharacterVisualController  ownerVisual;
        public readonly ITimeManager                timeMgr;
        public readonly IDamageManager              damageMgr;
        public readonly ReferenceDbAliasesConfig    aliasConfig;
        public readonly State                       state;

        public AbilityActionSharedData(ICharacterFacade _owner, ITimeManager _timeMgr, IDamageManager _damageMgr, ReferenceDbAliasesConfig _aliasConfig, State _state)
        {
            owner       = _owner;
            controller  = owner.P_Controller; 
            ownerVisual = controller.GetVisualController();
            timeMgr     = _timeMgr;
            damageMgr   = _damageMgr;
            aliasConfig = _aliasConfig;
            state       = _state;
        }
    }

    public class AbilityActionBase : ActionBase, IAbilityAction
    {
        protected AbilityActionSharedData   data;
        protected IDamageDispatcher         dispatcher;
        protected DamageDispatcherData      dispatcherData = new();

        /// <summary>
        /// Destroy default dispatcher on Reset. Set this true if you dont care about dispatcher callbacks. Will still be destroyed on interrupt.
        /// </summary>
        protected bool doNotDestroyDispatcherOnDisable = false;

        protected bool animationFinished    = false;
        protected bool dispatcherFinished   = false;
       
        private AbilityStatus               status;
        private     int                     mainCooldownId = -1; // -1 means ability with no cooldown

        private System.Action onDispatcherCallback;

        // configuration data

        protected AbilityConfigurationData abilityConfigData = null;
        public bool isInterrupting = false;

        // *****************************
        // AbilityActionBase 
        // *****************************
        public AbilityActionBase()
        {
            onDispatcherCallback = OnDispatcherFinished;
        }

        // *****************************
        // SetupData 
        // *****************************
        public void SetupData(AbilityConfigurationData _data)
        {
            abilityConfigData = _data;

            if (_data == null)
            {
                return;
            }
        }

        // *****************************
        // OnSetupSharedData 
        // *****************************
        protected override void OnSetupSharedData(ActionSharedDataBase _data)
        {
            base.OnSetupSharedData(_data);

            data = (_data as AbilityActionSharedData);
            dispatcherData.onDispatcherFinished = onDispatcherCallback;
        }

        // *****************************
        // OnDispatcherFinished 
        // *****************************
        protected virtual void OnDispatcherFinished()
        {
            bool isRunning = status == AbilityStatus.RunningAbility;
            if (!isRunning)
            {
                return;
            }

            dispatcherFinished = true;
        }

        // *****************************
        // OnActionStarted 
        // *****************************
        protected override void OnActionStarted()
        {
            dispatcherData.Reset();
            status = AbilityStatus.RunningAbility;
        }

        // *****************************
        // OnTriggerFinishAction 
        // *****************************
        protected override void OnTriggerFinishAction()
        {
            status = AbilityStatus.FinishingAbility;
        }

        // *****************************
        // OnAnimEvent 
        // *****************************
        public virtual void OnAnimEvent(AnimEventType _eventType, int _param)
        {
            bool alreadyFinished = status != AbilityStatus.RunningAbility;
            if (alreadyFinished)
            {
                Debug.Assert(false, $"OnDealDamageEvent called while ability={this} is not running or finished!");
                return;
            }

            switch (_eventType)
            {
                case AnimEventType.Finished:
                    OnAnimationFinishedEvent(_param);
                    break;
                case AnimEventType.DealDamage:
                    OnDealDamageEvent(_param);
                    break;
                case AnimEventType.SpawnFX:
                    break;
                case AnimEventType.DespawnFX:
                    break;
                default:
                    break;
            }
        }

        // *****************************
        // OnDealDamageEvent 
        // *****************************
        protected virtual void OnDealDamageEvent(int _param)
        {

        }

        // *****************************
        // OnAnimationFinishedEvent 
        // *****************************
        protected virtual void OnAnimationFinishedEvent(int _param)
        {
            animationFinished = true;
        }

        // *****************************
        // OnActionFinished 
        // *****************************
        protected override void OnActionFinished()
        {
            bool abilityWithCooldown = mainCooldownId >= 0;
            if (abilityWithCooldown)
            {
                status =  AbilityStatus.AtCooldown;
            }
            else
            {
                status = AbilityStatus.None;
            }

            data.state.dynamic.runningAbilityName = null;
        }

        // *****************************
        // OnActionInterrupted 
        // *****************************
        protected override void OnActionInterrupted()
        {
            base.OnActionInterrupted();

            isInterrupting = true;

            ToggleDispatcher(ref dispatcher, false);
            OnAbilityInterrupted();

            isInterrupting = false;
        }


        // *****************************
        // OnAbilityInterrupted 
        // *****************************
        /// <summary>
        /// Place ability interruption logic here
        /// </summary>
        protected virtual void OnAbilityInterrupted()
        {

        }

        // *****************************
        // GetCooldownDuration 
        // *****************************
        public void GetCooldownDuration(out float total, out float remaining)
        {
            throw new System.NotImplementedException();
        }

        // *****************************
        // GetStatus 
        // *****************************
        public AbilityStatus GetStatus()
        {
            return status;
        }

        // *****************************
        // OnAnimEvent 
        // *****************************
        public void ResetCooldown()
        {   
            if (status != AbilityStatus.AtCooldown)
            {
                return;
            }

            status = AbilityStatus.None;
        }

        // *****************************
        // OnRemovePoolable 
        // *****************************
        protected override void OnRemovePoolable()
        {
            OnCleanup();
        }


        // *****************************
        // OnDispose 
        // *****************************
        protected override void OnDispose()
        {
            OnCleanup();
        }

        // *****************************
        // OnCleanup 
        // *****************************
        /// <summary>
        /// Here you can break references and unsuscribe all events. Triggered when ability is being slept at pool or removed completely.
        /// </summary>
        protected virtual void OnCleanup()
        {
            if (data == null)
            {
                return;
            }

            dispatcherData.onDispatcherFinished = null;
            data = null;

            ToggleDispatcher(ref dispatcher, false);
            dispatcherData.Reset();
        }

        // *****************************
        // ToggleDispatcher 
        // *****************************
        /// <summary>
        /// Create or remove dispatcher
        /// </summary>
        protected void ToggleDispatcher(ref IDamageDispatcher _dispatcher, bool _create, CATEGORY_DAMAGEDISPATCHERS _dispatcherType = default)
        {
            if (_create)
            {
                if (_dispatcher == null)
                {
                    dispatcher = data.damageMgr.CreateDispatcher(_dispatcherType);
                }
            }
            else
            {
                if (_dispatcher != null)
                {
                    if (isInterrupting || !doNotDestroyDispatcherOnDisable)
                    {
                        // allowed to be destroyed
                        data.damageMgr.RemoveDispatcher(dispatcher);
                    }

                    dispatcher = null;
                }
            }
        }

        // *****************************
        // OnReset 
        // *****************************
        protected override void OnReset()
        {
            base.OnReset();

            dispatcherFinished  = false;
            animationFinished   = false;

            ToggleDispatcher(ref dispatcher, false);
        }

        // *****************************
        // CheckAnimationAndDispatcherFInished 
        // *****************************
        protected bool CheckAnimationAndDispatcherFinished()
        {
            return animationFinished && dispatcherFinished;
        }

        // *****************************
        // TryFinishDispatcherBasedAbility 
        // *****************************
        protected bool TryFinishDispatcherBasedAbility(bool _waitForDispatcher = true)
        {
            if (!_waitForDispatcher)
            {
                ReportFinished();
                return true;
            }

            bool isDone = CheckAnimationAndDispatcherFinished();

            if (isDone)
            {
                ReportFinished();
            }

            return isDone;
        }
    }
}