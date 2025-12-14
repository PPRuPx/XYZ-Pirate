using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private bool _invulnerability;

        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        public void SetHealth(int healthValue)
        {
            _health = healthValue;
        }

        public void ModifyHealth(int delta)
        {
            _health += delta;
            _onChange?.Invoke(_health);
            
            if (delta < 0)
                _onDamage?.Invoke();
            
            if (delta > 0)
                _onHeal?.Invoke();
            
            if (_health <= 0)
                _onDie?.Invoke();
        }

        public bool IsInvulnerable() => _invulnerability;
    }

    [Serializable]
    public class HealthChangeEvent : UnityEvent<int>
    {
    }
}