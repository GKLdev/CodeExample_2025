using Actions.Abilities_Public;
using Modules.CharacterController_Public;
using Modules.CharacterVisualController_Public;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.Abilities
{
    public class LibAbilityActions : MonoBehaviour
    {
        // *****************************
        // PlayBlockingAnimation
        // *****************************
        public static void PlayBlockingAnimation(AbilityActionSharedData _data, AnimationType _anim)
        {
            _data.controller.TogglePhysics(false);
            _data.ownerVisual.PlayAnimation(_anim);
        }

        // *****************************
        // PlayAnimation
        // *****************************
        public static void PlayAnimation(AbilityActionSharedData _data, AnimationType _anim)
        {
            _data.ownerVisual.PlayAnimation(_anim);
        }

        // *****************************
        // ForceStopAnimation
        // *****************************
        public static void ForceStopAnimation(AbilityActionSharedData _data)
        {
            _data.ownerVisual.ForceStopAnimation();
        }

        // *****************************
        // OnBlockingAnimEnd
        // *****************************
        public static void OnBlockingAnimEnd(AbilityActionSharedData _data)
        {
            _data.controller.TogglePhysics(true);
        }

        // *****************************
        // FillDispatcherData
        // *****************************
        public static void FillDispatcherData(
            DamageDispatcherData    _dispatcherData,
            DamageSource            _damageSource,
            FactionRestriction      _restriction,
            Vector3                 _position,
            Vector3                 _orientation,
            Vector2                 _scale,
            DamageType              _damageType,
            Transform               _followTarget,
            float                   _baseDamage,
            bool                    _makeCallbackOnFinish
            )
        {
            _dispatcherData.source          = _damageSource;
            _dispatcherData.restriction     = _restriction;

            _dispatcherData.position        = _position;
            _dispatcherData.orientation     = _orientation;

            _dispatcherData.scale           = _scale;
            _dispatcherData.type            = _damageType;
            _dispatcherData.damageValue     = _baseDamage; // TODO: add modifiers

            _dispatcherData.followTarget            = _followTarget;
            _dispatcherData.makeCallbackOnFinish    = _makeCallbackOnFinish;
        }

        // *****************************
        // GetDispatcherFromAlias
        // *****************************
        public static CATEGORY_DAMAGEDISPATCHERS GetDispatcherFromAlias(ReferenceDbAliasesConfig _dbConfig, string _alias)
        {
            CATEGORY_DAMAGEDISPATCHERS result = default;

            result = (CATEGORY_DAMAGEDISPATCHERS)_dbConfig.GetId(_alias); // see at damagemanager

            return result;
        }

        // *****************************
        // ForceCharacterOrientation
        // *****************************
        public static void ForceCharacterOrientation(ICharacterController _controller, AbilityConfigurationData _data)
        {
            if (_data == null)
            {
                return;
            }

            if (_data.applyStartingDirection)
            {
                _controller.P_Orientation = _data.startDirection;
            }

        }
    }
}