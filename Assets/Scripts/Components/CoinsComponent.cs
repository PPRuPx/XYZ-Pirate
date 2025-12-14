using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class CoinsComponent : MonoBehaviour
    {
        [SerializeField] private int _value;
        [SerializeField] private HealthChangeEvent _onChange;

        public void SetCoins(int value)
        {
            _value = value;
        }

        public void ModifyCoins(int delta)
        {
            _value += delta;
            _onChange?.Invoke(_value);
            // Debug.Log("Coins: " + _coins);
        }
    }

    [Serializable]
    public class CoinChangeEvent : UnityEvent<int>
    {
    }
}