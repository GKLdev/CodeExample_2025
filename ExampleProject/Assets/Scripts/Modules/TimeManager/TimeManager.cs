using System.Collections;
using System.Collections.Generic;
using GDTUtils.Extensions;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Modules.TimeManager
{

    public class TimeManager : LogicBase, ITimeManager
    {
        [SerializeField]
        private State state;

        private bool disposed = false;
        
        // *****************************
        // InitModule 
        // *****************************
        public void InitModule()
        {
            if (state.dynamicData.initialised)
            {
                return;
            }
            
            CompInit.Init(state);
            ToggleTimeEvaluation(false);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        private void Update()
        {
            if (!state.dynamicData.initialised || !state.dynamicData.evaluateTime)
            {
                return;
            }

            state.dynamicData.dictLayers.ForEach(x => x.Value.OnUpdate());
            state.dynamicData.cooldowns.ForEach(x => x.OnUpdate());
        }

        // *****************************
        // ToggleTimeEvaluation 
        // *****************************
        public void ToggleTimeEvaluation(bool _val)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);

            state.dynamicData.evaluateTime = _val;
            state.dynamicData.dictLayers.ForEach(x => x.Value.SetFrozen(!_val));
        }

        // *****************************
        // AddCooldown 
        // *****************************
        public int AddCooldown(float _duration, TimeLayerType _timeLayer)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompCooldowns.AddCooldown(state, _timeLayer, _duration);
        }

        // *****************************
        // PauseCooldown 
        // *****************************
        public void PauseCooldown(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompCooldowns.PauseCooldown(state, _id);
        }

        // *****************************
        // ResumeCooldown 
        // *****************************
        public void ResumeCooldown(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompCooldowns.ResumeCooldown(state, _id);
        }

        // *****************************
        // ResetCooldown 
        // *****************************
        public void ResetCooldown(int _id, float _newDuration = -1f)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompCooldowns.ResetCooldown(state, _id, _newDuration);
        }

        // *****************************
        // StopCooldown 
        // *****************************
        public void StopCooldown(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompCooldowns.StopCooldown(state, _id);
        }

        // *****************************
        // CheckCooldownPassed 
        // *****************************
        public bool CheckCooldownPassed(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompCooldowns.CheckCooldown(state, _id);
        }

        // *****************************
        // CheckCooldownIsRunning 
        // *****************************
        public bool CheckCooldownIsRunning(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompCooldowns.IsRunning(state, _id);
        }

        // *****************************
        // GetCooldownProgressNormalised 
        // *****************************
        public float GetCooldownProgressNormalised(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompCooldowns.GetProgress(state, _id);
        }

        // *****************************
        // GetCoolDownDuration 
        // *****************************
        public float GetCooldownDuration(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompCooldowns.GetDuration(state, _id);
        }

        // *****************************
        // StopAllCoolDowns 
        // *****************************
        public void StopAllCoolDowns(int _id)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompCooldowns.PauseAll(state);
        }

        // *****************************
        // GetDeltaTime 
        // *****************************
        public float GetDeltaTime(TimeLayerType _layer)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompLayers.GetDeltaTime(state, _layer);
        }

        // *****************************
        // SetTImeScale 
        // *****************************
        public void SetTimeScale(TimeLayerType _layer, float _val)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            CompLayers.SetTimeScale(state, _layer, _val);
        }
        
        // *****************************
        // GetTimeScale 
        // *****************************
        public float GetTimeScale(TimeLayerType _layer)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompLayers.GetTimeScale(state, _layer);
        }
        
        // *****************************
        // GetTimeSinceStartup 
        // *****************************
        public float GetTimeSinceStartup(TimeLayerType _layer)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.dynamicData.initialised);
            return CompLayers.GetTimeSinceStartup(state, _layer);
        }
        
        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            
            disposed = true;
        }
    }

    // *****************************
    // State 
    // *****************************
    [System.Serializable]
    public class State
    {
        public DynamicData dynamicData = new();
        
        // *****************************
        // DynamicData 
        // *****************************
        public class DynamicData
        {
            public bool initialised = false;
            public Dictionary<TimeLayerType, TimeLayer> dictLayers  = new();
            public List<Cooldown>                       cooldowns   = new();

            public bool evaluateTime = false;
        }
    }
    
    // *****************************
    // TimeLayer 
    // *****************************
    public class TimeLayer
    {
        private float timeScale          = 0f;
        private float timeSinceStartup   = 0f;

        private float startedAt;
        private bool layerIsFrozen = false;

        public TimeLayer(float _timeSinceStartup)
        {
            startedAt = _timeSinceStartup;
            timeScale = 1f;
        }
        
        // *****************************
        // GetDeltaTime 
        // *****************************
        public float GetDeltaTime()
        {
            return layerIsFrozen ? 0f : Time.deltaTime * timeScale;
        }

        // *****************************
        // SetFrozen 
        // *****************************
        public void SetFrozen(bool _val) {
            layerIsFrozen = _val;
        }

        // *****************************
        // GetScale 
        // *****************************
        public float GetScale()
        {
            return timeScale;
        }

        // *****************************
        // SetScale 
        // *****************************
        public void SetScale(float _scale)
        {
            _scale           = Mathf.Clamp(_scale, 0f, float.MaxValue);
            timeScale        = _scale;
        }
        
        // *****************************
        // GetTimeSinceStartup 
        // *****************************
        public float GetTimeSinceStartup()
        {
            return timeSinceStartup;
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            timeSinceStartup += GetDeltaTime();
        }
    }
    
    // *****************************
    // Cooldown 
    // *****************************
    public class Cooldown
    {
        private float totalDuration;
        private float progress;
        private bool  isActive = false;
        private bool  isPassed = false;
        private float startedAt;
        
        private TimeLayer layer;

        public Cooldown(float _duration, TimeLayer _layer)
        {
            if (Mathf.Approximately(_duration, 0f))
            {
                throw new System.Exception($"Cooldown duration cannot be zero!");
            }
            
            totalDuration = _duration;
            layer         =  _layer;

            Reset();
        }
        
        // *****************************
        // Reset 
        // *****************************
        public void Reset(float _newDuration = -1f)
        {
            progress  = 0f;
            startedAt = layer.GetTimeSinceStartup();
            isActive  = true;
            isPassed  = false;

            bool wantNewDuration = _newDuration > 0f;
            if (wantNewDuration)
            {
                totalDuration = _newDuration;
            }
        }

        // *****************************
        // Stop 
        // *****************************
        public void Stop()
        {
            Reset();
            Pause();
        }
        
        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            if (!isActive)
            {
                return;
            }
            
            progress += layer.GetDeltaTime();
            
            // check if passed
            isPassed = progress > totalDuration;
            
            if (isPassed)
            {
                progress = totalDuration;
                isActive = false;
            }
        }

        // *****************************
        // Pause 
        // *****************************
        public void Pause()
        {
            isActive = false;
        }

        // *****************************
        // Resume 
        // *****************************
        public void Resume()
        {
            if (isPassed)
            {
                return;
            }
            
            isActive = true;
        }

        // *****************************
        // GetProgressNormalised 
        // *****************************
        public float GetProgressNormalised()
        {
            return progress / totalDuration;
        }

        // *****************************
        // GetTotalDuration 
        // *****************************
        public float GetTotalDuration()
        {
            return totalDuration;
        }

        // *****************************
        // IsPassed 
        // *****************************
        public bool IsPassed()
        {
            return isPassed;
        }

        // *****************************
        // IsRunning 
        // *****************************
        public bool IsRunning()
        {
            return isActive;
        }
    }
}