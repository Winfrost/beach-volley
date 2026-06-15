using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.AI
{
    /// <summary>
    /// CPU implementation of IPlayerInput. Produces movement and jump intent by tracking
    /// the ball, so the SAME PlayerController that drives a human drives the CPU.
    /// Lives on a player GameObject in place of KeyboardPlayerInput.
    /// </summary>
    public class AIPlayerInput : MonoBehaviour, IPlayerInput
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Config")]
        [SerializeField] private AIStats stats;

        [Tooltip("The ball to track. Auto-found if left empty.")]
        [SerializeField] private Ball ball;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private float horizontal;
        private bool jumpHeld;
        private bool jumpQueued;

        private float homeX;     // neutral ready position, captured at spawn
        private float mySign;    // +1 = right of net, -1 = left of net (net at X = 0)
        private float jumpTimer; // counts down to the next allowed jump

        public float Horizontal => horizontal;
        public bool JumpHeld => jumpHeld;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (stats == null)
            {
                Debug.LogError("[AIPlayerInput] AIStats not assigned!", this);
                enabled = false;
                return;
            }

            if (ball == null) ball = FindFirstObjectByType<Ball>();
            if (ball == null)
                Debug.LogError("[AIPlayerInput] No Ball found in scene!", this);

            homeX = transform.position.x;
            mySign = Mathf.Sign(Mathf.Approximately(homeX, 0f) ? 1f : homeX);
        }

        // ============================================================
        // BRAIN — called once per frame by PlayerController.Update
        // ============================================================

        public void Tick()
        {
            if (jumpTimer > 0f) jumpTimer -= Time.deltaTime;

            if (ball == null)
            {
                horizontal = 0f;
                jumpHeld = false;
                return;
            }

            float myX = transform.position.x;
            float myY = transform.position.y;
            Vector3 ballPos = ball.transform.position;

            // Ball on my half of the court? (net at X = 0)
            bool ballOnMySide = Mathf.Sign(ballPos.x) == mySign;

            // --- Movement: chase the ball on my side, otherwise return to ready position ---
            float targetX = ballOnMySide ? ballPos.x : homeX;
            float dx = targetX - myX;
            horizontal = Mathf.Abs(dx) <= stats.chaseDeadzone ? 0f : Mathf.Sign(dx);

            // --- Jump: when the ball is overhead, within reach, on my side ---
            float ballDx = ballPos.x - myX;
            float ballDy = ballPos.y - myY;
            bool wantsToJump = ballOnMySide
                               && Mathf.Abs(ballDx) <= stats.hitReachX
                               && ballDy > 0f
                               && ballDy <= stats.jumpTriggerHeight;

            // Hold the jump while reaching, so the controller grants full jump height.
            jumpHeld = wantsToJump;

            if (wantsToJump && jumpTimer <= 0f)
            {
                jumpQueued = true;
                jumpTimer = stats.jumpCooldown;
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
