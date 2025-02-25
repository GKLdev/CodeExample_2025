using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.TimeManager_Public
{

    public interface ITimeManager : IDisposable, IModuleInit
    {
        /// <summary>
        /// Must be set to true if you want this system to start working.
        /// if true - time manager will work as intended
        /// if false - it stops evaluating any time delta
        /// </summary>
        void ToggleTimeEvaluation(bool _val);

        /// <summary>
        /// Add new cooldown. This just a registration! Cooldown will start ONLY AFTER calling 'ResetCooldown'
        /// </summary>
        int AddCooldown(float _duration, TimeLayerType _timeLayer);
        
        /// <summary>
        /// Pause current cooldown
        /// </summary>
        void PauseCooldown(int _id);

        /// <summary>
        /// Resume current cooldown
        /// </summary>
        void ResumeCooldown(int _id);

        /// <summary>
        /// Restart cooldown and reset internal values
        /// </summary>
        void ResetCooldown(int _id, float _newDuration = -1f);

        /// <summary>
        /// Stops cooldown. In fact a 'Reset', but without immediate playback
        /// </summary>
        void StopCooldown(int _id);

        /// <summary>
        /// if cooldown passed - returns true
        /// </summary>
        bool CheckCooldownPassed(int _id);

        /// <summary>
        /// Check if cooldown is running now.
        /// </summary>
        bool CheckCooldownIsRunning(int _id);

        /// <summary>
        /// Returns normalized progress of given cooldown
        /// </summary>
        /// <returns></returns>
        float GetCooldownProgressNormalised(int _id);

        /// <summary>
        /// Get full cooldown duration
        /// </summary>
        float GetCooldownDuration(int _id);

        /// <summary>
        /// s\Stop all
        /// </summary>
        /// <param name="_id"></param>
        void StopAllCoolDowns(int _id);

        /// <summary>
        /// Get delta time from a given layer
        /// </summary>
        float GetDeltaTime(TimeLayerType _layer);

        /// <summary>
        /// Set time scale to given layer
        /// </summary>
        void SetTimeScale(TimeLayerType _layer, float _val);

        /// <summary>
        /// Get time scale from given layer
        /// </summary>
        float GetTimeScale(TimeLayerType _layer);

        /// <summary>
        /// Get total time passed from given layer
        /// </summary>
        float GetTimeSinceStartup(TimeLayerType _layer);
    }

    // *****************************
    // TimeLayer 
    // *****************************
    /// <summary>
    /// Default time layers. TimeManager will automatically create them from this enum
    /// </summary>
    public enum TimeLayerType
    {
        Undef = -1,
        World = 0,
        Player,
        PlayerWeapon,
        Projectiles
    }
}