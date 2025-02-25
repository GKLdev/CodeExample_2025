using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Modules.ModuleManager
{
    public static class CompAddOrRemoveToUpdateSequence
    {
        // *****************************
        // AddToUpdate
        // *****************************
        public static void AddToUpdate(State _state, IModuleUpdate _module)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.dynamicData.isInitialised);
            AddToUpdateInternal(_state, _module, _state.dynamicData.updateSequenceRuntime);
        }

        // *****************************
        // RemoveFromUpdate
        // *****************************
        public static void RemoveFromUpdate(State _state, IModuleUpdate _module)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.dynamicData.isInitialised);
            RemoveFromUpdateInternal(_state, _module, _state.dynamicData.updateSequenceRuntime);
        }

        // *****************************
        // AddToLateUpdate
        // *****************************
        public static void AddToLateUpdate(State _state, IModuleLateUpdate _module)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.dynamicData.isInitialised);
            AddToUpdateInternal(_state, _module, _state.dynamicData.lateUpdateSequenceRuntime);
        }

        // *****************************
        // RemoveFromLateUpdate
        // *****************************
        public static void RemoveFromLateUpdate(State _state, IModuleLateUpdate _module)
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.dynamicData.isInitialised);
            RemoveFromUpdateInternal(_state, _module, _state.dynamicData.lateUpdateSequenceRuntime);
        }


        // *****************************
        // AddToUpdateExternal
        // *****************************
        public static void AddToUpdateInternal<TIface>(State _state, TIface _module, List<TIface> _updateSequence)
            where TIface : class
        {
            bool error = _module == null;
            if (error)
            {
                Debug.LogWarning($"Module cannot be added since it does not implement IModuleUpdate or its NULL!");
                return;
            }

            _updateSequence.Add(_module);
        }

        // *****************************
        // RemoveFromUpdateInternal
        // *****************************
        public static void RemoveFromUpdateInternal<TIface>(State _state, TIface _module, List<TIface> _updateSequence)
            where TIface : class
        {
            bool error = _module == null;
            if (error)
            {
                Debug.LogWarning($"Module cannot be removed since it does not implement IModuleUpdate or its NULL!");
                return;
            }

            _updateSequence.Remove(_module);
        }
    }
}
