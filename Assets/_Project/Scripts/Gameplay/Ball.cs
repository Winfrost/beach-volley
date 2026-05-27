using System;
using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// The volleyball. Handles physics-driven collisions and broadcasts events.
    /// Does NOT contain scoring logic — it just announces what happened.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Ball : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Stats")]
        [SerializeField] private BallStats stats;

        // ============================================================
        // EVENTS
        // ============================================================

        /// <summary>
        /// Raised when the ball is hit by a player.
        /// Parameter: the PlayerIndex that hit the ball.
        /// </summary>
        public event Action<PlayerIndex> OnHitByPlayer;

        /// <summary>
        /// Raised when the ball touches the ground.
        /// Parameter: which side of the court (PlayerIndex of the player whose half it landed on).
        /// </summary>
        public event Action<PlayerIndex> OnGroundTouched;

        /// <summary>
        /// Raised when the ball hits the net.
        /// </summary>
        public event Action OnNetTouched;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private Rigidbody2D rb;
        private PlayerIndex lastTouchedBy = PlayerIndex.Player1; // arbitrary default
        private bool hasBeenTouched = false; // becomes true after the first hit

        // Public read-only access for other systems
        public PlayerIndex LastTouchedBy => lastTouchedBy;
        public bool HasBeenTouched => hasBeenTouched;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (stats == null)
            {
                Debug.LogError("[Ball] BallStats not assigned!", this);
                enabled = false;
            }
        }

        // ============================================================
        // COLLISION HANDLING
        // ============================================================

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject other = collision.gameObject;

            if (other.CompareTag("Player1"))
            {
                HandlePlayerHit(PlayerIndex.Player1);
            }
            else if (other.CompareTag("Player2"))
            {
                HandlePlayerHit(PlayerIndex.Player2);
            }
            else if (other.CompareTag("Ground"))
            {
                HandleGroundTouch();
            }
            else if (other.CompareTag("Net"))
            {
                HandleNetTouch();
            }
            // Walls: no event needed for now, just physics bounce
        }

        private void HandlePlayerHit(PlayerIndex player)
        {
            lastTouchedBy = player;
            hasBeenTouched = true;
            Debug.Log($"[Ball] Hit by {player}");
            OnHitByPlayer?.Invoke(player);
        }

        private void HandleGroundTouch()
        {
            // Determine which side of the court the ball landed on
            // Negative X = Player1's side, Positive X = Player2's side
            PlayerIndex sideOfCourt = transform.position.x < 0f
                ? PlayerIndex.Player1
                : PlayerIndex.Player2;

            Debug.Log($"[Ball] Touched ground on {sideOfCourt}'s side");
            OnGroundTouched?.Invoke(sideOfCourt);
        }

        private void HandleNetTouch()
        {
            Debug.Log("[Ball] Touched the net");
            OnNetTouched?.Invoke();
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        /// <summary>
        /// Resets the ball above one of the two sides, ready for serve.
        /// </summary>
        /// <param name="servingPlayer">Which player will serve (ball appears above their side).</param>
        public void ResetBall(PlayerIndex servingPlayer)
        {
            float xPos = servingPlayer == PlayerIndex.Player1
                ? -stats.serveXOffset
                : stats.serveXOffset;

            transform.position = new Vector3(xPos, stats.serveY, 0f);
            rb.linearVelocity = new Vector2(0f, -stats.initialServeVelocity);
            rb.angularVelocity = 0f;

            lastTouchedBy = servingPlayer;
            hasBeenTouched = false;

            Debug.Log($"[Ball] Reset for {servingPlayer}'s serve at X={xPos}");
        }
        
        #if UNITY_EDITOR
        private void Update()
        {
            // DEBUG: press R to reset ball for Player1's serve, T for Player2's serve
            if (Input.GetKeyDown(KeyCode.R))
                ResetBall(PlayerIndex.Player1);
            else if (Input.GetKeyDown(KeyCode.T))
                ResetBall(PlayerIndex.Player2);
        }
#endif
    }
}