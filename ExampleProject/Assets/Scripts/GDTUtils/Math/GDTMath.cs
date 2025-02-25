using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using GDTUtils.Math;
using Unity.VisualScripting;

namespace GDTUtils
{

    public static class GDTMath
    {
        //*****************************
        // ClampPercent
        //*****************************
        /// <summary>
        /// Clamps value between -1f and 1f;
        /// </summary>
        /// <param name="_val"></param>
        public static void ClampPercent(ref float _val)
        {
            _val = math.sign(_val) * math.clamp(_val, 0f, 1f);
        }

        //*****************************
        // ProcessSimpleAxis
        //*****************************
        /// <summary>
        /// Moves value from current value to target
        /// </summary>
        /// <param name="_axis"></param>
        /// <param name="_delta"></param>
        /// <param name="_target"></param>
        public static void ProcessSimpleAxis(ref float _axis, float _delta, float _target)
        {
            _axis += _delta;
            _axis = _axis > 0f ? math.clamp(_axis, 0, _target) : math.clamp(_axis, -_target, 0);
        }

        // ***********************
        //  Equal
        // ***********************
        /// <summary>
        /// '=' for floats
        /// </summary>
        /// <param name="_val"></param>
        /// <param name="_reference"></param>
        /// <param name="_precision"></param>
        /// <returns></returns>
        public static bool Equal(float _val, float _reference, float _precision = 0f)
        {
            return LibFloatOperations.Equal(_val, _reference, _precision);
        }

        // ***********************
        //  MoreOREqual
        // ***********************
        public static bool MoreOREqual(float _val, float _reference, float _precision = 0f)
        {
            return LibFloatOperations.MoreOREqual(_val, _reference, _precision);
        }

        // ***********************
        //  MoreNotEqual
        // ***********************
        public static bool MoreNotEqual(float _val, float _reference, float _precision = 0f)
        {
            return LibFloatOperations.MoreNotEqual(_val, _reference, _precision);
        }

        // ***********************
        //  LessOREqual
        // ***********************
        public static bool LessOREqual(float _val, float _reference, float _precision = 0f)
        {
            return LibFloatOperations.LessOREqual(_val, _reference, _precision);
        }

        // ***********************
        //  LessNOTEqual
        // ***********************
        public static bool LessNOTEqual(float _val, float _reference, float _precision = 0f)
        {
            return LibFloatOperations.LessNOTEqual(_val, _reference, _precision);
        }
        
        // ***********************
        //  SmoothInterpolate
        // ***********************
        public static float SmoothInterpolate(float _from, float _to, ref float _progressAxis, float _delta)
        {
            float normalizedDelta = _delta / (_to - _from);
            ProcessSimpleAxis(ref _progressAxis, normalizedDelta, 1f);

            float sinInput     = Mathf.Deg2Rad * (-90f + 180f * _progressAxis);
            float smoothFactor = (1f + Mathf.Sin(sinInput)) * 0.5f;
            float result       = _from + (_to - _from) * smoothFactor;

            return result;
        }
        
        // ***********************
        //  SmoothInterpolate
        // ***********************
        public static Vector3 SmoothInterpolate(Vector3 _from, Vector3 _to, ref float _progressAxis, float _delta)
        {
            Vector3 direction = _to - _from;

            float   progress = SmoothInterpolate(0f, direction.magnitude, ref _progressAxis, _delta);
            Vector3 result   = _from + direction.normalized * progress;

            return result;
        }
    }
}

