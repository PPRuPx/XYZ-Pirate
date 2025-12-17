using UnityEngine;

namespace Components
{
    public class SwordsChangeComponent : MonoBehaviour
    {
        [SerializeField] private int _value;

        public void ApplyChange(GameObject target)
        {
            var coinsComponent = target.GetComponent<SwordsComponent>();
            if (coinsComponent != null)
                coinsComponent.ModifySwords(_value);
        }
    }
}