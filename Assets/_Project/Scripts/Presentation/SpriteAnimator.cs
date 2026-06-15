using UnityEngine;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Simple code-driven sprite animator. Cycles through frame arrays for idle / walk / air,
    /// choosing the state from the parent Rigidbody2D's velocity. No Animator asset and no
    /// sprite-sheet slicing — just arrays of single-frame sprites.
    /// Put it on the player's Visual child (the one with the SpriteRenderer).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Frames (single sprites, in play order)")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] walkFrames;
        [SerializeField] private Sprite[] airFrames;

        [Header("Timing")]
        [Tooltip("Frames per second for cycling idle / walk.")]
        [Range(1f, 24f)]
        [SerializeField] private float fps = 8f;

        [Header("State thresholds (world units / second)")]
        [Tooltip("Horizontal speed above which the player counts as 'walking'.")]
        [Range(0.05f, 3f)]
        [SerializeField] private float walkSpeedThreshold = 0.3f;

        [Tooltip("Vertical speed above which the player counts as 'in the air'.")]
        [Range(0.05f, 3f)]
        [SerializeField] private float airSpeedThreshold = 0.5f;

        [Tooltip("Flip the sprite to face the movement direction (for side-facing art).")]
        [SerializeField] private bool faceMovement = true;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private SpriteRenderer sr;
        private Rigidbody2D body;
        private Sprite[] current;
        private int frameIndex;
        private float timer;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            body = GetComponentInParent<Rigidbody2D>();
            if (body == null)
                Debug.LogError("[SpriteAnimator] No Rigidbody2D in parent — put this on a child of the player.", this);
        }

        private void Update()
        {
            Sprite[] target = SelectState();

            // Restart cleanly when the state changes.
            if (target != current)
            {
                current = target;
                frameIndex = 0;
                timer = 0f;
            }

            Advance();
            UpdateFacing();
        }

        // ============================================================
        // STATE + PLAYBACK
        // ============================================================

        private Sprite[] SelectState()
        {
            Vector2 v = body != null ? body.linearVelocity : Vector2.zero;

            if (Mathf.Abs(v.y) > airSpeedThreshold && HasFrames(airFrames))
                return airFrames;
            if (Mathf.Abs(v.x) > walkSpeedThreshold && HasFrames(walkFrames))
                return walkFrames;
            return HasFrames(idleFrames) ? idleFrames : current;
        }

        private void Advance()
        {
            if (!HasFrames(current)) return;

            if (current.Length == 1)
            {
                sr.sprite = current[0];
                return;
            }

            timer += Time.deltaTime;
            float frameDuration = 1f / fps;
            while (timer >= frameDuration)
            {
                timer -= frameDuration;
                frameIndex = (frameIndex + 1) % current.Length;
            }
            sr.sprite = current[frameIndex];
        }

        private void UpdateFacing()
        {
            if (!faceMovement || body == null) return;

            float vx = body.linearVelocity.x;
            if (Mathf.Abs(vx) > walkSpeedThreshold)
                sr.flipX = vx < 0f;
        }

        private static bool HasFrames(Sprite[] frames) => frames != null && frames.Length > 0;
    }
}
