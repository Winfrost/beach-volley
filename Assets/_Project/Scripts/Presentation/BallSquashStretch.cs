using System.Collections;
using UnityEngine;
using BeachVolley.Gameplay;   // <-- aggiunta: per Ball e PlayerIndex

namespace BeachVolley.Presentation   // <-- cambiato da .Gameplay
{
    /// <summary>
    /// Squash &amp; stretch juice for the ball's visual.
    /// Lives on a child "Visual" GameObject (the one with the SpriteRenderer)
    /// so that scaling the sprite NEVER touches the physics collider on the root.
    /// Purely reactive: subscribes to Ball events and animates localScale.
    /// Does one thing — visual feedback. No gameplay logic.
    /// </summary>
    public class BallSquashStretch : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION (tunable, no magic numbers)
        // ============================================================

        [Header("Squash amounts (0 = none, ~0.3 = strong)")]
        [Tooltip("How much the ball FLATTENS when it bounces off the ground.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float groundSquash = 0.28f;

        [Tooltip("How much the ball STRETCHES (pops) when a player hits it.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float hitStretch = 0.22f;

        [Tooltip("How much the ball squishes sideways when it hits the net.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float netSquash = 0.18f;

        [Header("Recovery")]
        [Tooltip("Seconds taken to spring back to the normal shape.")]
        [Range(0.05f, 0.6f)]
        [SerializeField] private float recoverDuration = 0.22f;

        [Tooltip("Spring-back shape. Let the curve rise above 1.0 near the end for a bouncy overshoot.")]
        [SerializeField]
        private AnimationCurve recoverCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private Ball ball;
        private Coroutine activeAnim;
        private static readonly Vector3 NormalScale = Vector3.one;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            ball = GetComponentInParent<Ball>();
            if (ball == null)
                Debug.LogError("[BallSquashStretch] No Ball found in parent! " +
                               "This must live on a child of the Ball.", this);
        }

        private void OnEnable()
        {
            if (ball == null) return;
            ball.OnGroundTouched += HandleGround;
            ball.OnHitByPlayer += HandleHit;
            ball.OnNetTouched += HandleNet;
        }

        private void OnDisable()
        {
            if (ball != null)
            {
                ball.OnGroundTouched -= HandleGround;
                ball.OnHitByPlayer -= HandleHit;
                ball.OnNetTouched -= HandleNet;
            }

            // Never leave the visual stuck mid-squash.
            if (activeAnim != null) StopCoroutine(activeAnim);
            transform.localScale = NormalScale;
        }

        // ============================================================
        // EVENT HANDLERS
        // The shape of the squash depends only on WHICH surface was hit,
        // so the event parameters are intentionally ignored.
        // ============================================================

        private void HandleGround(PlayerIndex _) => Play(1f + groundSquash, 1f - groundSquash);
        private void HandleHit(PlayerIndex _) => Play(1f - hitStretch * 0.5f, 1f + hitStretch);
        private void HandleNet() => Play(1f - netSquash, 1f + netSquash);

        // ============================================================
        // ANIMATION
        // ============================================================

        /// <summary>
        /// Snaps the visual to the given (x, y) scale, then springs back to normal.
        /// Restarts cleanly if a new impact arrives mid-animation.
        /// </summary>
        private void Play(float startX, float startY)
        {
            if (activeAnim != null) StopCoroutine(activeAnim);
            activeAnim = StartCoroutine(SquashRoutine(new Vector3(startX, startY, 1f)));
        }

        private IEnumerator SquashRoutine(Vector3 startScale)
        {
            float elapsed = 0f;
            while (elapsed < recoverDuration)
            {
                float t = recoverCurve.Evaluate(elapsed / recoverDuration);
                // LerpUnclamped so a curve that overshoots 1.0 produces a springy bounce.
                transform.localScale = Vector3.LerpUnclamped(startScale, NormalScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = NormalScale;
            activeAnim = null;
        }
    }
}