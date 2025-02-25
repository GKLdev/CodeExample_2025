using Modules.CharacterFacade_Public;
using Modules.DamageDispatcher_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.DamageManager_Public
{
    public interface IDamageable : IDisposable
    {
        /// <summary>
        /// Callback on damage
        /// </summary>
        bool OnDamage(DamageSource _source, DamageType _type, float _value);

        /// <summary>
        /// Callback to init damageable
        /// </summary>
        void OnCreated();

        /// <summary>
        /// Setup custom config
        /// </summary>
        /// <param name="_config"></param>
        void SetupConfig(ConfigDamageable _config);

        /// <summary>
        /// Reset to default values. Active state not affected
        /// </summary>
        void ResetDamageable();

        /// <summary>
        /// Set active/innactive.
        /// </summary>
        /// <param name="_val"></param>
        void ToggleActive(bool _val);

        Faction GetFaction();

        bool IsDead();

        event System.Action<bool> OnDamageApplied; 
    }

    // *****************************
    // DamageSource
    // *****************************
    public class DamageSource
    {
        public Faction          faction;
        public System.Object    obj;
    }

    // *****************************
    // DamageType
    // *****************************
    // mb to config ?
    public enum DamageType
    {
        Physical = 0,
        Fire,
        Water,
        Earth,
        Air,
        Light,
        Darkness
    }
}