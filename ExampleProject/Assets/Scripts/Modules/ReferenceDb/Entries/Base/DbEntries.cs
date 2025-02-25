using System.Collections;
using System.Collections.Generic;
using Modules.ReferenceDb_Public;
using UnityEngine;


namespace Modules.ReferenceDb
{
    [CreateAssetMenu(menuName = "ReferenceDb/Base/DbEntries", fileName = "DbEntries")]
    public class DbEntries : ScriptableObject
    {
        public List<Category> categories = new();
        
        [System.Serializable]
        public class Category
        {
            public string categoryName;
            public List<EntryContainer> entries;
        }
        
        [System.Serializable]
        public class EntryContainer
        {
            public string      alias;
            public DbEntryBase entry;
        }
    }
}