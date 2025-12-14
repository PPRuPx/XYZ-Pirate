using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private EnterEvent _event;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_tag))
                _event.Invoke(other.gameObject);
        }
        
        [Serializable]
        public class EnterEvent : UnityEvent<GameObject>
        {
        }
    }
}