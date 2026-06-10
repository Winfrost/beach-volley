using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Central impact-feedback COORDINATOR (policy).
    /// Subscribes ONCE to the Ball's events and dispatches the full feel response for each
    /// one to the underlying mechanisms: CameraShake, HitStop, impact ParticleSystems, SfxPlayer.
    ///
    /// Config is grouped BY EVENT, so this single file answers
    /// "what happens when the ball is hit / lands?" at a glance.
    ///
    /// The mechanisms are untouched — only the policy grows. BallSquashStretch stays
    /// separate on purpose: it is ball-local visual, not scene-level feedback.
    /// </summary>
    public class ImpactFeedback : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION (grouped by event, not by effect)
        // ============================================================

        [Header("Source")]
        [Tooltip("The ball to listen to. Auto-found if left empty.")]
        [SerializeField] private Ball ball;

        [Header("On player hit")]
        [Tooltip("Camera shake intensity (0 = off, 1 = max). Subtle: fires on every rally touch.")]
        [Range(0f, 1f)]
        [SerializeField] private float hitShake = 0.25f;

        [Tooltip("Freeze duration in real seconds (0 = off). Per-hit freezes hurt rally flow.")]
        [Range(0f, 0.2f)]
        [SerializeField] private float hitFreeze = 0f;

        [Tooltip("Spark played at the contact point. Optional (can be left empty).")]
        [SerializeField] private ParticleSystem hitSpark;

        [Tooltip("Sound played when a player hits the ball. Optional (can be left empty).")]
        [SerializeField] private AudioClip hitSound;

        [Header("On ball landing (point)")]
        [Tooltip("Camera shake intensity (0 = off, 1 = max). The decisive moment.")]
        [Range(0f, 1f)]
        [SerializeField] private float groundShake = 0.6f;

        [Tooltip("Freeze duration in real seconds (0 = off). Punctuates the point.")]
        [Range(0f, 0.2f)]
        [SerializeField] private float groundFreeze = 0.06f;

        [Tooltip("Sand puff played where the ball lands. Optional (can be left empty).")]
        [SerializeField] private ParticleSystem sandPuff;

        [Tooltip("Sound played when the ball lands (a point). Optional (can be left empty).")]
        [SerializeField] private AudioClip pointSound;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (ball == null) ball = FindFirstObjectByType<Ball>();
            if (ball == null)
                Debug.LogError("[ImpactFeedback] No Ball found in scene!", this);
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
        // EVENT HANDLERS — one place per event, the full response in one read
        // ============================================================

        private void HandleHit(PlayerIndex _)
        {
            Shake(hitShake);
            Freeze(hitFreeze);
            Burst(hitSpark);
            PlaySound(hitSound);
        }

        private void HandleGround(PlayerIndex _)
        {
            Shake(groundShake);
            Freeze(groundFreeze);
            Burst(sandPuff);
            PlaySound(pointSound);
        }

        // ============================================================
        // DISPATCH TO MECHANISMS (thin, null-safe wrappers)
        // ============================================================

        private void Shake(float intensity)
        {
            if (intensity > 0f && CameraShake.Instance != null)
                CameraShake.Instance.Shake(intensity);
        }

        private void Freeze(float duration)
        {
            if (duration > 0f && HitStop.Instance != null)
                HitStop.Instance.Stop(duration);
        }

        private void Burst(ParticleSystem system)
        {
            if (system == null) return;
            system.transform.position = ball.transform.position;
            system.Play();
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && SfxPlayer.Instance != null)
                SfxPlayer.Instance.Play(clip);
        }
    }
}
