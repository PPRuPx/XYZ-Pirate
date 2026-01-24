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