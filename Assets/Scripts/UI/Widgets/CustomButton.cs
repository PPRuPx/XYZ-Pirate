using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
    public class CustomButton : Button
    {
        [SerializeField] private GameObject _normal;
        [SerializeField] private GameObject _pressed;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            
            bool isPressedOrDisabled = state == SelectionState.Pressed || state == SelectionState.Disabled;
            
            _normal.SetActive(!isPressedOrDisabled);
            _pressed.SetActive(isPressedOrDisabled);
        }
    }
}