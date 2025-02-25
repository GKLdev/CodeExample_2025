using Modules.DamageManager_Public;
using System.Collections;
using UnityEngine;

namespace Modules.DamageManager
{
    public static class CompDamageableRegistration
    {
        // *****************************
        // Register
        // *****************************
        public static void Register(State state, IDamageable _component, Collider _collider)
        {
            bool alreadyRegistered = state.dynamic.damageableToCollider.ContainsKey(_component);
            if (alreadyRegistered)
            {
                Debug.Assert(false, $"Trying to register damageable which is already registered!");
            }

            state.dynamic.damageableToCollider.Add(_component, _collider);
            state.dynamic.colliderToDamageable.Add(_collider, _component);
        }

        // *****************************
        // Unregister
        // *****************************
        public static void Unregister(State state, IDamageable _component)
        {
            bool registered = state.dynamic.damageableToCollider.ContainsKey(_component);
            if (!registered)
            {
                Debug.Assert(false, $"Trying to unregister damageable which is not registered!");
            }

            var collider = state.dynamic.damageableToCollider[_component];

            state.dynamic.damageableToCollider.Remove(_component);
            state.dynamic.colliderToDamageable.Remove(collider);
        }


        // *****************************
        // GetDamageable
        // *****************************
        public static IDamageable GetDamageable(State state, Collider _collision)
        {
            bool registered = state.dynamic.colliderToDamageable.ContainsKey(_collision);
            if (!registered)
            {
                Debug.Assert(false, $"Collider={_collision.name} is unregistered at DamageManager!");
            }

            return state.dynamic.colliderToDamageable[_collision];
        }
    }
}