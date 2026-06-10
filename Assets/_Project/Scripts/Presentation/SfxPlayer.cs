using UnityEngine;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Sound-effects MECHANISM. Knows HOW to play a one-shot clip, not WHEN or WHICH.
    /// Scene-scoped singleton (same lifetime choice as CameraShake / HitStop).
    ///
    /// Uses PlayOneShot so overlapping impacts LAYER instead of cutting each other off
    /// (in a rally, several hits can sound almost on top of one another).
    /// Audio is driven by its own DSP clock, so it keeps playing during hit-stop
    /// (Time.timeScale = 0) — you hear the impact crack while the screen is frozen.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        public static SfxPlayer Instance { get; private set; }

        private AudioSource source;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Plays a one-shot clip layered on top of anything already playing.
        /// </summary>
        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            source.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
