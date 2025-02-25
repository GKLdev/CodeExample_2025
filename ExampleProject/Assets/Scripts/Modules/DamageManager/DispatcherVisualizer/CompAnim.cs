using System.Collections;
using UnityEngine;
using static UnityEngine.Timeline.AnimationPlayableAsset;

namespace Modules.DispatcherVisualizer
{
    public class CompAnim : MonoBehaviour
    {
        // *****************************
        // OnAnimEvent 
        // *****************************
        public static void OnAnimEvent(State _state, int _eventId, int _param)
        {
            bool ignore = _eventId != 0;
            if (ignore)
            {
                Debug.LogError("Unsupported event id!");
                return;
            }

            _state.dynamic.target.OnVisualizerFinished();
        }

        // *****************************
        // Start 
        // *****************************
        public static void Start(State _state, bool _loopMode = false)
        {
            _state.dynamic.isLoopMode = _loopMode;

            // reseting animation values
            _state.alphaControl.alpha = 0f;
            _state.alphaControl.SetAlpha();

            Pause(_state, false);
            _state.anim.Play(_state.config.AVar_DefaultState, 0);
            _state.anim.SetTrigger(_loopMode ? _state.config.AVar_StartLoopTrigger : _state.config.AVar_StartTrigger);

            CalculateSpeed(_state);
            _state.anim.SetFloat(_state.config.AVar_Speed, _state.dynamic.progressPlaybackSpeed);
        }

        // *****************************
        // CalculateSpeed 
        // *****************************
        static void CalculateSpeed(State _state)
        {
            var data = _state.dynamic.target.GetVisualData();
            _state.dynamic.progressPlaybackSpeed = 1f / data.ChargingOrTickDuration;
        }

        // *****************************
        // Pause 
        // *****************************
        public static void Pause(State _state, bool _val)
        {
            _state.dynamic.desiredAnimSpeed = _val ? 0f : 1f;
            _state.anim.speed               = _state.dynamic.desiredAnimSpeed;
            _state.dynamic.isStopped        = _val ? true : false;
        }

        // *****************************
        // Stop 
        // *****************************
        public static void Stop(State _state)
        {
            Pause(_state, true);
            _state.anim.StopPlayback();

            _state.alphaControl.alpha = 0f;
            _state.alphaControl.SetAlpha();

            _state.dynamic.isLoopMode = false;
        }

        // *****************************
        // OrderStopLoop 
        // *****************************
        public static void OrderStopLoop(State _state)
        {
            if (!_state.dynamic.isLoopMode)
            {
                return;
            }

            _state.anim.SetTrigger(_state.config.AVar_StopLoopTrigger);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public static void OnUpdate(State _state, float _delta)
        {
            float scaleMultiplier = _state.dynamic.timeMgr.GetTimeScale(_state.dynamic.timeLayer);
            _state.anim.speed = _state.dynamic.desiredAnimSpeed * scaleMultiplier;
        }
    }
}