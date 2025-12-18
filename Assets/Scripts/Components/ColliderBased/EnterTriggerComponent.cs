using System;
using DefaultNamespace.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Components.ColliderBased
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private LayerMask _layer = ~0;
        [SerializeField] private EnterEvent _event;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.IsInLayer(_layer))
                return;
                
            if (!string.IsNullOrEmpty(_tag) && !other.gameObject.CompareTag(_tag))
                return;
                
            _event.Invoke(other.gameObject);
        }
        
        [Serializable]
        public class EnterEvent : UnityEvent<GameObject>
        {
        }
    }
}