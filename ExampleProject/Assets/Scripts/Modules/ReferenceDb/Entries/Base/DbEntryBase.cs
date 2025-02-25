using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ReferenceDb_Public
{
    /// <summary>
    /// A 'static' database entry which should not change in runtime
    /// </summary>
    public abstract class DbEntryBase : ScriptableObject
    {
        public string  Alias => alias;
        public int     Id  => id;
        
        [SerializeField]
        private int id;
        
        [SerializeField]
        private string alias;
        
#if UNITY_EDITOR
        
        // *****************************
        // SetId 
        // *****************************
        /// <summary>
        /// /!\ Editor only.
        /// </summary>
        /// <param name="_id"></param>
        public void SetId(int _id)
        {
            id = _id;
        }

        // *****************************
        // SetAlias 
        // *****************************
        /// <summary>
        /// /!\ Editor only!
        /// </summary>
        /// <param name="_name"></param>
        public void SetAlias(string _name)
        {
            alias = _name;
        }
#endif

        public List<DbEntryBase> subEntries = new();
    }
}