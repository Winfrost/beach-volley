using System.Collections;
using UnityEngine;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Hit-stop MECHANISM. Knows HOW to freeze the game for a brief moment, not WHEN.
    /// Sets Time.timeScale to 0 for a real-time duration, then restores it.
    /// Reusable and game-agnostic.
    ///
    /// Scene-scoped singleton (NOT DontDestroyOnLoad), same lifetime choice as CameraShake.
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        private Coroutine active;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;

            // Safety: never leave the game frozen if we're destroyed mid-stop.
            // (Same lesson as the rematch flag bug — reset state on every exit path.)
            if (Time.timeScale == 0f) Time.timeScale = 1f;
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        /// <summary>
        /// Freezes the game for the given duration in REAL seconds (immune to timeScale).
        /// A new call restarts the freeze cleanly.
        /// </summary>
        public void Stop(float durationRealSeconds)
        {
            if (durationRealSeconds <= 0f) return;

            if (active != null) StopCoroutine(active);
            active = StartCoroutine(StopRoutine(durationRealSeconds));
        }

        private IEnumerator StopRoutine(float duration)
        {
            Time.timeScale = 0f;

            // Real-time wait: WaitForSecondsRealtime keeps counting while timeScale is 0,
            // whereas a plain WaitForSeconds (scaled) would hang forever here.
            yield return new WaitForSecondsRealtime(duration);

            // Restore to normal speed. NOTE: when you add a pause menu later, that system
            // should own timeScale and hit-stop should be suppressed while paused, so the
            // two don't fight over this value.
            Time.timeScale = 1f;
            active = null;
        }
    }
}
