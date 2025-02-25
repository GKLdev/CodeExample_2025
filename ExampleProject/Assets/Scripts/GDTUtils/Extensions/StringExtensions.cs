using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace GDTUtils.Extensions
{
    public static class StringExtensions
    {
        // *****************************
        // IsNullOrEmpty 
        // *****************************
        public static bool NullOrEmpty(this string _str)
        {
            return _str == null || _str.IsEmpty() || _str == "";
        }
    }
}