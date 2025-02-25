using GDTUtils.Common;
using Modules.CharacterFacade_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterVisualController_Public
{
    public interface ICharacterVisualController : 
        IModuleInit, 
        IModuleUpdate, 
        IGameObjectAccess, 
        ICharacterFacadeCallbacks
    {
        Vector3 P_MovementVelocity { get; set; }

        /// <summary>
        /// Play custom fullbody animation
        /// </summary>
        /// <param name="_animation"></param>
        void PlayAnimation(AnimationType _animation);

        /// <summary>
        /// Force animator to default state
        /// </summary>
        void ForceStopAnimation();
        void Setup(VisualControllerSetupData _data);

        event Action<AnimEventType, int> OnAnimationFinished;
    }
}