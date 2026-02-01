using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Creatures.Hero
{
    public class InputEnableComponent : MonoBehaviour
    {

        private PlayerInput _input;
        
        private void Start()
        {
            _input = FindObjectOfType<PlayerInput>();
        }

        public void SetInput(bool isEnabled)
        {
            _input.enabled = isEnabled;
        }
    }
}