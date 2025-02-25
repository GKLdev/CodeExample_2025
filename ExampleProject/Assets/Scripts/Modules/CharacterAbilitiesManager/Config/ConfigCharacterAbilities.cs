using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterAbilitiesManager
{
    [CreateAssetMenu(fileName = "ConfigCharacterAbilitiesManager", menuName = "Configs/Character/Abilities")]
    public class ConfigCharacterAbilities : DbEntryBase
    {
        public List<AbilitySettingsContainer> Abilities;

        [System.Serializable]
        public class AbilitySettingsContainer
        {
            public string Alias;
        }
    }
}