using Modules.CharacterFacade_Public;
using Modules.CharacterVisualController_Public;
using System;
using UnityEngine;

namespace Modules.CharacterController_Public
{
    /// <summary>
    /// Contains player related non-visual logic
    /// </summary>
    public interface ICharacterController : 
        IModuleInit, 
        IModuleUpdate, 
        IDisposable, 
        ICharacterFacadeCallbacks
    {
        public Vector3      P_Position { get; set; }
        public Quaternion   P_Rotation { get; set; }
        public Vector3      P_Orientation { get; set; }

        void SetupConfig(ConfigPlayerController _config);

        /// <summary>
        /// Move at direction
        /// </summary>
        /// <param name="_direction"></param>
        void Move(Vector3 _direction);

        /// <summary>
        /// Move via direction using navmesh
        /// </summary>
        /// <param name="_direction"></param>
        void MoveViaNavmesh(Vector3 _direction);

        /// <summary>
        /// Simplified path mode for better performance
        /// </summary>
        /// <param name="_val"></param>
        void ToggleSimplifiedPathMode(bool _val);

        /// <summary>
        /// Look at position
        /// </summary>
        /// <param name="_direction"></param>
        void LookAt(Vector3 _wSpacePos);

        /// <summary>
        /// reset values to default
        /// </summary>
        void ResetModule();

        /// <summary>
        /// Toggle movement and collision update
        /// </summary>
        /// <param name="_val"></param>
        void TogglePhysics(bool _val);

        /// <summary>
        /// Attach visual controller and init immediately
        /// </summary>
        /// <param name="_visual"></param>
        void AttachVisual(ICharacterVisualController _visual);

        /// <summary>
        /// Set to path mode
        /// </summary>
        /// <param name="_targetPos"></param>
        void SetNavigationTarget(Vector3 _targetPos);

        /// <summary>
        /// Set to player controled mode (and active physics calculation)
        /// </summary>
        void SetDirectMovementMode();

        /// <summary>
        /// Get attached visual controller
        /// </summary>
        /// <returns></returns>
        ICharacterVisualController GetVisualController();
    }
}
