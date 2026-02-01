using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private bool _invulnerability;
        [SerializeField] private float _invulnerabilityOnHitTime;

        [SerializeField] public UnityEvent _onHeal;
        [SerializeField] public UnityEvent _onDamage;
        [SerializeField] public UnityEvent _onDie;
        [SerializeField] public HealthChangeEvent _onChange;
        
        private bool _defaultInvulnerability;

        private void Awake()
        {
            _defaultInvulnerability = _invulnerability;
        }

        public int Health => _health;

        public void SetHealth(int healthValue) =>
            _health = healthValue;
        
        public bool IsInvulnerable() => _invulnerability;
        
        public void SetInvulnerability(bool invulnerability) =>
            _invulnerability = invulnerability;

        public void SetDefaultInvulnerability(bool invulnerability)
        {
            _invulnerability = invulnerability;
            _defaultInvulnerability = invulnerability;
        }

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
        
        public void ResetInvulnerability() =>
            _invulnerability = _defaultInvulnerability;
        
#if UNITY_EDITOR
        [ContextMenu("Update Health")]
        private void UpdateHealth()
        {
            _onChange?.Invoke(_health);
        }
#endif
        
        private void OnDestroy()
        {
            _onDie.RemoveAllListeners();
        }
    }

    [Serializable]
    public class HealthChangeEvent : UnityEvent<int>
    {
    }
}