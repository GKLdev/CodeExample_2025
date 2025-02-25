using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils
{
    public static class GDTTypes
    {

        //*****************************
        // TryCreateObjectFromString
        //*****************************
        public static T TryCreateObjectFromString<T>(string _input) where T : class
        {
            System.Type type = System.Type.GetType(_input);

            bool wrongType = type == null;
            if (wrongType)
            {
                throw new System.Exception($"Type {_input} is invalid!");
            }

            object obj = System.Activator.CreateInstance(type);

            bool castFailed = (obj as T) == null;
            if (castFailed)
            {
                throw new System.Exception($" {type.GetType()} cannot be casted to { typeof(T)}");
            }

            return obj as T;
        }

    }
}