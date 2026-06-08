using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Hit-stop POLICY. Decides WHEN to freeze, by listening to Ball events and calling
    /// the HitStop mechanism. No freeze logic here — only the decision.
    /// </summary>
    public class HitStopTrigger : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Source")]
        [Tooltip("The ball to listen to. Auto-found if left empty.")]
        [SerializeField] private Ball ball;

        [Header("Freeze durations (real seconds, 0 = off)")]
        [Tooltip("Freeze when the ball lands on the ground (a point). The dramatic moment.")]
        [Range(0f, 0.2f)]
        [SerializeField] private float groundFreeze = 0.06f;

        [Tooltip("Freeze on every player hit. OFF by default: per-hit freezes hurt rally flow.")]
        [Range(0f, 0.2f)]
        [SerializeField] private float hitFreeze = 0f;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (ball == null) ball = FindFirstObjectByType<Ball>();
            if (ball == null)
                Debug.LogError("[HitStopTrigger] No Ball found in scene!", this);
        }

        private void OnEnable()
        {
            if (ball == null) return;
            ball.OnGroundTouched += HandleGround;
            ball.OnHitByPlayer += HandleHit;
        }

        private void OnDisable()
        {
            if (ball == null) return;
            ball.OnGroundTouched -= HandleGround;
            ball.OnHitByPlayer -= HandleHit;
        }

        // ============================================================
        // EVENT HANDLERS
        // ============================================================

        private void HandleGround(PlayerIndex _)
        {
            if (HitStop.Instance != null) HitStop.Instance.Stop(groundFreeze);
        }

        private void HandleHit(PlayerIndex _)
        {
            if (HitStop.Instance != null) HitStop.Instance.Stop(hitFreeze);
        }
    }
}
