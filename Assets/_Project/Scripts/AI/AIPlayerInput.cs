using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.AI
{
    /// <summary>
    /// CPU implementation of IPlayerInput. Produces movement and jump intent by tracking
    /// (and predicting) the ball, so the SAME PlayerController that drives a human drives the CPU.
    /// Lives on a player GameObject alongside (and selected instead of) KeyboardPlayerInput.
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

        private Rigidbody2D ballRb; // for velocity + gravity scale (prediction)
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
            {
                Debug.LogError("[AIPlayerInput] No Ball found in scene!", this);
            }
            else
            {
                ballRb = ball.GetComponent<Rigidbody2D>();
            }

            homeX = transform.position.x;
            mySign = Mathf.Sign(Mathf.Approximately(homeX, 0f) ? 1f : homeX);
        }

        // ============================================================
        // INJECTION (called by GameplayBootstrap)
        // ============================================================

        /// <summary>
        /// Swap the AIStats this brain reads = set the difficulty. Stat values are read
        /// live every Tick, so the new difficulty applies immediately. This is the BRAVURA
        /// axis; the character's PHYSICAL stats (PlayerStats) are untouched.
        /// </summary>
        public void SetStats(AIStats newStats)
        {
            if (newStats == null) return;
            stats = newStats;
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

            // --- Movement target: predicted landing X if possible, else current ball X ---
            float targetX;
            if (stats.usePrediction && TryPredictInterceptX(out float predX))
            {
                bool comingToMySide = Mathf.Sign(predX) == mySign;
                targetX = comingToMySide ? predX : homeX;
            }
            else
            {
                bool ballOnMySide = Mathf.Sign(ballPos.x) == mySign;
                targetX = ballOnMySide ? ballPos.x : homeX;
            }

            float dx = targetX - myX;
            horizontal = Mathf.Abs(dx) <= stats.chaseDeadzone ? 0f : Mathf.Sign(dx);

            // --- Jump: based on the ACTUAL ball position (real proximity), not the prediction ---
            bool ballOnMySideNow = Mathf.Sign(ballPos.x) == mySign;
            float ballDx = ballPos.x - myX;
            float ballDy = ballPos.y - myY;
            bool wantsToJump = ballOnMySideNow
                               && Mathf.Abs(ballDx) <= stats.hitReachX
                               && ballDy > 0f
                               && ballDy <= stats.jumpTriggerHeight;

            jumpHeld = wantsToJump; // hold for full jump height while reaching

            if (wantsToJump && jumpTimer <= 0f)
            {
                jumpQueued = true;
                jumpTimer = stats.jumpCooldown;
            }
        }

        /// <summary>
        /// Ballistic prediction: where (X) the ball crosses the intercept height while descending.
        /// Ignores bounces/hits, but it is recomputed every frame, so it self-corrects after each
        /// collision (new velocity -> new prediction). Returns false if the ball never reaches that
        /// height, or only does so too far in the future / in the past.
        /// </summary>
        private bool TryPredictInterceptX(out float predictedX)
        {
            predictedX = 0f;
            if (ballRb == null) return false;

            float g = Physics2D.gravity.y * ballRb.gravityScale; // negative
            if (Mathf.Approximately(g, 0f)) return false;

            Vector2 p = ball.transform.position;
            Vector2 v = ballRb.linearVelocity;
            float yTarget = transform.position.y + stats.interceptHeight;

            // Solve 0.5*g*t^2 + v.y*t + (p.y - yTarget) = 0 for t.
            float a = 0.5f * g;
            float b = v.y;
            float c = p.y - yTarget;
            float disc = b * b - 4f * a * c;
            if (disc < 0f) return false;

            float sq = Mathf.Sqrt(disc);
            float t1 = (-b + sq) / (2f * a);
            float t2 = (-b - sq) / (2f * a);
            float t = Mathf.Max(t1, t2); // later crossing = descending pass

            if (t <= 0f || t > stats.maxPredictTime) return false;

            predictedX = p.x + v.x * t;
            return true;
        }

        public bool ConsumeJumpPressed()
        {
            if (!jumpQueued) return false;
            jumpQueued = false;
            return true;
        }
    }
}
