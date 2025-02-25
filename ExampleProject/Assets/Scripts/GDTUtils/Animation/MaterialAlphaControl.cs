using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEditor.Progress;

namespace GDTUtils.Animation
{
    public class MaterialAlphaControl : MonoBehaviour
    {
        public Renderer[]   renderers;
        public float        alpha;
        public bool         setAplhaOnUpdate = false;
        public bool         initOnStart = false;

        private float[] defaultAlphas;
        private bool    initialized = false;
        
        private void Start()
        {
            if (initOnStart)
            {
                Init();
            }
        }

        // *****************************
        // Init 
        // *****************************
        public void Init()
        {
            if (initialized)
            {
                return;
            }

            defaultAlphas = new float[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                defaultAlphas[i] = renderers[i].material.color.a;
            }

            initialized = true;
        }

        // *****************************
        // Update 
        // *****************************
        private void Update()
        {
            if (!initialized || !setAplhaOnUpdate)
            {
                return;
            }

            SetAlphaInternal();
        }

        // *****************************
        // SetAlpha 
        // *****************************
        public void SetAlpha()
        {
            SetAlphaInternal();
        }

        // *****************************
        // SetAlphtaInternal 
        // *****************************
        private void SetAlphaInternal()
        {
            if (!initialized)
            {
                Debug.LogError($"Component MUST be initialized before usage! component={this}");
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                var col = renderers[i].material.color;
                col.a = Mathf.Clamp(defaultAlphas[i] * alpha, 0f, 1f);

                renderers[i].material.color = col;
            }
        }

    }
}