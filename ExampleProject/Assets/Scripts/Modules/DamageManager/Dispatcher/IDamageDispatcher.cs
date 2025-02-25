using GDTUtils;
using GDTUtils.Common;
using GDTUtils.Patterns.Factory;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Modules.DamageDispatcher_Public
{
    public interface IDamageDispatcher : 
        IModuleUpdate,
        IFactoryProduct,
        IPoolable,
        IElementTypeAccess<CATEGORY_DAMAGEDISPATCHERS>,
        IGameObjectAccess
    {
        void Init(ConfigDamageDispatcher _config);

        /// <summary>
        /// Start dispatching damage.
        /// </summary>
        /// <param name="_data"></param>
        void StartDispatcher(DamageDispatcherData _data); // mb damage source ?

        /// <summary>
        /// Stop damage-per-second dispatcher.
        /// </summary>
        void StopDispatcher();

        /// <summary>
        /// Get visualization data
        /// </summary>
        DamageDispatcherVisualizationData GetVisualData();
        void Modify(); // ? 


        void ResetData();
        void OnVisualizerFinished();
    }

    // *****************************
    // DamageDispatcherData
    // *****************************
    public class DamageDispatcherData
    {
        public DamageType   type;
        public float        damageValue;
        public Vector2      scale;
        public Vector3      orientation;
        public Vector3      position;
        public Transform    followTarget = null; // follow position and rotation

        public FactionRestriction   restriction;
        public DamageSource         source;

        public System.Action    onDispatcherFinished;
        public bool             makeCallbackOnFinish = true;

        // *****************************
        // Reset
        // *****************************
        public void Reset()
        {
            type                = DamageType.Physical;
            damageValue         = 0f;
            scale               = new Vector2(1f,1f);
            orientation         = default;
            position            = default;

            restriction     = FactionRestriction.DamageEverything;

            followTarget            = null;
            source                  = null;
            //onDispatcherFinished    = null;
        }

        // *****************************
        // CopyTo
        // *****************************
        public void CopyTo(DamageDispatcherData _target)
        {
            _target.type                    = type;
            _target.damageValue             = damageValue;
            _target.scale                   = scale;
            _target.orientation             = orientation;
            _target.position                = position;
            _target.followTarget            = followTarget;
            _target.restriction             = restriction;
            _target.source                  = source;
            _target.onDispatcherFinished    = onDispatcherFinished;
            _target.makeCallbackOnFinish    = makeCallbackOnFinish;
        }
    }

    // *****************************
    // DamageDispatcherVisualizationData
    // *****************************
    public class DamageDispatcherVisualizationData
    {
        public float ChargingOrTickDuration;
    }

    // *****************************
    // FactionRestriction
    // *****************************
    public enum FactionRestriction
    {
        DamageEverything = 0,
        DamageEverythingButCasterFaction
    }

    // *****************************
    // Team
    // *****************************
    public enum Faction
    {
        Player = 0,
        AI
    }

    public enum ModifyParamType
    {
        DamageType = 0,
        MultiplyDamage,
        Scale

    }
}