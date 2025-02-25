using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.DamageDispatcher
{
    public class CollisionEventsRelay : LogicBase
    {
        public DamageDispatcher target;


        // *****************************
        // OnTriggerEnter
        // *****************************
        private void OnTriggerEnter(Collider other)
        {
            target.OnRelayTriggerEnter(other);
        }

        // *****************************
        // OnTriggerExit
        // *****************************
        private void OnTriggerExit(Collider other)
        {
            target.OnRelayTriggerExit(other);
        }
    }
}