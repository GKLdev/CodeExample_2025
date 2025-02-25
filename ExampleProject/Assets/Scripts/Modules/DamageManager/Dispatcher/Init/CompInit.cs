using Modules.DamageManager_Public;
using Modules.ModuleManager_Public;
using Modules.TimeManager_Public;
using System.Collections;
using UnityEngine;

namespace Modules.DamageDispatcher
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr)
        {
            // dependencies
            _state.dynamic.damageMgr    = _moduleMgr.Container.Resolve<IDamageManager>();
            _state.dynamic.timeMgr      = _moduleMgr.Container.Resolve<ITimeManager>();
            
            // config
            _state.dynamic.timeLayer    = _state.config.TimeLayer;

            // cooldowns
            if (_state.config.HasChargeTime)
            {
                _state.dynamic.chargeCooldown = _state.dynamic.timeMgr.AddCooldown(_state.config.ChargingTime, _state.dynamic.timeLayer);
            }

            if (_state.config.IsPeriodic)
            {
                _state.dynamic.tickCooldown = _state.dynamic.timeMgr.AddCooldown(_state.config.TickDelay, _state.dynamic.timeLayer);
            }

            _state.dynamic.defaultScale = _state.triggerRoot.localScale;

            _state.visualizer.Value?.InitModule();

            // finish
            _state.initialized = true;
        }

        // *****************************
        // Dispose
        // *****************************
        public static void Dispose(State _state)
        { 
            Reset(_state);
        }

        // *****************************
        // Reset
        // *****************************
        public static void Reset(State _state)
        {
            //_state.dynamic.data                 = null;
            _state.dynamic.stage                = DispatchStage.Innactive;

            if (_state.dynamic.chargeCooldown > 0)
            {
                _state.dynamic.timeMgr.PauseCooldown(_state.dynamic.chargeCooldown);
            }

            if (_state.dynamic.tickCooldown > 0)
            {
                _state.dynamic.timeMgr.PauseCooldown(_state.dynamic.tickCooldown);
            }

            _state.dynamic.registeredTargets.Clear();
            _state.dynamic.ticksCount = 0;

            _state.dynamic.visualizerReportedFinished   = false;
        }
    }
}