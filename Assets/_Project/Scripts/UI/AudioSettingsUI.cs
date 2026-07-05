using UnityEngine;
using UnityEngine.UI;
using BeachVolley.Presentation;

namespace BeachVolley.UI
{
    /// <summary>
    /// Binds the two volume sliders to VolumeSettings. One job: keep the UI and the settings
    /// in sync. It holds no volume state and no persistence logic of its own — it reads the
    /// current values from VolumeSettings to initialise the sliders, then forwards changes
    /// back to VolumeSettings, which applies and saves.
    ///
    /// Kept separate from MainMenuController on purpose: that one collects match choices and
    /// builds a MatchConfig; volume is an unrelated concern.
    /// </summary>
    public class AudioSettingsUI : MonoBehaviour
    {
        [Header("Sliders (Min Value 0, Max Value 1, Whole Numbers off)")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            VolumeSettings settings = VolumeSettings.Instance;
            if (settings == null)
            {
                Debug.LogError("[AudioSettingsUI] No VolumeSettings in scene!", this);
                return;
            }

            // Initialise WITHOUT notifying, THEN subscribe. Setting slider.value normally
            // would fire onValueChanged and trigger a redundant save on startup.
            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(settings.MusicVolume);
                musicSlider.onValueChanged.AddListener(OnMusicChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(settings.SfxVolume);
                sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            }
        }

        private void OnDestroy()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        }

        private void OnMusicChanged(float value)
        {
            if (VolumeSettings.Instance != null) VolumeSettings.Instance.SetMusicVolume(value);
        }

        private void OnSfxChanged(float value)
        {
            if (VolumeSettings.Instance != null) VolumeSettings.Instance.SetSfxVolume(value);
        }
    }
}
