using System.Collections;
using System.Collections.Generic;
using Modules.ReferenceDb_Public;
using UnityEngine;

namespace Modules.ReferenceDb
{
    [CreateAssetMenu(menuName = "ReferenceDb/Base/DbEntries/Debug/DbEntryDebug", fileName = "DbEntryDebug")]
    public class DbEntryDebug : DbEntryBase
    {
        public string testString = "DbEntryDebug";
    }
}