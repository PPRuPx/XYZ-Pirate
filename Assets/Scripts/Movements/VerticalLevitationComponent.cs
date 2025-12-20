using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movements
{
    public class VerticalLevitationComponent : MonoBehaviour
    {
        [SerializeField] private float _frequency = 1;
        [SerializeField] private float _amplitude = 1;
        [SerializeField] private bool _randomize;
        
        private float _originalY;
        private float _seed;
        
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _originalY = _rigidbody.position.y;

            if (_randomize)
                _seed = Random.value * Mathf.PI * 2;
        }

        private void Update()
        {
            var pos = _rigidbody.position;
            pos.y = _originalY + Mathf.Sin(_seed + Time.time * _frequency) * _amplitude;
            _rigidbody.MovePosition(pos);
        }
    }
}