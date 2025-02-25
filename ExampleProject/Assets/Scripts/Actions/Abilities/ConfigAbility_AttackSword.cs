using Modules.ActionsManger_Public;
using System.Collections;
using UnityEngine;

namespace Actions.Abilities
{
    [CreateAssetMenu(fileName = "ConfigAction_AttackSword", menuName = "Configs/Actions/Abilities/ConfigAction_AttackSword")]
    public class ConfigAbility_AttackSword : ConfigActionBase
    {
        public AnimationCurve   ForwardMovementCurve;
        public float            ForwardMovementDistance;
    }
}