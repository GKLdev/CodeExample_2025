using System.Collections;
using UnityEngine;

namespace Modules.DispatcherVisualizer_Public
{
    [CreateAssetMenu(fileName = "ConfigDispatcherVisualizer", menuName = "Configs/Damage/Dispatcher/Visualizer")]
    public class ConfigDispatcherVisualizer : ScriptableObject
    {
        public string AVar_StartTrigger = "Start";

        public string AVar_StartLoopTrigger = "StartLoop";
        public string AVar_StopLoopTrigger  = "StopLoop";

        public string AVar_Speed        = "Speed";
        public string AVar_DefaultState = "NULL";
    }
}