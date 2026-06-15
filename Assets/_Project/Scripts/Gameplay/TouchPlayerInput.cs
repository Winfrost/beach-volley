using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Touch implementation of IPlayerInput, driven by on-screen TouchButtons.
    /// Lives on a player GameObject in place of KeyboardPlayerInput (for mobile builds).
    /// Same contract as keyboard and AI — the PlayerController doesn't know the difference.
    /// </summary>
    public class TouchPlayerInput : MonoBehaviour, IPlayerInput
    {
        [Header("On-screen buttons")]
        [SerializeField] private TouchButton leftButton;
        [SerializeField] private TouchButton rightButton;
        [SerializeField] private TouchButton jumpButton;

        private float horizontal;
        private bool jumpHeld;
        private bool jumpQueued;

        public float Horizontal => horizontal;
        public bool JumpHeld => jumpHeld;

        public void Tick()
        {
            float h = 0f;
            if (leftButton != null && leftButton.IsPressed) h -= 1f;
            if (rightButton != null && rightButton.IsPressed) h += 1f;
            horizontal = h;

            jumpHeld = jumpButton != null && jumpButton.IsPressed;

            if (jumpButton != null && jumpButton.ConsumePressedThisFrame())
                jumpQueued = true;
        }

        public bool ConsumeJumpPressed()
        {
            if (!jumpQueued) return false;
            jumpQueued = false;
            return true;
        }
    }
}
