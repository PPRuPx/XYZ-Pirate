using UnityEngine;

namespace Creatures.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] protected float _speed;
        [SerializeField] protected bool _invertX;

        protected Rigidbody2D _rigidbody;
        protected int _direction;

        protected virtual void Start()
        {
            var mod = _invertX ? -1 : 1;
            _direction = mod * transform.lossyScale.x > 0 ? 1 : -1;
            _rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}