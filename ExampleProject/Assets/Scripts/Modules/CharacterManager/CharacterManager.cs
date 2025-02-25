using GDTUtils;
using Modules.CharacterFacade_Public;
using Modules.CharacterManager_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using Modules.TimeManager_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.CharacterManager
{
    public class CharacterManager : LogicBase, ICharacterManager
    {
        [SerializeField]
        State state;

        [Inject]
        IModuleManager moduleMgr;

        // *****************************
        // InitModule
        // *****************************
        public void InitModule()
        {
            CompInit.Init(state, moduleMgr);
        }

        // *****************************
        // CreateCharacter
        // *****************************
        public ICharacterFacade CreateCharacter(CATEGORY_CHARACTERS _type)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            ICharacterFacade result = state.dynamic.manager.AwakeEntry(_type);

            // is player
            bool isPlayer = state.dynamic.playerCharacter == null && _type == state.dynamic.playerCategory;
            if (isPlayer)
            {
                state.dynamic.playerCharacter = result;
            }

            return result;
        }

        // *****************************
        // CreateCharacter
        // *****************************
        public ICharacterFacade CreateCharacter(string _alias)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);

            CATEGORY_CHARACTERS     category    = (CATEGORY_CHARACTERS)state.dynamic.referenceConfig.GetId(_alias);
            ICharacterFacade        result      = CreateCharacter(category);

            return result;
        }

        // *****************************
        // GetPlayer
        // *****************************
        public ICharacterFacade GetPlayer()
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            return state.dynamic.playerCharacter;
        }

        // *****************************
        // RemoveCharacter
        // *****************************
        public void RemoveCharacter(ICharacterFacade _character)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(state.initialized);
            state.dynamic.manager.SleepEntry(_character);

            // is player
            bool isPlayer = state.dynamic.playerCharacter != null && state.dynamic.playerCharacter == _character;
            if (isPlayer)
            {
                state.dynamic.playerCharacter = null;
            }
        }

        // *****************************
        // OnUpdate
        // *****************************
        public void OnUpdate()
        {
            if (!state.initialized)
            {
                return;
            }

            state.dynamic.manager.OnUpdate();
        }

        // *****************************
        // Dispose
        // *****************************
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }

    // *****************************
    // State
    // *****************************
    [System.Serializable]
    public class State
    {
        public bool initialized = false;

        public ConfigCharacterManager   config;
        public Transform                fallbackRoot;
        public Transform[]              poolRoots;
        public DynamicData              dynamic = new();

        // *****************************
        // DynamicData
        // *****************************
        [System.Serializable]
        public class DynamicData
        {
            public PoolBasedManager<CATEGORY_CHARACTERS, ICharacterFacade, FactoryCharacter> manager;

            public ReferenceDbAliasesConfig referenceConfig;
            public ITimeManager timeMng;
            public TimeManager_Public.TimeLayerType timeLayer;

            public ICharacterFacade     playerCharacter;
            public CATEGORY_CHARACTERS  playerCategory;
        }
    }
}