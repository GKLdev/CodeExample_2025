using System.Collections;
using System.Collections.Generic;
using Modules.ReferenceDb_Public;
using UnityEngine;

namespace Modules.ReferenceDb
{
    public static class CompGetEntry
    {
        
        // *****************************
        // GetEntry 
        // *****************************
        public static T GetEntry<T>(State _state, int _id) where T : DbEntryBase
        {
            LibModuleExceptions.ExceptionIfNotInitialized(_state.dynamicData.initialized);

            DbEntryBase entry = default;

            bool success = _state.dynamicData.entries.TryGetValue(_id, out entry);
            if (!success)
            {
                throw new System.Exception($"ReferenceDb: requested item with id={_id} not found!");
            }

            T result = entry as T;

            bool error = result == null;
            if (error)
            {
                throw new System.Exception($"ReferenceDb: failed to cast item with id={_id} to type={typeof(T)}");
            }
            
            return result;
        }

    }
}