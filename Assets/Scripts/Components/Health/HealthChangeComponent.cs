using UnityEngine;

namespace Components.Health
{
    public class HealthChangeComponent : MonoBehaviour
    {
        [SerializeField] private int _value;

        public void ApplyChange(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
                healthComponent.ModifyHealth(_value);
        }
    }
}