using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace GDTUtils
{
    [System.Serializable]
    public class SerializedInterface<T> where T : class
    {
        [field: SerializeField] private MonoBehaviour Field { get; set; }

        public T Value
        {
            get
            {
                if (casterField == null)
                {
                    casterField = Field as T;
                }

                return casterField;
            }
            set
            {
                if (value is T)
                {
                    casterField = value;
                    Field = value as MonoBehaviour;
                }
            }
        }

        private T casterField;
    }
}