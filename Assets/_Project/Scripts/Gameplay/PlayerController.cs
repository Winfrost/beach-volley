using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Controls a player character: horizontal movement and jump.
    /// Reads input from keyboard (Player 1: arrows + Space, Player 2: WASD).
    /// Touch input will be added later via the new Input System.
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

        [Header("Player Identity")]
        [Tooltip("Which player is this? Determines input mapping.")]
        [SerializeField] private PlayerIndex playerIndex = PlayerIndex.Player1;

        [Header("Ground Detection")]
        [Tooltip("LayerMask used to detect what counts as ground.")]
        [SerializeField] private LayerMask groundLayer;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private Rigidbody2D rb;
        private bool isGrounded;
        private float horizontalInput;
        private bool jumpPressedThisFrame;
        private bool jumpHeld;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (stats == null)
            {
                Debug.LogError($"[PlayerController:{name}] PlayerStats not assigned!", this);
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            // Input is read every frame for responsiveness.
            // Physics updates happen in FixedUpdate.
            ReadInput();
        }

        private void FixedUpdate()
        {
            // Physics updates use FixedUpdate for consistent simulation
            // regardless of frame rate.
            CheckGrounded();
            ApplyHorizontalMovement();
            HandleJump();
            ApplyBetterGravity();
        }

        // ============================================================
        // INPUT
        // ============================================================

        private void ReadInput()
        {
            if (playerIndex == PlayerIndex.Player1)
            {
                // Player 1: arrow keys + Space
                horizontalInput = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput -= 1f;
                if (Input.GetKey(KeyCode.RightArrow)) horizontalInput += 1f;

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
                    jumpPressedThisFrame = true;

                jumpHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
            }
            else // Player 2
            {
                // Player 2: A/D + W
                horizontalInput = 0f;
                if (Input.GetKey(KeyCode.A)) horizontalInput -= 1f;
                if (Input.GetKey(KeyCode.D)) horizontalInput += 1f;

                if (Input.GetKeyDown(KeyCode.W))
                    jumpPressedThisFrame = true;

                jumpHeld = Input.GetKey(KeyCode.W);
            }
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
            // Keep current Y velocity intact (jump/fall handled separately).
            Vector2 velocity = rb.linearVelocity;
            velocity.x = horizontalInput * stats.moveSpeed;
            rb.linearVelocity = velocity;
        }

        private void HandleJump()
        {
            if (jumpPressedThisFrame && isGrounded)
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = stats.jumpForce;
                rb.linearVelocity = velocity;
            }

            // Reset the one-frame flag
            jumpPressedThisFrame = false;
        }

        private void ApplyBetterGravity()
        {
            // Falling: apply extra gravity for snappier descent
            if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (stats.fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
            }
            // Rising but jump released: variable jump height (short hop)
            else if (rb.linearVelocity.y > 0f && !jumpHeld)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (stats.lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }

        // ============================================================
        // DEBUG GIZMOS
        // ============================================================

        private void OnDrawGizmosSelected()
        {
            // Visualize the ground check circle in the Scene view
            if (stats == null) return;

            Gizmos.color = Color.green;
            Vector2 checkPosition = (Vector2)transform.position + new Vector2(0f, stats.groundCheckOffsetY);
            Gizmos.DrawWireSphere(checkPosition, stats.groundCheckRadius);
        }
    }

    /// <summary>
    /// Identifies which player a controller belongs to.
    /// Used to determine input mapping.
    /// </summary>
    public enum PlayerIndex
    {
        Player1,
        Player2
    }
}