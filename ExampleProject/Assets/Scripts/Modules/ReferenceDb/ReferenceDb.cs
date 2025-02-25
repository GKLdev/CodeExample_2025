using System;
using System.Collections;
using System.Collections.Generic;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using UnityEngine;
using Zenject;

namespace Modules.ReferenceDb
{
    // TODO: Add min and max id aliases for each category af ReferenceDb
    public class ReferenceDb : LogicBase, IReferenceDb
    {
        [SerializeField]
        public State state = new();

        [Inject] 
        private IModuleManager moduleManager;
        
        // *****************************
        // InitModule 
        // *****************************
        public void InitModule()
        {
            if (state.dynamicData.initialized)
            {
                throw new System.Exception("ReferenceDb: cannot be initialized twice!");
            }
            
            CompInit.Init(state, moduleManager);
        }

        // *****************************
        // GetEntry 
        // *****************************
        public T GetEntry<T>(int _id) where T : DbEntryBase
        {
            return CompGetEntry.GetEntry<T>(state, _id);
        }

        public T GetEntry<T>(string _alias) where T : DbEntryBase
        {
            int id = state.dynamicData.aliasesConfig.GetId(_alias);
            return GetEntry<T>(id);
        }
    }

    // *****************************
    // State 
    // *****************************
    [System.Serializable]
    public class State
    {

        public DbEntries entriesList;
        public DbGroups  groupsList;
        
        public DynamicData dynamicData = new();
        
        public class DynamicData
        {
            public bool initialized = false;

            public ReferenceDbAliasesConfig     aliasesConfig;
            public ReferenceDbGroupsConfig      groupsConfig;

            public Dictionary<int, DbEntryBase> entries = new();
        }
    }

}