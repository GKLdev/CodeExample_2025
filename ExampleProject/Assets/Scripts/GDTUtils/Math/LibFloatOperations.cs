using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.Math
{
    public class LibFloatOperations
    {
        // ***********************
        //  Equal
        // ***********************
        public static bool Equal(float _val, float _reference, float _precision = 0f)
        {
            bool result = Mathf.Abs(_val - _reference) < _precision;

            return result;
        }

        // ***********************
        //  MoreOREqual
        // ***********************
        public static bool MoreOREqual(float _val, float _reference, float _precision = 0f)
        {
            bool result = Mathf.Abs(_val - _reference) < _precision || Mathf.Approximately(_val, _reference) || _val > _reference;

            return result;
        }

        // ***********************
        //  MoreNotEqual
        // ***********************
        public static bool MoreNotEqual(float _val, float _reference, float _precision = 0f)
        {
            bool result = !(Mathf.Abs(_val - _reference) < _precision) && _val > _reference;

            return result;
        }

        // ***********************
        //  LessOREqual
        // ***********************
        public static bool LessOREqual(float _val, float _reference, float _precision = 0f)
        {
            bool result = Mathf.Abs(_val - _reference) < _precision || Mathf.Approximately(_val, _reference) || _val < _reference;

            return result;
        }

        // ***********************
        //  LessNOTEqual
        // ***********************
        public static bool LessNOTEqual(float _val, float _reference, float _precision = 0f)
        {
            bool result = !(Mathf.Abs(_val - _reference) < _precision) && _val < _reference;

            return result;
        }
    }
}