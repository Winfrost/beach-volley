using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Player-local feedback COORDINATOR (policy). Subscribes to a PlayerController's
    /// gameplay events and drives the matching reaction.
    ///
    /// The player-scoped sibling of ImpactFeedback: ImpactFeedback reacts to BALL events,
    /// PlayerFeedback reacts to PLAYER events. It reads a Gameplay event and drives a
    /// Presentation mechanism (SfxPlayer today; shake/particles could slot in later).
    ///
    /// One per player, so each slot can carry its own jump sound if desired.
    /// Config is grouped by event so this file answers "what does a player sound/feel like?"
    /// at a glance.
    ///
    /// NOTE: distinct from SfxPlayer. SfxPlayer is the MECHANISM (HOW to play a clip);
    /// PlayerFeedback is the POLICY (WHEN a player event happens, WHICH clip to play).
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerFeedback : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION (grouped by event)
        // ============================================================

        [Header("On jump")]
        [Tooltip("Sound played when this player jumps. Optional (can be left empty).")]
        [SerializeField] private AudioClip jumpSound;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private PlayerController controller;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (controller != null) controller.OnJumped += HandleJump;
        }

        private void OnDisable()
        {
            if (controller != null) controller.OnJumped -= HandleJump;
        }

        // ============================================================
        // EVENT HANDLERS — one place per event, the full response in one read
        // ============================================================

        private void HandleJump()
        {
            PlaySound(jumpSound);
        }

        // ============================================================
        // DISPATCH TO MECHANISM (thin, null-safe wrapper)
        // ============================================================

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && SfxPlayer.Instance != null)
                SfxPlayer.Instance.Play(clip);
        }
    }
}
