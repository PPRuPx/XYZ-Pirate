using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class CoinsComponent : MonoBehaviour
    {
        [SerializeField] private int _coins;
        [SerializeField] private HealthChangeEvent _onChange;

        public int Coins() =>  _coins;
        
        public void SetCoins(int value) => 
            _coins = value;

        public void ModifyCoins(int delta)
        {
            _coins += delta;
            _onChange?.Invoke(_coins);
        }
    }

    [Serializable]
    public class CoinChangeEvent : UnityEvent<int>
    {
    }
}