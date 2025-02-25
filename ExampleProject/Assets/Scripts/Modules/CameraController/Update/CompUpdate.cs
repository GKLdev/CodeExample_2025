using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Modules.CameraController
{
    public class CompUpdate
    {
        // *****************************
        // Update
        // *****************************
        public static void Update(State _state)
        {
            _state.dynamic.desiredPos       = _state.root.position;

            GetDeltaTime(_state);
            UpdateFollowData(_state);
            Follow(_state);
            MaintainPositionAndAngle(_state);
            ApplyMaxDistanceContraint(_state);

            _state.root.transform.position = _state.dynamic.desiredPos;
        }

        // *****************************
        // Follow
        // *****************************
        static void Follow(State _state)
        {
            bool ignore = !_state.dynamic.needToFollowTarget;
            if (ignore)
            {
                return;
            }

            _state.dynamic.desiredPos = Vector3.Lerp(_state.dynamic.desiredPos, _state.dynamic.target.position, _state.config.MaxFollowSpeed * _state.dynamic.currentDeltaTime);
        }


        // *****************************
        // UpdateFollowData
        // *****************************
        static void UpdateFollowData(State _state)
        {
            bool ignore = !_state.dynamic.needToFollowTarget;
            if (ignore)
            {
                return;
            }

            _state.dynamic.currentTargetPos = _state.dynamic.target.position;
        }

        // *****************************
        // MaintainPositionAndAngle
        // *****************************
        static void MaintainPositionAndAngle(State _state)
        {
            // offset height
            Vector3 desiredPos = _state.root.position;
            desiredPos += _state.root.up * _state.config.DefaultHeight;

            // setup angle
            float camAngle = _state.config.DefaultAngle;
            _state.cameraHolder.rotation = _state.root.rotation * Quaternion.Euler(camAngle, 0f, 0f);

            // setup offset postion
            Vector3 dist            = desiredPos - _state.root.position;
            float   camHorOffset    = Mathf.Cos(camAngle * Mathf.Deg2Rad) * dist.magnitude;

            desiredPos += -_state.root.forward * camHorOffset;
            _state.cameraHolder.position = desiredPos;
        }


        // *****************************
        // ApplyMaxDistanceContraint
        // *****************************
        static void ApplyMaxDistanceContraint(State _state)
        {
            bool ignore = !_state.dynamic.needToFollowTarget;
            if (ignore)
            {
                return;
            }

            Vector3 distToTarget = _state.dynamic.currentTargetPos - _state.dynamic.desiredPos;
            
            bool clampByMaxDist = distToTarget.magnitude > _state.config.MaxChaseOffset;
            if (clampByMaxDist)
            {
                _state.dynamic.desiredPos = _state.dynamic.currentTargetPos + -distToTarget.normalized * _state.config.MaxChaseOffset;
            }
        }

        // *****************************
        // GetDeltaTime
        // *****************************
        static void GetDeltaTime(State _state)
        {
            _state.dynamic.currentDeltaTime = _state.dynamic.timeMgr.GetDeltaTime(_state.config.TimeLayer);
        }
    }
}