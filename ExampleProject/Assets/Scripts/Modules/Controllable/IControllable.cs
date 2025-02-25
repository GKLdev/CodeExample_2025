using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Controllable_Public
{
    public interface IControllable
    {
        void Move(Vector3 _direction);
        void LookAt(Vector3 _wSpacePos);
        void SetNavigationTarget(Vector3 _targetPos);
        void SetDirectMovementMode();
    }
}