using UnityEngine;

namespace Creatures.Weapons
{
    public class Projectile : BaseProjectile
    {
        protected override void Start()
        {
            base.Start();
            
            var force = new Vector2(_direction * _speed, 0);
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }
    }
}