using GDTUtils;
using Modules.CharacterFacade_Public;
using Modules.Controllable_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Controllable
{
    public class ControllableCharacter : LogicBase, IControllable
    {
        public SerializedInterface<ICharacterFacade> characterFacade;

        // *****************************
        // Move
        // *****************************
        public void Move(Vector3 _direction)
        {
            characterFacade.Value.P_Controller.Move(_direction);
        }

        // *****************************
        // LookAt
        // *****************************
        public void LookAt(Vector3 _wSpacePos)
        {
            characterFacade.Value.P_Controller.LookAt(_wSpacePos);
        }

        // *****************************
        // SetDirectMovementMode
        // *****************************
        public void SetDirectMovementMode()
        {
            characterFacade.Value.P_Controller.SetDirectMovementMode();
        }

        // *****************************
        // SetNavigationTarget
        // *****************************
        public void SetNavigationTarget(Vector3 _targetPos)
        {
            characterFacade.Value.P_Controller.SetNavigationTarget(_targetPos);
        }
    }
}