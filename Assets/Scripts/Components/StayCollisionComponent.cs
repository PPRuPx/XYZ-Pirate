using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class StayCollisionComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private EnterEvent _action;

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(_tag))
                _action.Invoke(other.gameObject);
        }

        [Serializable]
        public class EnterEvent : UnityEvent<GameObject>
        {
        }
    }
}