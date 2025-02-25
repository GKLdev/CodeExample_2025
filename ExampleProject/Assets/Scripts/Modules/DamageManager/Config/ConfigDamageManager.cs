using GDTUtils;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.DamageManager
{
    [CreateAssetMenu(fileName = "ConfigDamageManager", menuName = "Configs/Damage/Manager")]
    public class ConfigDamageManager : ScriptableObject
    {
        public List<PoolSettingsContainer> poolSettings;

        [System.Serializable]
        public class PoolSettingsContainer
        {
            public string                       dispatcherAlias;
            public int                          prewarmElements;
            public PoolLimitType                limitType = PoolLimitType.None;

            // *****************************
            // CreatePbmSettings
            // *****************************
            public PoolTypeSettings<CATEGORY_DAMAGEDISPATCHERS> CreatePbmSettings(State _state, Transform _root)
            {
                int id = _state.dynamic.referenceConfig.GetId(dispatcherAlias);
                CATEGORY_DAMAGEDISPATCHERS category = default;

                try
                {
                    category = (CATEGORY_DAMAGEDISPATCHERS)id;
                }
                catch (System.Exception)
                {
                    Debug.LogError($"Failed to find dispatcher={dispatcherAlias} OR its not a member of 'CATEGORY_DAMAGEDISPATCHERS' category.");
                }

                PoolTypeSettings<CATEGORY_DAMAGEDISPATCHERS> result = new(category, _root, prewarmElements, limitType);

                return result;
            }
        }
    }
}