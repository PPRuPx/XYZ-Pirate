using UnityEngine;
using UnityEngine.InputSystem;

namespace Creatures.Hero
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;

        public void OnMovementIA(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            _hero.SetDirection(direction);
        }

        public void OnInteractIA(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.Interact();
        }
    
        public void OnAttackIA(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.Attack();
        }
    
        public void OnThrowIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.StartThrowing();
            }

            if (context.canceled)
            {
                _hero.PerformThrowing();
            }
        }
    }
}