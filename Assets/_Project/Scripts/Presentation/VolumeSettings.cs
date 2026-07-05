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
    /// Load happens in Awake (no external deps) so any reader — e.g. the slider UI — sees the
    /// correct value in its own Start, regardless of Start ordering. Apply happens in Start,
    /// when VolumeController.Instance is guaranteed ready.
    ///
    /// The public setters update the value, apply it, and persist it — the same setters the
    /// menu sliders (AudioSettingsUI) call.
    ///
    /// Scene-scoped singleton, lives in MainMenu (scene 0 = app start). The mixer keeps its
    /// runtime values across scene loads, so this need not persist; returning to the menu
    /// simply reloads and reapplies (idempotent).
    /// </summary>
    public class VolumeSettings : MonoBehaviour
    {
        public static VolumeSettings Instance { get; private set; }

        // ============================================================
        // CURRENT VALUES (loaded from save in Awake)
        // ============================================================

        [Header("Current (loaded from save; 0 = silent, 1 = full)")]
        [Range(0f, 1f)][SerializeField] private float musicVolume = 1f;
        [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;

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

            // Load here (no dependency on VolumeController) so readers see correct values
            // in Start regardless of Start order.
            SaveData data = SaveSystem.Load();
            musicVolume = Mathf.Clamp01(data.musicVolume);
            sfxVolume = Mathf.Clamp01(data.sfxVolume);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            // Apply to the mixer here: after all Awakes, VolumeController.Instance is ready.
            ApplyToController();
        }

        // ============================================================
        // PUBLIC API (used by the menu sliders; save defaults to true)
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
    }
}
