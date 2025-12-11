using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    public class DamageComponent : MonoBehaviour
    {
        [SerializeField] private int _damage;
        [SerializeField] private bool _isHealing;

        public void ApplyDamage(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                if (_isHealing)
                    healthComponent.ApplyHeal(_damage);
                else
                    healthComponent.ApplyDamage(_damage);
            }
        }
    }
}