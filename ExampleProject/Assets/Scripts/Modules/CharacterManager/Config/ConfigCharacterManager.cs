using GDTUtils;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterManager
{

    [CreateAssetMenu(fileName = "ConfigCharacterManager", menuName = "Configs/Character/Manager")]
    public class ConfigCharacterManager : DbEntryBase
    {
        public string PlayerCharacterAlias;

        public List<PoolSettingsContainer> PoolSettings;

        [System.Serializable]
        public class PoolSettingsContainer
        {
            public string           characterAlias;
            public int              prewarmElements;
            public PoolLimitType    limitType = PoolLimitType.None;

            // *****************************
            // CreatePbmSettings
            // *****************************
            public PoolTypeSettings<CATEGORY_CHARACTERS> CreatePbmSettings(State _state, Transform _root)
            {
                int                 id          = _state.dynamic.referenceConfig.GetId(characterAlias);
                CATEGORY_CHARACTERS category    = default;

                try
                {
                    category = (CATEGORY_CHARACTERS)id;
                }
                catch (System.Exception)
                {
                    Debug.LogError($"Failed to find dispatcher={characterAlias} OR its not a member of 'CATEGORY_CHARACTERS' category.");
                }

                PoolTypeSettings<CATEGORY_CHARACTERS> result = new(category, _root, prewarmElements, limitType);

                return result;
            }
        }
    }
}