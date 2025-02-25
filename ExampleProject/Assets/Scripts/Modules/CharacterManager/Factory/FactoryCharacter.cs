using GDTUtils;
using Modules.CharacterFacade_Public;
using Modules.CharacterVisualController_Public;
using Modules.ReferenceDb_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.CharacterManager_Public
{
    public class FactoryCharacter : PbmFactoryBase
    {
        DbEntryCharacter        entryCasted;
        CATEGORY_CHARACTERS     type;

        // *****************************
        // Init
        // *****************************
        public override void Init(DbEntryBase _entry, DiContainer _container, Enum _category, Transform _parent)
        {
            base.Init(_entry, _container, _category, _parent);

            entryCasted = _entry as DbEntryCharacter;
            type = (CATEGORY_CHARACTERS)_category;
        }

        // *****************************
        // Produce
        // *****************************
        public override GDTUtils.IPoolable Produce()
        {
            ICharacterFacade            result          = container.InstantiatePrefabForComponent<ICharacterFacade>(entryCasted.CharacterFacade);
            ICharacterVisualController  characterVisual = container.InstantiatePrefabForComponent<ICharacterVisualController>(entryCasted.CharacterVisual);

            result.SetupCharacter(entryCasted.Config, entryCasted.Damageable);
            result.InitModule(characterVisual);

            result.P_ElementType                        = type;
            result.P_GameObjectAccess.transform.parent  = parent;

            return result;
        }
    }
}