using UnityEngine;
using UnityEngine.InputSystem;

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
        if (context.canceled)
            _hero.Interact();
    }
    
    public void OnAttackIA(InputAction.CallbackContext context)
    {
        if (context.canceled)
            _hero.Attack();
    }
}