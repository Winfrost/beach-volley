using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Controls a player character: horizontal movement and jump.
    /// Reads INTENT from an IPlayerInput component on the same GameObject — it does not
    /// know or care whether that intent comes from a keyboard, the AI, or touch.
    /// Movement physics (the mechanism) lives here; the input source (the policy) is swappable.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Stats (ScriptableObject)")]
        [Tooltip("Reference to the PlayerStats asset. Assign in Inspector.")]
        [SerializeField] private PlayerStats stats;

        [Header("Ground Detection")]
        [Tooltip("LayerMask used to detect what counts as ground.")]
        [SerializeField] private LayerMask groundLayer;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private Rigidbody2D rb;
        private IPlayerInput input;
        private bool isGrounded;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<IPlayerInput>();

            if (stats == null)
            {
                Debug.LogError($"[PlayerController:{name}] PlayerStats not assigned!", this);
                enabled = false;
                return;
            }

            if (input == null)
            {
                Debug.LogError($"[PlayerController:{name}] No IPlayerInput component found! " +
                               "Add KeyboardPlayerInput (or another source) to this GameObject.", this);
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            // Refresh the intent from whatever source is attached.
            input.Tick();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyHorizontalMovement();
            HandleJump();
            ApplyBetterGravity();
        }

        // ============================================================
        // STATS INJECTION
        // ============================================================

        /// <summary>
        /// Swap the PlayerStats this controller reads. Because every stat value is read
        /// live (never cached), the change takes effect on the next physics step.
        /// Called by PlayerCharacter when a CharacterDefinition is applied.
        /// </summary>
        public void SetStats(PlayerStats newStats)
        {
            if (newStats == null) return;
            stats = newStats;
        }

        // ============================================================
        // GROUND CHECK
        // ============================================================

        private void CheckGrounded()
        {
            Vector2 checkPosition = (Vector2)transform.position + new Vector2(0f, stats.groundCheckOffsetY);
            isGrounded = Physics2D.OverlapCircle(checkPosition, stats.groundCheckRadius, groundLayer);
        }

        // ============================================================
        // MOVEMENT
        // ============================================================

        private void ApplyHorizontalMovement()
        {
            // Direct velocity assignment for snappy arcade feel (no momentum).
            Vector2 velocity = rb.linearVelocity;
            velocity.x = input.Horizontal * stats.moveSpeed;
            rb.linearVelocity = velocity;
        }

        private void HandleJump()
        {
            // ConsumeJumpPressed() is evaluated first so the queued jump is always cleared.
            // (Swap the operands — isGrounded && ConsumeJumpPressed() — to get jump buffering:
            //  a press just before landing would then fire on touchdown. Behaviour change, opt-in.)
            if (input.ConsumeJumpPressed() && isGrounded)
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = stats.jumpForce;
                rb.linearVelocity = velocity;
            }
        }

        private void ApplyBetterGravity()
        {
            // Falling: extra gravity for a snappier descent.
            if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (stats.fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
            }
            // Rising but jump released: variable jump height (short hop).
            else if (rb.linearVelocity.y > 0f && !input.JumpHeld)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (stats.lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }

        // ============================================================
        // DEBUG GIZMOS
        // ============================================================

        private void OnDrawGizmosSelected()
        {
            if (stats == null) return;

            Gizmos.color = Color.green;
            Vector2 checkPosition = (Vector2)transform.position + new Vector2(0f, stats.groundCheckOffsetY);
            Gizmos.DrawWireSphere(checkPosition, stats.groundCheckRadius);
        }
    }

    /// <summary>
    /// Identifies which player a controller belongs to. Used for input mapping and side logic.
    /// </summary>
    public enum PlayerIndex
    {
        Player1,
        Player2
    }
}
