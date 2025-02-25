using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using State         = Modules.ActionsManger.State;
using ActionsMgr    = Modules.ActionsManger.ActionsManager;
using Modules.ActionsManger;
using Unity.Collections;
using Zenject;
using GDTUtils.Patterns.Factory;

namespace Modules.ActionsManger
{
    public interface IActionInternal
    {
        IActionsStorage GetStorage();
        void Update(float _delta);
    }
}

/*

 Usage:
1) Add action via IActionManager.AddAction,
2) Get IAction in return.
3) Setup your IAction.
4) Start / Freeze / Interupt / trigger finish action
5) OnReset() will be called after action finished. And then OnActionFinished()
6) You can restart your action as long as you want. Don't need to setup every time - only before first startup.
7) If you don't need this action anymore call IActionManager.RemoveAction
8) If action is pooled: 
    Reset() and OnRemovePoolable() will be called. You must reset all external references here.
9) If action is not poolable:
    OnDisposed will be called. You must reset all external references here.
 
 */

namespace Modules.ActionsManger_Public
{
    // *****************************
    // IAction 
    // *****************************
    public interface IAction
    {
        /// <summary>
        /// Setup action data.
        /// </summary>
        /// <param name="_data"></param>
        void SetSharedData(ActionSharedDataBase _data);

        /// <summary>
        /// Start action and its update cycle.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops action from being updated.
        /// </summary>
        void Freeze(bool _val);

        /// <summary>
        /// Finishes action immediately at this frame.
        /// </summary>
        void Interrupt();

        /// <summary>
        /// Reset action state. Set values to default.
        /// </summary>
        void Reset();

        /// <summary>
        /// Start finishing sequence. Can be called both from inside and outside of Action.
        /// </summary>
        void TriggerFinishAction();
    }

    // *****************************
    // ActionSharedDataBase 
    // *****************************
    public abstract class ActionSharedDataBase
    {
    }

    // *****************************
    // ActionBase 
    // *****************************
    /// <summary>
    /// Base class for action.
    /// </summary>
    public abstract class ActionBase : GDTUtils.IPoolable, IFactoryProduct, IAction, IActionInternal
    {
        public int Id { get; set; }

        private IActionsStorage actionContainer;

        private bool isActive   = false;
        private bool IsFrozen   = false;
        private bool isDisposed = false;
        private bool isPristine = false; // if true - means that action was not yet modified, so dont need to reset it
        
        private bool isSetupPristine = false; // if true - means no setup was done y eat, so dont need to cleanup poolable object

        protected DiContainer container;
        private UpdateMode mode = UpdateMode.Regular;

        // *****************************
        // GetConfig 
        // *****************************
        protected T GetConfig<T>() where T : ConfigActionBase
        {
            return actionContainer.GetConfig<T>();
        }

        // *****************************
        // Setup 
        // *****************************
        public void Setup(State _state, IActionsStorage _container)
        {
            container       = _state.dynamic.moduleMgr.Container;
            actionContainer = _container;
            isPristine      = true;
        }

        // *****************************
        // GetStorage 
        // *****************************
        IActionsStorage IActionInternal.GetStorage()
        {
            return actionContainer;
        }

        // *****************************
        // SetSharedData 
        // *****************************
        void IAction.SetSharedData(ActionSharedDataBase _data)
        {
            isSetupPristine = false;
            OnSetupSharedData(_data);
        }

        // *****************************
        // OnSetupSharedData 
        // *****************************
        /// <summary>
        /// Call after 'IAction.Setup' call. You should place your data interpretation here.
        /// </summary>
        /// <param name="_data"></param>
        protected virtual void OnSetupSharedData(ActionSharedDataBase _data)
        {

        }

        // *****************************
        // Start 
        // *****************************
        void IAction.Start()
        {
            Debug.Assert(!isActive, "Action already started!");
            isActive    = true;
            isPristine  = false;
            OnActionStarted();

        }

        // *****************************
        // OnActionStarted 
        // *****************************
        /// <summary>
        /// Called when action started
        /// </summary>
        protected virtual void OnActionStarted()
        {

        }

        // *****************************
        // Freeze 
        // *****************************
        void IAction.Freeze(bool _val) {
            IsFrozen = _val;
            OnFrozen(_val);
        }

        // *****************************
        // OnFrozen 
        // *****************************
        /// <summary>
        /// Called when action just frozen / unfrozen
        /// </summary>
        /// <param name="_val"></param>
        protected virtual void OnFrozen(bool _val)
        {

        }

        // *****************************
        // Interrupt 
        // *****************************
        void IAction.Interrupt()
        {
            Debug.Assert(isActive, "Interrupt called on innactive action!");

            OnActionInterrupted();
            ReportFinished();
        }

        // *****************************
        // OnActionInterrupted 
        // *****************************
        /// <summary>
        /// Called when action is being interrupted. ReportFinished() will be automatically called right after this method. Interruption should finish action immediately in one frame.
        /// </summary>
        protected virtual void OnActionInterrupted() 
        {
        
        }

        // *****************************
        // TriggerFinishAction 
        // *****************************
        public void TriggerFinishAction()
        {
            Debug.Assert(isActive, "TriggerFinishAction called on innactive action!");
            Debug.Assert(mode == UpdateMode.Regular, "TriggerFinishAction called on action which is already being queued for finishing!");

            mode = UpdateMode.FinishingSequence;
            OnTriggerFinishAction();
        }

        // *****************************
        // OnTriggerFinishAction 
        // *****************************
        /// <summary>
        /// Called when action just started finish sequence. (!) Dont forget to call ReportFinished() when sequence is done (!)
        /// </summary>
        protected virtual void OnTriggerFinishAction()
        {

        }

        // *****************************
        // ReportFinished 
        // *****************************
        /// <summary>
        /// You must call this manually when action is finished.
        /// </summary>
        protected void ReportFinished()
        {
            Debug.Assert(isActive, "Finish called on innactive action!");

            isActive = false;
            OnActionFinished();
            (this as IAction).Reset();
        }

        // *****************************
        // OnActionFinished 
        // *****************************
        /// <summary>
        /// Triggered when ReportFinished called. Reset will be called shortly after that method.
        /// </summary>
        protected virtual void OnActionFinished()
        {

        }

        // *****************************
        // Update 
        // *****************************
        void IActionInternal.Update(float _delta)
        {

            bool ignore = !isActive || IsFrozen || isDisposed;
            if (ignore)
            {
                return;
            }

            OnUpdate(mode, _delta);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        protected virtual void OnUpdate(UpdateMode _mode, float _delta)
        {
        }

        // *****************************
        // Reset 
        // *****************************
        void IAction.Reset()
        {
            isActive    = false;
            IsFrozen    = false;
            mode        = UpdateMode.Regular;
            OnReset();
        }

        // *****************************
        // OnReset 
        // *****************************
        /// <summary>
        /// Called on action reset. Which happens after action is finished or slept in a pool. Purpose of this method is to reset a temporary data! Action may be called again right after Reset!
        /// </summary>
        protected virtual void OnReset()
        {

        }

        // *****************************
        // Dispose 
        // *****************************
        /// <summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            OnDispose();
            actionContainer = null;
            container       = null;
        }

        // *****************************
        // OnDispose 
        // *****************************
        /// <summary>
        /// Called when disposing action. This happens when ActionManager is being disposed or when non-poolable action is being removed. Called Once.
        /// </summary>
        protected virtual void OnDispose()
        {

        }

        // *****************************
        // OnAwakePoolable 
        // *****************************
        /// <summary>
        /// Called when poolalbe action was activated (i e "created")
        /// </summary>
        protected virtual void OnAwakePoolable()
        {

        }

        // *****************************
        // OnSleptPoolable 
        // *****************************
        /// <summary>
        /// Called when poolable action is being removed (slept in fact). Here you can nullify event subsriptions and off class references. So they will not preserve its value when action will be awaken next time.
        /// </summary>
        protected virtual void OnRemovePoolable()
        {

        }

        // IPoolable:

        // *****************************
        // Activate 
        // *****************************
        public void Activate()
        {
        }

        // *****************************
        // Deactivate 
        // *****************************
        public void Deactivate()
        {
            if (!isPristine)
            {
                isPristine = true;
                OnReset();
            }

            if (!isSetupPristine)
            {
                isSetupPristine = true;
                OnRemovePoolable();
            }
        }

        // *****************************
        // OnAdded 
        // *****************************
        public void OnAdded()
        {
        }

        // *****************************
        // ActionMode 
        // *****************************
        protected enum UpdateMode {
            Regular,
            FinishingSequence
        }
    }
}