using Modules.ActionsManger_Public;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.Abilities
{
    [CreateAssetMenu(fileName = "ConfigAction_MobAttack", menuName = "Configs/Actions/Abilities/ConfigAction_MobAttack")]
    public class ConfigAbility_MobAttack : ConfigActionBase
    {
        [Header("Damage dispatcher settings")]
        public float                BaseDamage;
        public string               DispatcherAlias;
        public DamageType           DamageType;
        public Vector2              DispatcherScale;
        public FactionRestriction   FactionRestriction;
        public bool                 FollowCaster = false;

        [Header("Visuals")]
        public Modules.CharacterVisualController_Public.AnimationType animation;
    }
}