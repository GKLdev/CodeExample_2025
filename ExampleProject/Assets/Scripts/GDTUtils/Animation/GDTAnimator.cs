using System.Collections;
using System.Collections.Generic;
using ModestTree.Util;
using UnityEngine;
using System;

namespace GDTUtils.Animation
{

    public static class GDTAnimator
    {
        //*****************************
        // InitBehaviours
        //*****************************
        public static void InitBehaviours(Animator _anim, out AnimBhvBase[] _behaviours, AnimBehaviourInitContainer _container)
        {
            _behaviours = _anim.GetBehaviours<AnimBhvBase>();

            SetupBehaviours(_behaviours, _container);
        }

        //*****************************
        // SetupBehaviours
        //*****************************
        public static void SetupBehaviours(AnimBhvBase[] _behaviours, AnimBehaviourInitContainer _container)
        {
            if (_behaviours == null)
            {
                return;
            }

            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Setup(_container);
            }
        }

        //*****************************
        // DisposeBehaviours
        //*****************************
        public static void DisposeBehaviours(AnimBhvBase[] _behaviours)
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Dispose();
            }
        }
    }

    //*****************************
    // AnimBehaviourInitContainer
    //*****************************
    public class AnimBehaviourInitContainer : IDisposable
    {
        public Action<int, int> onAnimationEvent;

        private bool disposed = false;
        
        //*****************************
        // Dispose
        //*****************************
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            onAnimationEvent = null;
        }
    }
}