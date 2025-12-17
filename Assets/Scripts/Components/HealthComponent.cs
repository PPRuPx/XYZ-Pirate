using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private bool _invulnerability;
        [SerializeField] private float _invulnerabilityOnHitTime;

        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        private bool _defaultInvulnerability;

        private void Awake()
        {
            _defaultInvulnerability = _invulnerability;
        }

        public int Health() => _health;

        public void SetHealth(int healthValue) =>
            _health = healthValue;
        
        public bool IsInvulnerable() => _invulnerability;
        
        public void SetInvulnerability(bool invulnerability) =>
            _invulnerability = invulnerability;

        public void ModifyHealth(int delta)
        {
            if (delta < 0 && _invulnerability) 
                return;

            _health += delta;
            _onChange?.Invoke(_health);

            if (delta < 0 && !_invulnerability)
            {
                _onDamage?.Invoke();
                if (_invulnerabilityOnHitTime > 0)
                {
                    _invulnerability = true;
                    Invoke(nameof(ResetInvulnerability), _invulnerabilityOnHitTime);
                }
            }

            if (delta > 0)
                _onHeal?.Invoke();
            
            if (_health <= 0)
                _onDie?.Invoke();
        }
        
        private void ResetInvulnerability() =>
            _invulnerability = _defaultInvulnerability;
    }

    [Serializable]
    public class HealthChangeEvent : UnityEvent<int>
    {
    }
}