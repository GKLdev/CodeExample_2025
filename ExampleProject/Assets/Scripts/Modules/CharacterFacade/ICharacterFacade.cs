using GDTUtils;
using GDTUtils.Common;
using GDTUtils.Patterns.Factory;
using Modules.CharacterAbilitiesManager_Public;
using Modules.CharacterController_Public;
using Modules.CharacterVisualController_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterFacade_Public
{
    public interface ICharacterFacade : 
        IModuleUpdate, 
        IFactoryProduct,
        IPoolable,
        IElementTypeAccess<CATEGORY_CHARACTERS>,
        IGameObjectAccess
    {
        ICharacterController        P_Controller { get; }
        ICharacterAbilitiesManager  P_AbilitiesMgr { get; }

        void SetupCharacter(ConfigPlayerController configController, ConfigDamageable configDamageable);
        void InitModule(ICharacterVisualController _visual);
        DamageSource GetDamageSource();     
        void MakeAIControlled();
        void MakePlayerControlled();
        bool IsDead();
    }
}