using System;
using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Controls a player character: horizontal movement and jump.
    /// Reads INTENT from an IPlayerInput component on the same GameObject — it does not
    /// know or care whether that intent comes from a keyboard, the AI, or touch.
    /// Movement physics (the mechanism) lives here; the input source (the policy) is swappable.
    ///
    /// Announces gameplay moments as events (e.g. OnJumped) but owns NO feedback:
    /// audio/visual reactions live in separate listeners (see PlayerFeedback), mirroring how
    /// Ball announces and ImpactFeedback reacts.
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
        // EVENTS
        // ============================================================

        /// <summary>
        /// Raised the moment this player actually leaves the ground on a jump
        /// (grounded + jump press consumed). Carries no data: it just announces
        /// "this player jumped". Listeners (e.g. PlayerSfx) decide the reaction.
        /// </summary>
        public event Action OnJumped;

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

            // Default input source = whatever single IPlayerInput is on this GameObject.
            // If the GameObject hosts more than one (e.g. Player 2 carries both Keyboard
            // and AI), GameplayBootstrap will override this via SetInput() in Start(),
            // which runs after all Awakes but before the first Update/Tick.
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
        // INJECTION (called by GameplayBootstrap / PlayerCharacter)
        // ============================================================

        /// <summary>
        /// Swap the PlayerStats this controller reads. Because every stat value is read
        /// live (never cached), the change takes effect on the next physics step.
        /// </summary>
        public void SetStats(PlayerStats newStats)
        {
            if (newStats == null) return;
            stats = newStats;
        }

        /// <summary>
        /// Choose which input source drives this player. Used to select keyboard vs AI
        /// for player 2 based on the match mode. Safe to call in Start (before first Tick).
        /// </summary>
        public void SetInput(IPlayerInput newInput)
        {
            if (newInput == null) return;
            input = newInput;
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

                // Announce the jump AFTER it has actually been applied. Listeners react;
                // this controller stays feedback-agnostic.
                OnJumped?.Invoke();
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
