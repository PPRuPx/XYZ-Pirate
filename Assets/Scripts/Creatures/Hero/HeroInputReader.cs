using UnityEngine;
using UnityEngine.InputSystem;

namespace Creatures.Hero
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private float _doubleTapThreshold = 0.3f;
        
        private float _lastTapTime;
        private float _lastDirectionX;

        public void OnMovementIA(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();

            if (context.started && direction.x != 0)
            {
                float currentDir = Mathf.Sign(direction.x);
                float timeSinceLastTap = Time.time - _lastTapTime;

                if (currentDir == _lastDirectionX && timeSinceLastTap < _doubleTapThreshold)
                    _hero.Dash(direction.x);

                _lastTapTime = Time.time;
                _lastDirectionX = currentDir;
            }

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
        
        public void OnUseHealPotionIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UseHealPotion();
            }
        }
        
        public void OnUseJumpPotionIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UseJumpPotion();
            }
        }
        
        public void OnUseRecoveryPotionIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UseRecoveryPotion();
            }
        }
        
        public void OnNextItemIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.NextItem();
            }
        }
        
        public void OnUseInventoryItemIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UseInventoryItem();
            }
        }
        
        public void OnUsePerkIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UsePerk();
            }
        }
        
        public void OnLightIA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _hero.UseLight();
            }
        }
    }
}