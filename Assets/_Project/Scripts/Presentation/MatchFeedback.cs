using UnityEngine;
using BeachVolley.Core;
using BeachVolley.Gameplay;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Match-level feedback COORDINATOR (policy). Subscribes to GameManager's match events
    /// and drives the matching reaction. The third sibling of the feedback family:
    ///   ball events   -> ImpactFeedback
    ///   player events  -> PlayerFeedback
    ///   match events   -> MatchFeedback (this)
    ///
    /// Scene-scoped, but listens to the persistent GameManager singleton. The
    /// OnEnable/OnDisable subscribe/unsubscribe pair keeps it clean across scene reloads
    /// (e.g. a tournament advancing reloads Gameplay): the old instance unsubscribes as its
    /// scene unloads, the fresh one subscribes on load — no accumulation.
    ///
    /// NEUTRAL by design: it reacts to "the match ended", not to "the human won/lost".
    /// The winner-vs-human perspective lives in the config; if a victory/defeat split is
    /// wanted later, GameplayBootstrap can inject the human side and this file chooses
    /// between two clips — an extension at the border, not a rewrite.
    /// </summary>
    public class MatchFeedback : MonoBehaviour
    {
        // ============================================================
        // CONFIGURATION (grouped by event)
        // ============================================================

        [Header("On match end")]
        [Tooltip("Sound played when the match ends, regardless of winner. " +
                 "Neutral by design (e.g. a final whistle). Optional (can be left empty).")]
        [SerializeField] private AudioClip matchEndSound;

        // ============================================================
        // RUNTIME STATE
        // ============================================================

        private GameManager gameManager;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            // Cache the persistent singleton once; the getter finds/creates it if needed.
            gameManager = GameManager.Instance;
            if (gameManager == null)
                Debug.LogError("[MatchFeedback] No GameManager available!", this);
        }

        private void OnEnable()
        {
            if (gameManager != null) gameManager.OnMatchEnded += HandleMatchEnded;
        }

        private void OnDisable()
        {
            if (gameManager != null) gameManager.OnMatchEnded -= HandleMatchEnded;
        }

        // ============================================================
        // EVENT HANDLERS — one place per event, the full response in one read
        // ============================================================

        // Winner is intentionally ignored: the match-end sound is neutral for now.
        private void HandleMatchEnded(PlayerIndex _)
        {
            PlaySound(matchEndSound);
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
