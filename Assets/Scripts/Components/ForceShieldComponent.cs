using System;
using Components.Health;
using UnityEngine;
using Utils;

namespace Components
{
    public class ForceShieldComponent : MonoBehaviour
    {
        [SerializeField] private HealthComponent _health;
        [SerializeField] private Cooldown _cooldown;

        public void Use()
        {
            _health.SetInvulnerability(true);
            _cooldown.Reset();
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_cooldown.IsReady)
                gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _health.ResetInvulnerability();
        }
    }
}