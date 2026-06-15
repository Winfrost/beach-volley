using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Keyboard implementation of IPlayerInput (legacy Input Manager).
    /// Player1 scheme: arrows + Space/Up. Player2 scheme: A/D + W.
    /// The control scheme moved here from PlayerController — input mapping is this
    /// component's job, movement is the controller's.
    /// </summary>
    public class KeyboardPlayerInput : MonoBehaviour, IPlayerInput
    {
        [Header("Control Scheme")]
        [Tooltip("Which key set this player uses.")]
        [SerializeField] private PlayerIndex scheme = PlayerIndex.Player1;

        private float horizontal;
        private bool jumpHeld;
        private bool jumpQueued;

        public float Horizontal => horizontal;
        public bool JumpHeld => jumpHeld;

        public void Tick()
        {
            if (scheme == PlayerIndex.Player1)
            {
                horizontal = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
                if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
                    jumpQueued = true;

                jumpHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
            }
            else // Player2
            {
                horizontal = 0f;
                if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
                if (Input.GetKey(KeyCode.D)) horizontal += 1f;

                if (Input.GetKeyDown(KeyCode.W))
                    jumpQueued = true;

                jumpHeld = Input.GetKey(KeyCode.W);
            }
        }

        public bool ConsumeJumpPressed()
        {
            if (!jumpQueued) return false;
            jumpQueued = false;
            return true;
        }
    }
}
