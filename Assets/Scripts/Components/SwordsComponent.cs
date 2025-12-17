using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class SwordsComponent : MonoBehaviour
    {
        [SerializeField] private int _swords;
        [SerializeField] private SwordsChangeEvent _onChange;

        public int Swords() =>  _swords;
        
        public void SetSwords(int value) => 
            _swords = value;

        public void ModifySwords(int delta)
        {
            _swords += delta;
            _onChange?.Invoke(_swords);
        }
    }

    [Serializable]
    public class SwordsChangeEvent : UnityEvent<int>
    {
    }
}