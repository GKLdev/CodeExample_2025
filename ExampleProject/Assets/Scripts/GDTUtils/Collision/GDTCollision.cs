using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDTUtils.Collision;

namespace GDTUtils
{
    public static class GDTCollision
    {
        //*****************************
        // TestCollision
        //*****************************
        public static Vector3 ResolveCollisionForPosition(Vector3 _desiredPosition, ref CollisionResolveData _data)
        {
            return LibResolveCollision.ResolveCollisionForPosition(_desiredPosition, ref _data);
        }

        //*****************************
        // GenerateCdtResolveData
        //*****************************
        public static CollisionResolveData GenerateCdtResolveData(Transform _playerTrasf, Collider _playerCdt, int _layerMask, int _maxColliderContacts, float _gatherCdtOffset)
        {
            return LibResolveCollision.GenerateCdtResolveData(_playerTrasf, _playerCdt, _layerMask, _maxColliderContacts, _gatherCdtOffset);
        }

        //*****************************
        // DrawDebugInfo
        //*****************************
        /// <summary>
        /// call oat OnDrawGizmoz
        /// </summary>
        /// <param name="_data"></param>
        public static void DrawDebugInfo(ref CollisionResolveData _data)
        {
            LibResolveCollision.OnDrawGizmoz(ref _data);
        }
    }
}