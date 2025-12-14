using UnityEngine;

namespace Components
{
    public class CoinsChangeComponent : MonoBehaviour
    {
        [SerializeField] private int _value;

        public void ApplyChange(GameObject target)
        {
            var coinsComponent = target.GetComponent<CoinsComponent>();
            if (coinsComponent != null)
                coinsComponent.ModifyCoins(_value);
        }
    }
}