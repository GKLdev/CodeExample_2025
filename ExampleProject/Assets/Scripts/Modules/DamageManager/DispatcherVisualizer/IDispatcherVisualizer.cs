using Modules.DamageDispatcher_Public;
using Modules.TimeManager_Public;
using System;
using System.Collections;
using UnityEngine;

namespace Modules.DispatcherVisualizer_Public
{
    public interface IDispatcherVisualizer : IModuleInit, IDisposable
    {
        /// <summary>
        /// Assign target dispatcher.
        /// </summary>
        /// <param name="_target"></param>
        void Setup(IDamageDispatcher _target, TimeLayerType _layer);
        
        /// <summary>
        /// Run visualization.
        /// </summary>
        void StartVisualizer(bool _loopMode = false);

        /// <summary>
        /// Stop visualization immediately.
        /// </summary>
        void Stop();

        /// <summary>
        /// Order stop visualizarion if its at loop mode 
        /// </summary>
        void OrderStopLoop();

        /// <summary>
        /// Update loop.
        /// </summary>
        /// <param name="_delta"></param>
        void OnUpdate(float _delta);
    }
}