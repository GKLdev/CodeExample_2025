using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger_Public
{
    [CreateAssetMenu(fileName = "ConfigAction_Example", menuName = "Configs/Actions/Example")]
    public class ConfigActionExample : ConfigActionBase
    {
        public int ExampleConfigField;
    }
}