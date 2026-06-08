using UnityEngine;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Screen shake MECHANISM. Knows HOW to shake, not WHEN.
    /// Applies a decaying random offset on top of the camera's rest position each LateUpdate.
    /// Reusable and game-agnostic: it has no idea what a ball or a point is.
    ///
    /// Scene-scoped singleton: NOT DontDestroyOnLoad — it belongs to THIS scene's camera,
    /// unlike GameManager which must persist. Different lifetime, deliberate choice.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Feel")]
        [Tooltip("How quickly a shake dies down. Higher = snappier, shorter shakes.")]
        [Range(0.5f, 8f)]
        [SerializeField] private float damping = 3f;

        [Tooltip("Maximum offset (world units) reached by a shake of intensity 1.")]
        [Range(0.05f, 1f)]
        [SerializeField] private float maxOffset = 0.35f;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private Vector3 restPosition;   // camera's normal position (no shake)
        private float currentIntensity; // 0 = still

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            // Scene-scoped singleton: last one in the scene wins, no persistence.
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            // Static camera: capturing rest once is fine. If you later add a camera
            // follow, apply shake on a child transform instead of fighting the follow.
            restPosition = transform.localPosition;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        /// <summary>
        /// Triggers a shake. intensity is clamped to 0..1; the strongest active shake wins
        /// (a fresh strong impact overrides a fading weak one, but not vice versa).
        /// </summary>
        public void Shake(float intensity)
        {
            currentIntensity = Mathf.Max(currentIntensity, Mathf.Clamp01(intensity));
        }

        // ============================================================
        // SHAKE
        // ============================================================

        private void LateUpdate()
        {
            if (currentIntensity <= 0f)
            {
                transform.localPosition = restPosition;
                return;
            }

            Vector2 offset = Random.insideUnitCircle * (currentIntensity * maxOffset);
            transform.localPosition = restPosition + (Vector3)offset;

            currentIntensity = Mathf.MoveTowards(currentIntensity, 0f, damping * Time.deltaTime);
        }
    }
}
