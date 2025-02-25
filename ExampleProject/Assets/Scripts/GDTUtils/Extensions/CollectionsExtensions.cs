using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GDTUtils.Extensions
{
    public static class CollectionsExtensions
    {
        // *****************************
        // ForEach 
        // *****************************
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> _dict, Action<KeyValuePair<TKey, TValue>> _action)
        {
            foreach (var pair in _dict)
            {
                _action(pair);
            }
        }
        
        // *****************************
        // ForEach 
        // *****************************
        public static void ForEach<TValue>(this HashSet<TValue> _hash, Action<TValue> _action)
        {
            foreach (TValue value in _hash)
            {
                _action(value);
            }
        }
    }
}