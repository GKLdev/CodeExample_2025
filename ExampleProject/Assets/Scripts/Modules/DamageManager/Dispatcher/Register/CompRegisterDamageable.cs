using System.Collections;
using UnityEngine;

namespace Modules.DamageDispatcher
{
    public static class CompRegisterDamageable
    {
        // *****************************
        // TryRegisterTarget
        // *****************************
        public static void TryRegisterTarget(State _state, Collider _cdt)
        {
            var component =_state.dynamic.damageMgr.GetDamageableByCollision(_cdt);

            bool notRegisteredAtDamagebale = component == null;
            if (notRegisteredAtDamagebale)
            {
                return;
            }

            bool alreadyRegistered = _state.dynamic.registeredTargets.Contains(component);
            if (alreadyRegistered)
            {
                return;
            }

            _state.dynamic.registeredTargets.Add(component);
        }

        // *****************************
        // TryUnregisterTarget
        // *****************************
        public static void TryUnregisterTarget(State _state, Collider _cdt)
        {
            var component = _state.dynamic.damageMgr.GetDamageableByCollision(_cdt);
            bool notRegisteredAtDamagebale = component == null;
            if (notRegisteredAtDamagebale)
            {
                return;
            }

            bool registered = _state.dynamic.registeredTargets.Contains(component);
            if (!registered)
            {
                return;
            }

            _state.dynamic.registeredTargets.Remove(component);
        }
    }
}