using UnityEngine;
using BeachVolley.Core;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Bridge between persisted settings (SaveData/SaveSystem, Core) and the volume authority
    /// (VolumeController). Owns the current volume VALUES; VolumeController owns HOW to apply
    /// them; SaveSystem owns where they live. Keeping these three responsibilities apart is
    /// why VolumeController does not touch SaveData and SaveData does not touch the mixer.
    ///
    /// At boot it loads the saved volumes and pushes them to the controller. Its public
    /// setters update the value, apply it, and persist it — the SAME setters the B4 UI
    /// sliders will call, so the UI adds no new persistence logic.
    ///
    /// Scene-scoped singleton, lives in MainMenu (scene 0 = app start). The mixer keeps its
    /// runtime values across scene loads, so this need not persist; returning to the menu
    /// simply reloads and reapplies (idempotent).
    /// </summary>
    public class VolumeSettings : MonoBehaviour
    {
        public static VolumeSettings Instance { get; private set; }

        // ============================================================
        // CURRENT VALUES (loaded from save on Start)
        // ============================================================

        [Header("Current (loaded from save on Start; 0 = silent, 1 = full)")]
        [Range(0f, 1f)][SerializeField] private float musicVolume = 1f;
        [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;

        // True only AFTER Start has loaded from disk. Guards the editor test hook (OnValidate)
        // against the enter-Play-mode call that fires BEFORE Start with the stale scene value.
        private bool initialized;

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
        }

        private void Start()
        {
            // Load persisted volumes and apply them. Runs after all Awakes, so
            // VolumeController.Instance is ready.
            SaveData data = SaveSystem.Load();
            musicVolume = Mathf.Clamp01(data.musicVolume);
            sfxVolume = Mathf.Clamp01(data.sfxVolume);

            ApplyToController();

            // From here on, real inspector drags (and B4 sliders) are allowed to persist.
            initialized = true;
        }

        // ============================================================
        // PUBLIC API (used by the B4 sliders; save defaults to true)
        // ============================================================

        public void SetMusicVolume(float value, bool persist = true)
        {
            musicVolume = Mathf.Clamp01(value);
            if (VolumeController.Instance != null)
                VolumeController.Instance.SetMusicVolume(musicVolume);
            if (persist) Persist();
        }

        public void SetSfxVolume(float value, bool persist = true)
        {
            sfxVolume = Mathf.Clamp01(value);
            if (VolumeController.Instance != null)
                VolumeController.Instance.SetSfxVolume(sfxVolume);
            if (persist) Persist();
        }

        // ============================================================
        // INTERNAL
        // ============================================================

        private void ApplyToController()
        {
            if (VolumeController.Instance == null) return;
            VolumeController.Instance.SetMusicVolume(musicVolume);
            VolumeController.Instance.SetSfxVolume(sfxVolume);
        }

        private void Persist()
        {
            SaveData data = SaveSystem.Load();
            data.musicVolume = musicVolume;
            data.sfxVolume = sfxVolume;
            SaveSystem.Save(data);
        }

#if UNITY_EDITOR
        // TEMPORARY verification hook: drag the fields in the Inspector during Play to test
        // the full load/apply/persist round-trip. Replaced by the real UI sliders in B4.
        //
        // The `initialized` guard is essential: OnValidate ALSO fires when entering Play mode,
        // before Start, with the stale scene value (e.g. 1.0). Without the guard that call
        // would persist 1.0 over your saved value before Start could load it.
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (!initialized) return;
            SetMusicVolume(musicVolume);
            SetSfxVolume(sfxVolume);
        }
#endif
    }
}
