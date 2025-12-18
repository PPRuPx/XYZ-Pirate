using UnityEngine;

namespace Components.GameObjectBased
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class InitialAcceleration : MonoBehaviour
    {
        [SerializeField] private Vector2 _vector;

        private Rigidbody2D _rigidbody;
    
        private void Awake()
        { 
            _rigidbody = GetComponent<Rigidbody2D>();
        }
    
        void Start()
        {
            _rigidbody.AddForce(_vector, ForceMode2D.Impulse);
        }
    }
}
