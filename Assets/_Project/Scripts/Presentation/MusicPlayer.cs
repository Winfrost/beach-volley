using UnityEngine;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Background-music MECHANISM. Knows WHAT is playing (one looping track), not HOW LOUD
    /// (that is the mixer's job, driven later by a VolumeController). Sibling in spirit to
    /// SfxPlayer, but PERSISTENT: it survives scene loads so the music does not restart when
    /// moving MainMenu <-> Gameplay.
    ///
    /// Persistent singleton (root + DontDestroyOnLoad), unlike the scene-scoped SfxPlayer.
    /// The singleton dedup means a MusicPlayer placed in more than one scene is safe: the
    /// first one persists and keeps playing; any later duplicate destroys itself on load,
    /// so the track continues seamlessly.
    ///
    /// One track for every scene in v1. Per-stage / per-menu music is a future extension
    /// (StageDefinition already reserves a commented music slot) — added at the border,
    /// not architected now.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer Instance { get; private set; }

        [Header("Track")]
        [Tooltip("Looping background track. Optional (can be left empty).")]
        [SerializeField] private AudioClip track;

        private AudioSource source;

        private void Awake()
        {
            // Singleton dedup: keep the first, destroy any later duplicate (e.g. the copy
            // that lives in the next scene). This is what keeps the music seamless.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Must be a root GameObject for DontDestroyOnLoad to work.
            if (transform.parent != null) transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            source = GetComponent<AudioSource>();
            source.loop = true;
            source.playOnAwake = false;

            if (track != null)
            {
                source.clip = track;
                source.Play();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
