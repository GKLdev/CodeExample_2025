using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GDTUtils
{
    public static class GDTArrays
    {

        //*****************************
        // CheckArrayOverflow
        //*****************************
        public static bool CheckArrayOverflow<T>(int _argument, ref T[] _array, bool _exception = false)
        {
            ExeptionOnInvalidArray(_array, false);

            bool error = _argument < 0 || _argument >= _array.Length;
            if (_exception && error)
            {
                throw new System.Exception(GetOverflowString(_argument, _array.GetType()));
            }

            return error;
        }

        public static bool CheckArrayOverflow<T>(int _argument, ref List<T> _array, bool _exception = false)
        {
            ExeptionOnInvalidArray(_array, false);

            bool error = _argument < 0 || _argument >= _array.Count;
            if (_exception && error)
            {
                throw new System.Exception(GetOverflowString(_argument, _array.GetType()));
            }

            return error;
        }

        public static bool CheckArrayOverflow<T>(int _argument, ref NativeArray<T> _array, bool _exception = false) where T : struct
        {
            bool error = _argument < 0 || _argument >= _array.Length;
            if (_exception && error)
            {
                throw new System.Exception(GetOverflowString(_argument, _array.GetType()));
            }

            return error;
        }

        //*****************************
        // CastOneArrayToAnother
        //*****************************
        public static void CastOneArrayToAnother<T, T1>(List<T> _input, ref List<T1> _output) where T : class where T1 : class
        {
            ExeptionOnInvalidArray(_input);
            ExeptionOnInvalidArray(_output);

            for (int i = 0; i < _input.Count; i++)
            {
                T1 casted = _input[i] as T1;

                if (casted == null)
                {
                    throw new System.Exception($" Failed to cast { typeof(T) } to {typeof(T1)}");
                }

                _output.Add(casted);
            }
        }

        //*****************************
        // GetOverflowString
        //*****************************
        static string GetOverflowString(int _elementId, System.Type _type)
        {
            return $" Index ( {_elementId} ) of Array ( {_type} ) is out of range!";
        }

        //*****************************
        // ExeptionOnInvalidArray
        //*****************************
        static void ExeptionOnInvalidArray<T>(List<T> _input, bool _emptyIsOk = true)
        {
            bool error = _input == null || !_emptyIsOk && _input.Count == 0;
            if (error)
            {
                throw new System.Exception("List is Null or empty");
            }
        }

        //*****************************
        // ExeptionOnInvalidArray
        //*****************************
        static void ExeptionOnInvalidArray<T>(T[] _input, bool _emptyIsOk = true)
        {
            bool error = _input == null || !_emptyIsOk && _input.Length == 0;
            if (error)
            {
                throw new System.Exception("Array is Null or empty");
            }
        }
    }
}