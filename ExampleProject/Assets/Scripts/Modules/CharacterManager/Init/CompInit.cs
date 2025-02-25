using GDTUtils;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterManager
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr)
        {
            _state.dynamic.referenceConfig = _moduleMgr.Container.Resolve<ReferenceDbAliasesConfig>();

            // init pbm manager
            List<PoolTypeSettings<CATEGORY_CHARACTERS>> settings = new();

            for (int i = 0; i < _state.config.PoolSettings.Count; i++)
            {
                var container = _state.config.PoolSettings[i];
                var root = i >= _state.poolRoots.Length ? _state.fallbackRoot : _state.poolRoots[i];

                settings.Add(container.CreatePbmSettings(_state, root));
            }

            _state.dynamic.manager          = new(_moduleMgr, settings);
            _state.dynamic.playerCategory   = (CATEGORY_CHARACTERS)_state.dynamic.referenceConfig.GetId(_state.config.PlayerCharacterAlias);

            // finish
            _state.initialized = true;
        }
    }
}