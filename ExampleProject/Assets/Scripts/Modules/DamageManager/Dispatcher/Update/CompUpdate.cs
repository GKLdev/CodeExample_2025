using GDTUtils;
using Modules.DamageDispatcher_Public;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Modules.DamageDispatcher
{
    public static class CompUpdate
    {
        // *****************************
        // OnUpdate
        // *****************************
        public static void OnUpdate(State _state)
        {
            bool ignore = _state.dynamic.stage == DispatchStage.Innactive || _state.dynamic.data == null;
            if (ignore)
            {
                return;
            }

            GetDeltaTime(_state);
            ApplyScale(_state);
            ApplyRotation(_state);
            LookDirection(_state);
            FollowTarget(_state);

            ProcessCharging(_state);
            ProcessAwaitingForTick(_state);

            bool readyForDispatch = _state.dynamic.stage == DispatchStage.ReadyForDispatch;
            if (readyForDispatch)
            {
                DispatchDamage(_state);
                TryAdvanceTick(_state);
            }

            ResetFrameData(_state);
            TryFinish(_state);
        }

        // *****************************
        // ApplyData
        // *****************************
        public static void ApplyData(State _state, DamageDispatcherData _data)
        {
            _data.CopyTo(_state.dynamic.data);

            bool needCharge = _state.config.HasChargeTime;
            _state.dynamic.stage = needCharge ? DispatchStage.Charging : DispatchStage.ReadyForDispatch;

            if (_state.dynamic.chargeCooldown > 0)
            {
                _state.dynamic.timeMgr.PauseCooldown(_state.dynamic.chargeCooldown);
            }

            if (_state.dynamic.tickCooldown > 0)
            {
                _state.dynamic.timeMgr.PauseCooldown(_state.dynamic.tickCooldown);
            }

            _state.dynamic.ticksCount = 0;

            _state.root.position    = _data.position;
            _state.root.rotation = Quaternion.LookRotation(_data.orientation.normalized,Vector3.up);

            ApplyScale(_state);
            FollowTarget(_state);
        }

        // *****************************
        // StopDispatcher
        // *****************************
        public static void StopDispatcher(State _state)
        {
            bool isDpsMode = _state.config.DealDamagePerSecond;
            if (!isDpsMode)
            {
                Debug.LogError("StopDispatcher cant be called on non damage-per-second dispatcher!");
                return;
            }

            MarkReadyToFinish(_state);

            // report to visual
            if (_state.visualizer.Value == null)
            {
                TryFinish(_state);
                return;
            }

            _state.visualizer.Value.OrderStopLoop();

        }

        // *****************************
        // ResetFrameData
        // *****************************
        static void ResetFrameData(State _state)
        {
        }

        // *****************************
        // ProcessCharging
        // *****************************
        static void ProcessCharging(State _state)
        {
            bool needToCharge = _state.dynamic.stage == DispatchStage.Charging;
            if (!needToCharge)
            {
                return;
            }

            bool cdRunning  = _state.dynamic.timeMgr.CheckCooldownIsRunning(_state.dynamic.chargeCooldown);
            bool cdPassed   = _state.dynamic.timeMgr.CheckCooldownPassed(_state.dynamic.chargeCooldown);
            if (!cdRunning && !cdPassed)
            {
                if (_state.debug)
                {
                    Debug.Log("Charging started...");
                }

                _state.dynamic.timeMgr.ResetCooldown(_state.dynamic.chargeCooldown);

                TryStartVisualization(_state, _state.config.ChargingTime);
            }

            if (!cdRunning && cdPassed)
            {
                if (_state.debug)
                {
                    Debug.Log("Charging finished!");
                }

                _state.dynamic.timeMgr.StopCooldown(_state.dynamic.chargeCooldown);
                _state.dynamic.stage = DispatchStage.ReadyForDispatch;
            }
        }

        // *****************************
        // IsDPSMode
        // *****************************
        static bool IsDPSMode(State _state)
        {
            return _state.config.IsPeriodic && _state.config.DealDamagePerSecond;
        }

        // *****************************
        // ProcessAwaitingForTick
        // *****************************
        static void ProcessAwaitingForTick(State _state)
        {
            if (IsDPSMode(_state))
            {
                return;
            }

            bool awatingForTick = _state.dynamic.stage == DispatchStage.AwaitingForTick;
            if (!awatingForTick) {
                return;
            }

            bool cdRunning  = _state.dynamic.timeMgr.CheckCooldownIsRunning(_state.dynamic.tickCooldown);
            bool cdPassed   = _state.dynamic.timeMgr.CheckCooldownPassed(_state.dynamic.tickCooldown);
            if (!cdRunning && !cdPassed)
            {
                if (_state.debug)
                {
                    Debug.Log("Tick started...");
                }

                _state.dynamic.timeMgr.ResetCooldown(_state.dynamic.tickCooldown);
                TryStartVisualization(_state, _state.config.TickDelay);
            }

            if (!cdRunning && cdPassed)
            {
                if (_state.debug)
                {
                    Debug.Log("Tick passed!");
                }

                _state.dynamic.timeMgr.StopCooldown(_state.dynamic.tickCooldown);
                _state.dynamic.stage = DispatchStage.ReadyForDispatch;
            }
        }

        // *****************************
        // TryStartVisualization
        // *****************************
        static void TryStartVisualization(State _state, float _tickDuration)
        {
            if (_state.visualizer.Value == null)
            {
                return;
            }

            _state.dynamic.visualizationData.ChargingOrTickDuration = _tickDuration;
            _state.dynamic.visualizerReportedFinished               = false;
            
            bool isDpsMode = IsDPSMode(_state);
            _state.visualizer.Value.StartVisualizer(isDpsMode);
        }

        // *****************************
        // DispatchDamage
        // *****************************
        static void DispatchDamage(State _state)
        {
            if (_state.debug)
            {
                Debug.Log("Dispatching damage...");
            }

            if (_state.dynamic.data.source == null)
            {
                Debug.Assert(false, "DispatchDamage: Damage source is Null!");
                return;
            }

            float damage = IsDPSMode(_state) ? _state.dynamic.data.damageValue  * _state.dynamic.deltaTime : _state.dynamic.data.damageValue;

            foreach (var target in _state.dynamic.registeredTargets)
            {
                bool applyRestrictions = _state.dynamic.data.restriction != FactionRestriction.DamageEverything;
                if (applyRestrictions)
                {
                    bool sameFaction        = _state.dynamic.data.source.faction    == target.GetFaction();
                    bool restrictionsPassed = _state.dynamic.data.restriction       == FactionRestriction.DamageEverythingButCasterFaction && !sameFaction;

                    if (!restrictionsPassed)
                    {
                        continue;
                    }
                }

                target.OnDamage(_state.dynamic.data.source, _state.dynamic.data.type, damage);
            }
        }

        // *****************************
        // TryAdvanceTick
        // *****************************
        static void TryAdvanceTick(State _state)
        {
            if (IsDPSMode(_state))
            {
                _state.dynamic.stage = DispatchStage.ReadyForDispatch;
                return;
            }

            bool startnewTick = _state.config.IsPeriodic && _state.dynamic.ticksCount < _state.config.TicksCount;
            if (startnewTick)
            {
                _state.dynamic.ticksCount++;
                _state.dynamic.stage = DispatchStage.AwaitingForTick;
                ProcessAwaitingForTick(_state); // start new cooldown right away
            }
            else
            {
                // finish dispatcher
                _state.dynamic.ticksCount   = 0;

                MarkReadyToFinish(_state);
            }
        }

        // *****************************
        // MarkReadyToFinish
        // *****************************
        static void MarkReadyToFinish(State _state)
        {
            bool visualizerExists   = _state.visualizer.Value != null;
            bool wantFinishNow      = !visualizerExists || _state.dynamic.visualizerReportedFinished;

            _state.dynamic.stage = wantFinishNow ? DispatchStage.Innactive : DispatchStage.AwaitingForVisualizer;
        }

        // *****************************
        // ApplyRotation
        // *****************************
        static void ApplyRotation(State _state)
        {
            if (!_state.config.IsRotating)
            {
                return;
            }

            // TODO: support lookat targeting mb?
            _state.root.rotation *= Quaternion.AngleAxis(_state.config.RotationSpeed * _state.dynamic.deltaTime, Vector3.up);
        }

        // *****************************
        // FollowTarget
        // *****************************
        static void FollowTarget(State _state)
        {
            if (_state.dynamic.data.followTarget == null)
            {
                return;
            }

            _state.root.position = _state.dynamic.data.followTarget.position;
        }

        // *****************************
        // LookDirection
        // *****************************
        static void LookDirection(State _state)
        {
            if (!_state.config.IsRotating)
            {
                _state.root.rotation = Quaternion.LookRotation(_state.dynamic.data.orientation.normalized, Vector3.up);
            }
        }

        // *****************************
        // FollowTarget
        // *****************************
        static void ApplyScale(State _state)
        {
            _state.triggerRoot.localScale = new Vector3(
                ApplyScale(_state.config.ScaleAxises.x, _state.dynamic.data.scale.x, _state.dynamic.defaultScale.x),
                ApplyScale(_state.config.ScaleAxises.y, 1f, _state.dynamic.defaultScale.y),
                ApplyScale(_state.config.ScaleAxises.z, _state.dynamic.data.scale.y, _state.dynamic.defaultScale.z)
                );

            float ApplyScale(float _axis, float _scale, float _default)
            {
                return Mathf.Clamp(GDTMath.MoreNotEqual(_axis, 0f) ? _scale : _default, 0f, float.MaxValue);
            }
        }

        // *****************************
        // GetDeltaTime
        // *****************************
        static void GetDeltaTime(State _state)
        {
            _state.dynamic.deltaTime = _state.dynamic.timeMgr.GetDeltaTime(_state.dynamic.timeLayer);
        }

        // *****************************
        // TryFinish
        // *****************************
        static void TryFinish(State _state)
        {
            bool finish = _state.dynamic.stage == DispatchStage.Innactive;
            if (!finish)
            {
                return;
            }

            bool autoDestroy = _state.config.DestroyOnFinished;
            if (autoDestroy)
            {
                if (_state.debug)
                {
                    Debug.Log("Auto destroying...");
                }

                var callback = _state.dynamic.data.onDispatcherFinished;
                _state.dynamic.damageMgr.RemoveDispatcher(_state.dynamic.self);
                ReportFinished(_state, callback);
                return;
            }

            ReportFinished(_state);

            void ReportFinished(State _state, System.Action _customCallback = null)
            {
                if (_state.debug)
                {
                    Debug.Log("Reporting dispatcher finished...");
                }

                _state.dynamic.visualizerReportedFinished = false;

                if (_customCallback != null)
                {
                    _customCallback.Invoke();
                }
                else
                {
                    if (_state.dynamic.data.makeCallbackOnFinish)
                    {
                        _state.dynamic.data?.onDispatcherFinished?.Invoke();
                    }
                }
            }
        }

        // *****************************
        // OnVisualizerFinished
        // *****************************
        public static void OnVisualizerFinished(State _state)
        {
            bool error = _state.dynamic.stage == DispatchStage.Innactive;
            if (error)
            {
                Debug.Assert(false, "OnVisualizerFinished called on innactive disparcher!");
                return;
            }

            _state.dynamic.visualizerReportedFinished = true;

            if (_state.dynamic.stage == DispatchStage.AwaitingForVisualizer)
            {
                _state.dynamic.stage = DispatchStage.Innactive;
                TryFinish(_state);
            }
        }

        // *****************************
        // UpdateVisualizer
        // *****************************
        public static void UpdateVisualizer(State _state)
        {
            _state.visualizer.Value?.OnUpdate(_state.dynamic.deltaTime);
        }
    }
}