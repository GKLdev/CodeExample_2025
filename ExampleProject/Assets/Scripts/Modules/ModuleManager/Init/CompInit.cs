using Modules.ModuleManager;
using Modules.ModuleManager_Public;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Modules.ModuleManager
{
    public static class CompInit
    {
        // *****************************
        // Init
        // *****************************
        public static void Init(State _state, IModuleManager _self)
        {
            bool alreadyInitialized = _state.dynamicData.isInitialised;
            if (alreadyInitialized)
            {
                return;
            }
            
            _state.installer.self = _self;
            InitModulesSequence(_state);
            InitRuntimeUpdateSequence(_state);
            
            _state.dynamicData.isInitialised = true;
        }

        // *****************************
        // InitModulesSequence
        // *****************************
        static void InitModulesSequence(State _state)
        {
            // init context and bind modules //
            _state.context.Run();

            // initialize sequence //
            for (int i = 0; i < _state.installer.initializationSequence.Count; i++)
            {
                IModuleInit module = (_state.installer.initializationSequence[i].instance as IModuleInit);
                module?.InitModule();
            }
        }


        // *****************************
        // InitRuntimeUpdateSequence
        // *****************************
        static void InitRuntimeUpdateSequence(State _state)
        {
            // update
            for (int i = 0; i < _state.updateSequence.Count; i++)
            {
                TryAddToUpdate(_state, _state.updateSequence[i], _state.dynamicData.updateSequenceRuntime);
            }

            // late update
            for (int i = 0; i < _state.lateUpdateSequence.Count; i++)
            {
                TryAddToUpdate(_state, _state.lateUpdateSequence[i], _state.dynamicData.lateUpdateSequenceRuntime);
            }

            void TryAddToUpdate<TIface>(State _state, LogicBase _module, List<TIface> _list)
                where TIface : class
            {
                TIface moduleCasted = _module as TIface;

                bool castFailed = moduleCasted == null;
                if (castFailed)
                {
                    Debug.LogWarning($"Module cannot be added since it does not implement {typeof(TIface)} or its NULL!");
                    return;
                }

                CompAddOrRemoveToUpdateSequence.AddToUpdateInternal(_state, moduleCasted, _list);
            }

        }
    }
}
