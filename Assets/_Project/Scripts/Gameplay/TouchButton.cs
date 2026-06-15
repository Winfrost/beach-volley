using UnityEngine;
using UnityEngine.EventSystems;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// A UI button that reports a HELD state via pointer events, so on-screen controls can
    /// drive continuous input (hold to move) instead of single clicks.
    /// Put it on a UI Image/Button under a Canvas (Raycast Target enabled).
    /// Works in the editor with the mouse and on device with touch — same pointer events.
    /// </summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool isPressed;
        private bool pressedThisFrame;

        public bool IsPressed => isPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            pressedThisFrame = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
        }

        private void OnDisable()
        {
            // Safety: never leave a button stuck "pressed" if it's disabled mid-hold.
            isPressed = false;
            pressedThisFrame = false;
        }

        /// <summary>Returns true ONCE if the button was pressed since the last call, then clears it.</summary>
        public bool ConsumePressedThisFrame()
        {
            if (!pressedThisFrame) return false;
            pressedThisFrame = false;
            return true;
        }
    }
}
