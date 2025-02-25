using GDTUtils;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace Modules.DamageManager
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
            List<PoolTypeSettings<CATEGORY_DAMAGEDISPATCHERS>> settings = new();

            for (int i = 0; i < _state.config.poolSettings.Count; i++)
            {
                var container   = _state.config.poolSettings[i];
                var root        = i >= _state.poolRoots.Length ? _state.fallbackRoot : _state.poolRoots[i];
                
                settings.Add(container.CreatePbmSettings(_state, root));
            }

            _state.dynamic.manager = new(_moduleMgr, settings);

            // finish
            _state.initialized = true;
        }

        // *****************************
        // Dispose 
        // *****************************
        public static void Dispose(State _state)
        {
            _state.dynamic.colliderToDamageable.Clear();
            _state.dynamic.damageableToCollider.Clear();
            _state.dynamic.manager.Dispose();

            _state.initialized = false;
        }
    }
}