using System.Collections;
using System.Collections.Generic;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Modules.TimeManager
{
    public static class CompCooldowns
    {
        // *****************************
        // AddCooldown 
        // *****************************
        public static int AddCooldown(State _state, TimeLayerType _layer, float _duration)
        {
            var layer = CompLayers.GetLayer(_state, _layer);
            _state.dynamicData.cooldowns.Add(new Cooldown(_duration, layer));

            return _state.dynamicData.cooldowns.Count - 1;
        }

        // *****************************
        // PauseCooldown 
        // *****************************
        public static void PauseCooldown(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            cd.Pause();
        }
        
        // *****************************
        // PauseCooldown 
        // *****************************
        public static void ResumeCooldown(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            cd.Resume();
        }
        
        // *****************************
        // CheckCooldown 
        // *****************************
        public static bool CheckCooldown(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            return cd.IsPassed();
        }

        // *****************************
        // GetProgress 
        // *****************************
        public static float GetProgress(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            return cd.GetProgressNormalised();
        }
        
        // *****************************
        // GetMaxDuration 
        // *****************************
        public static float GetDuration(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            return cd.GetTotalDuration();
        }
        
        // *****************************
        // PauseAll 
        // *****************************
        public static void PauseAll(State _state)
        {
            _state.dynamicData.cooldowns.ForEach(x => x.Pause());
        }
        
        // *****************************
        // GetCooldown 
        // *****************************
        public static Cooldown GetCooldown(State _state, int _id)
        {
            bool error = _id < 0 || _id >= _state.dynamicData.cooldowns.Count;
            if (error)
            {
                throw new System.Exception($"GetCooldown: id={_id} is out of range!");
            }

            return _state.dynamicData.cooldowns[_id];
        }

        // *****************************
        // ResetCooldown 
        // *****************************
        public static void ResetCooldown(State _state, int _id, float _newDuration = -1f)
        {
            var cd = GetCooldown(_state, _id);
            cd.Reset(_newDuration);
        }

        // *****************************
        // StopCooldown 
        // *****************************
        public static void StopCooldown(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            cd.Stop();
        }

        // *****************************
        // IsRunning 
        // *****************************
        public static bool IsRunning(State _state, int _id)
        {
            var cd = GetCooldown(_state, _id);
            return cd.IsRunning();
        }
    }
}