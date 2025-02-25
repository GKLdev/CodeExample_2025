using Modules.ReferenceDb_Public;
using System.Collections;
using UnityEngine;

namespace Modules.DamageDispatcher_Public
{
    [CreateAssetMenu(fileName = "DbEntryDamageDispatcher", menuName = "Configs/Damage/Dispatcher/DbEntryConfig")]
    public class DbEntryDamageDispatcher : DbEntryBase
    {
        public ConfigDamageDispatcher   Config;
        public LogicBase                Dispatcher;
    }
}