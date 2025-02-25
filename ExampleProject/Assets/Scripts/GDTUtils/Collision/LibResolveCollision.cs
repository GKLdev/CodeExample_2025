using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.Collision
{
    public static class LibResolveCollision
    {
        // ***********************
        //  GenerateCdtResolveData
        // ***********************
        public static CollisionResolveData GenerateCdtResolveData(Transform _playerTrasf, Collider _playerCdt, int _layerMask, int _maxColliderContacts, float _gatherCdtOffset)
        {
            CollisionResolveData result = new CollisionResolveData();

            result.inputData    = new CollisionResolveData.InputData();
            result.sharedData   = new CollisionResolveData.SharedData();
            result.tempData     = new CollisionResolveData.TempData();
            result.config       = new CollisionResolveData.ConfigData();

            result.inputData.playerTransf   = _playerTrasf;
            result.inputData.playerCdt      = _playerCdt;
            result.inputData.layerMask      = _layerMask;

            result.sharedData.nearColliders = new Collider[_maxColliderContacts];
            result.sharedData.contactPoints = new CdtContact[_maxColliderContacts];

            result.config.maxColliderContacts   = _maxColliderContacts;
            result.config.gatherCdtOffset       = _gatherCdtOffset;

            return result;
        }

        // ***********************
        //  ResolveCollisionForPosition
        // ***********************
        public static Vector3 ResolveCollisionForPosition(Vector3 _desiredPosition, ref CollisionResolveData _cdtData)
        {
            _cdtData.sharedData.cdtResolvedPosition = _desiredPosition;

            UpdateCrucialValues(ref _cdtData);
            GatherNearColliders(ref _cdtData);
            HandlePenetration(ref _cdtData);

            return _cdtData.sharedData.cdtResolvedPosition;
        }

        // ***********************
        //  UpdateCrucialValues
        // ***********************
        static void UpdateCrucialValues(ref CollisionResolveData _cdtData)
        {
            _cdtData.sharedData.playerCdtRotation   = _cdtData.inputData.playerTransf.rotation;
            _cdtData.sharedData.contactsCount       = 0;
        }

        // ***********************
        //  GatherNearColliders
        // ***********************
        static void GatherNearColliders(ref CollisionResolveData _cdtData)
        {
            _cdtData.tempData.height = _cdtData.inputData.playerCdt.bounds.size.y + 2f * _cdtData.config.gatherCdtOffset;
            _cdtData.tempData.radius = Mathf.Max(_cdtData.inputData.playerCdt.bounds.size.x, _cdtData.inputData.playerCdt.bounds.size.z) * 0.5f + _cdtData.config.gatherCdtOffset;

            _cdtData.tempData.capsulePt0 = _cdtData.sharedData.cdtResolvedPosition;
            _cdtData.tempData.capsulePt1 = _cdtData.sharedData.cdtResolvedPosition + _cdtData.inputData.playerTransf.up * (_cdtData.tempData.height - 2f * _cdtData.config.gatherCdtOffset);

            try
            {
                _cdtData.sharedData.overlapedCollidersCount = Physics.OverlapCapsuleNonAlloc(_cdtData.tempData.capsulePt0, _cdtData.tempData.capsulePt1, _cdtData.tempData.radius,
                    _cdtData.sharedData.nearColliders, _cdtData.inputData.layerMask);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new System.Exception($"Physics.OverlapCapsuleNonAlloc(): number of near colliders exeeds maximum of [{_cdtData.config.maxColliderContacts}]");
            }
        }

        static Collider currCdt;

        // ***********************
        //  HandlePenetration
        // ***********************
        static void HandlePenetration(ref CollisionResolveData _cdtData)
        {

            // *** no colliders *** //
            bool noNearColliders = _cdtData.sharedData.nearColliders[0] == null;
            if (noNearColliders)
            {
                return;
            }

            // *** analyse colliders *** //
            for (int i = 0; i < _cdtData.sharedData.overlapedCollidersCount; i++)
            {
                currCdt = _cdtData.sharedData.nearColliders[i];

                bool isSelf = currCdt == _cdtData.inputData.playerCdt;
                if (isSelf)
                {
                    continue;
                }

                bool intersectionHappened = Physics.ComputePenetration(
                    _cdtData.inputData.playerCdt, _cdtData.sharedData.cdtResolvedPosition, _cdtData.sharedData.playerCdtRotation,
                    currCdt, currCdt.transform.position, currCdt.transform.rotation, out Vector3 shiftDir, out float shiftDist);

                _cdtData.sharedData.cdtResolvedPosition += shiftDir.normalized * shiftDist;

                // *** add contact point *** //
                if (intersectionHappened)
                {
                    AddContactPoint(_cdtData.sharedData.contactsCount, _cdtData.inputData.playerCdt, currCdt, shiftDir, shiftDist, ref _cdtData);
                    _cdtData.sharedData.contactsCount++;
                }

                // *** debug *** //
                float dot = Mathf.Sign(Vector3.Dot((_cdtData.sharedData.cdtResolvedPosition - _cdtData.inputData.playerTransf.position).normalized, shiftDir));
                Debug.DrawLine(_cdtData.inputData.playerTransf.position, _cdtData.sharedData.cdtResolvedPosition + shiftDir * shiftDist, dot > 0f ? Color.green : Color.red, Time.deltaTime * 2f);
            }

        }

        // ***********************
        //  AddContactPoint
        // ***********************
        static void AddContactPoint(int _index, Collider _cdtSource, Collider _cdtOther, Vector3 _shiftDir, float _shiftValue, ref CollisionResolveData _cdtData)
        {
            bool isNull = _cdtData.sharedData.contactPoints[_index] == null;
            if (isNull)
            {
                _cdtData.sharedData.contactPoints[_index] = new CdtContact();
            }

            _cdtData.tempData.currContact               = _cdtData.sharedData.contactPoints[_index];
            _cdtData.tempData.currContact.sourceCdt     = _cdtSource;
            _cdtData.tempData.currContact.otherCdt      = _cdtOther;
            _cdtData.tempData.currContact.normal        = _shiftDir.normalized;
            _cdtData.tempData.currContact.shift         = _shiftValue;
        }

        // ***********************
        //  OnDrawGizmoz
        // ***********************
        public static void OnDrawGizmoz(ref CollisionResolveData _cdtData)
        {
            Gizmos.DrawWireSphere(_cdtData.tempData.capsulePt0, _cdtData.tempData.radius);
            Gizmos.DrawWireSphere(_cdtData.tempData.capsulePt1, _cdtData.tempData.radius);
        }
    }

    // ***********************
    //  CdtContact
    // ***********************
    public class CdtContact
    {
        public Collider sourceCdt;
        public Collider otherCdt;
        public Vector3  normal;
        public float    shift;
    }

    // ***********************
    //  CollisionResolveData
    // ***********************
    // strtuct is used to support cpu cashe friendly code.
    public struct CollisionResolveData
    {
        public SharedData   sharedData;
        public InputData    inputData;
        public TempData     tempData;
        public ConfigData   config;

        public struct SharedData
        {
            public Vector3      cdtResolvedPosition;
            public Collider[]   nearColliders;
            public Quaternion   playerCdtRotation;

            public int overlapedCollidersCount;
            public int contactsCount;

            public CdtContact[] contactPoints; // 1 per collider

        }

        public struct TempData
        {
            public CdtContact   currContact;
            public float        height;
            public float        radius;
            public Vector3      capsulePt0;
            public Vector3      capsulePt1;
        }

        public struct InputData
        {
            // directly input params //
            //public Vector3 desiredPosition; // is set from ResolveCollisionForPosition
            public Transform    playerTransf;
            public Collider     playerCdt;
            public int          layerMask;
        }

        public struct ConfigData
        {
            public int maxColliderContacts;
            public float gatherCdtOffset;
        }
    }
}