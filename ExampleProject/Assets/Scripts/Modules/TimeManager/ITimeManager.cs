using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.TimeManager_Public
{

    public interface ITimeManager : IDisposable, IModuleInit // IModuleUpdate
    {
        /// <summary>
        /// Must be set to true is you want this system to start working.
        /// if true - time manager will work as intended
        /// if false - it stops evaluating any time delta
        /// </summary>
        /// <param name="_val"></param>
        void ToggleTimeEvaluation(bool _val);

        /// <summary>
        /// add new cooldown. This just a registration! Cooldown will start ONLY AFTER calling 'ResetCooldown'
        /// </summary>
        /// <returns></returns>
        int AddCooldown(float _duration, TimeLayerType _timeLayer);
        
        /// <summary>
        /// pause current cooldown
        /// </summary>
        void PauseCooldown(int _id);

        /// <summary>
        /// resume current cooldown
        /// </summary>
        void ResumeCooldown(int _id);

        /// <summary>
        /// restart cooldown and reset internal values
        /// </summary>
        /// <param name="_id"></param>
        void ResetCooldown(int _id, float _newDuration = -1f);

        /// <summary>
        /// Stops cooldown. Inf fact a 'Reset', but without immediate playback.
        /// </summary>
        /// <param name="_id"></param>
        void StopCooldown(int _id);

        /// <summary>
        /// if cooldown passed - returns true
        /// </summary>
        /// <returns></returns>
        bool CheckCooldownPassed(int _id);

        /// <summary>
        /// Check if cooldown is running now.
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        bool CheckCooldownIsRunning(int _id);

        /// <summary>
        /// Returns normalized progress of given cooldown
        /// </summary>
        /// <returns></returns>
        float GetCooldownProgressNormalised(int _id);

        /// <summary>
        /// Get full cooldown duration
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        float GetCooldownDuration(int _id);

        /// <summary>
        /// stop all
        /// </summary>
        /// <param name="_id"></param>
        void StopAllCoolDowns(int _id);

        /// <summary>
        /// Get delta time from given layer
        /// </summary>
        /// <param name="_layer"></param>
        /// <returns></returns>
        float GetDeltaTime(TimeLayerType _layer);

        /// <summary>
        /// Set time scale to given layer
        /// </summary>
        /// <param name="_layer"></param>
        void SetTimeScale(TimeLayerType _layer, float _val);

        /// <summary>
        /// Get time scale from given layer
        /// </summary>
        /// <param name="_layer"></param>
        /// <returns></returns>
        float GetTimeScale(TimeLayerType _layer);

        /// <summary>
        /// Get total time passed from given layer
        /// </summary>
        /// <param name="_layer"></param>
        /// <returns></returns>
        float GetTimeSinceStartup(TimeLayerType _layer);
    }

    // *****************************
    // TimeLayer 
    // *****************************
    /// <summary>
    /// Default time layers. Module will automatically create them from this enum
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