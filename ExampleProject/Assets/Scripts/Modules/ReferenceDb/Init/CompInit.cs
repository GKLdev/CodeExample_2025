using System.Collections;
using System.Collections.Generic;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using UnityEngine;

namespace Modules.ReferenceDb
{
    public static class CompInit
    {
        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state, IModuleManager _modulesMng)
        {
            // bind zenject configs
            _state.dynamicData.aliasesConfig = new ReferenceDbAliasesConfig();
            _state.dynamicData.groupsConfig  = new ReferenceDbGroupsConfig();
            
            _modulesMng.Container.BindInstance(_state.dynamicData.aliasesConfig).AsSingle().NonLazy();
            _modulesMng.Container.BindInstance(_state.dynamicData.groupsConfig).AsSingle().NonLazy();

            // construct database
            BuildDatabase(_state);
            
            // mark initialized
            _state.dynamicData.initialized = true;
        }

        // *****************************
        // BuildDatabase 
        // *****************************
        static void BuildDatabase(State _state)
        {
            _state.dynamicData.entries.Clear();
            int lastAddedId = -1;
            
            _state.entriesList.categories.ForEach(x => x.entries.ForEach(y => _state.dynamicData.entries.Add(++lastAddedId, y.entry)));
        }
    }
}