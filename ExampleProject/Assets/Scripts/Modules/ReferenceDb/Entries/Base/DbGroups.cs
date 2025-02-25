using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ReferenceDb
{
    [CreateAssetMenu(menuName = "ReferenceDb/Base/DbGroups", fileName = "DbGroups")]
    public class DbGroups : ScriptableObject
    {
        public List<GroupContainer> groups = new();

        [System.Serializable]
        public class GroupContainer
        {
            public string alias;
            public List<string> membersAliases;
        }
    }
}