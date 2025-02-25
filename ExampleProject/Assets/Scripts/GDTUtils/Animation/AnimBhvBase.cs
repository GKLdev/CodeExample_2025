using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.Animation
{
    public class AnimBhvBase : StateMachineBehaviour, IDisposable
    {
        protected AnimBehaviourInitContainer initContainer;
        protected bool                       disposed = false;
        
        //*****************************
        // Setup
        //*****************************
        public void Setup(AnimBehaviourInitContainer _container)
        {
            initContainer = _container;
        }

        //*****************************
        // Dispose
        //*****************************
        public virtual void Dispose()
        {
            if (disposed)
            {
                return;
            }
            
            initContainer = null;
        }
    }
}