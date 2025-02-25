using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.DamageDispatcher_Public
{
    [CreateAssetMenu(fileName = "ConfigDamageDispatcher", menuName = "Configs/Damage/Dispatcher/Config")]
    public class ConfigDamageDispatcher : DbEntryBase
    {
        [Tooltip("Damage will be applied after some delay")]
        public bool     HasChargeTime;

        [Tooltip("Charging time before first damage tick")]
        public float    ChargingTime;

        [Tooltip("If true - damage will be applied several times")]
        public bool     IsPeriodic;

        [Tooltip("If true - deals constant damage every frame")]
        public bool DealDamagePerSecond = false;

        [Tooltip("Delay between ticks")]
        public float    TickDelay;

        [Tooltip("Ticks count for periodic damage")]
        public int      TicksCount;

        [Tooltip("Should dispatcher rotate by intself?")]
        public bool IsRotating;

        [Tooltip("Rotation speed of a dispatcher (sign dictates if rotation CV or CCV)")]
        public float RotationSpeed;

        [Tooltip("Time layer")]
        public TimeLayerType TimeLayer = TimeLayerType.World;

        [Tooltip("Auto detroy on finished")]
        public bool DestroyOnFinished = false;

        [Tooltip("Local axises this dispatcher can be scaled on")]
        public Vector3 ScaleAxises;
    }
}