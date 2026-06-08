using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Drives one-shot impact particle bursts from Ball events.
    ///
    /// Unlike shake/hit-stop, particles need no custom mechanism — Unity's ParticleSystem
    /// IS the mechanism. This component is pure POLICY: it moves a pre-placed system to the
    /// impact point and replays its burst.
    ///
    /// IMPORTANT: each referenced ParticleSystem must use Simulation Space = World and
    /// Play On Awake = OFF. World space keeps already-emitted particles in place when the
    /// system is repositioned for the next burst.
    /// </summary>
    public class ImpactParticleTrigger : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Source")]
        [Tooltip("The ball to listen to. Auto-found if left empty.")]
        [SerializeField] private Ball ball;

        [Header("Particle systems (one-shot, Simulation Space = World)")]
        [Tooltip("Sand puff, played where the ball lands on the ground.")]
        [SerializeField] private ParticleSystem sandPuff;

        [Tooltip("Small spark, played where a player hits the ball.")]
        [SerializeField] private ParticleSystem hitSpark;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (ball == null) ball = FindFirstObjectByType<Ball>();
            if (ball == null)
                Debug.LogError("[ImpactParticleTrigger] No Ball found in scene!", this);
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

        private void HandleGround(PlayerIndex _) => Burst(sandPuff);
        private void HandleHit(PlayerIndex _) => Burst(hitSpark);

        /// <summary>
        /// Repositions a one-shot system to the ball's current position and replays it.
        /// </summary>
        private void Burst(ParticleSystem system)
        {
            if (system == null) return;
            system.transform.position = ball.transform.position;
            system.Play();
        }
    }
}
