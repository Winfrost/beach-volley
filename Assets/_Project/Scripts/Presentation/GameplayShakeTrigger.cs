using UnityEditor.Rendering.LookDev;
using UnityEngine;
using BeachVolley.Gameplay;   // <-- aggiunta: per Ball e PlayerIndex

namespace BeachVolley.Presentation   // <-- cambiato da .Gameplay
{
    /// <summary>
    /// Screen shake POLICY. Decides WHEN to shake and HOW HARD by listening to
    /// Ball events and calling the CameraShake mechanism. No shake math lives here —
    /// only game-side decisions. Add more triggers (serve, match point) here later
    /// without ever touching CameraShake.
    /// </summary>
    public class GameplayShakeTrigger : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Source")]
        [Tooltip("The ball to listen to. Auto-found if left empty.")]
        [SerializeField] private Ball ball;

        [Header("Shake intensities (0 = off, 1 = max)")]
        [Tooltip("Shake when a player hits the ball. Keep subtle — it fires every rally.")]
        [Range(0f, 1f)]
        [SerializeField] private float hitShake = 0.25f;

        [Tooltip("Shake when the ball lands on the ground (a point is scored).")]
        [Range(0f, 1f)]
        [SerializeField] private float groundShake = 0.6f;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (ball == null) ball = FindFirstObjectByType<Ball>();
            if (ball == null)
                Debug.LogError("[GameplayShakeTrigger] No Ball found in scene!", this);
        }

        private void OnEnable()
        {
            if (ball == null) return;
            ball.OnHitByPlayer += HandleHit;
            ball.OnGroundTouched += HandleGround;
        }

        private void OnDisable()
        {
            if (ball == null) return;
            ball.OnHitByPlayer -= HandleHit;
            ball.OnGroundTouched -= HandleGround;
        }

        // ============================================================
        // EVENT HANDLERS (parameters ignored: only the event TYPE sets the intensity)
        // ============================================================

        private void HandleHit(PlayerIndex _)
        {
            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(hitShake);
        }

        private void HandleGround(PlayerIndex _)
        {
            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(groundShake);
        }
    }
}
