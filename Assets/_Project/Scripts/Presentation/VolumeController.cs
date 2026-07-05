using UnityEngine;
using UnityEngine.Audio;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Volume AUTHORITY. Owns the one true translation from a linear 0..1 volume (what a
    /// slider or a saved setting speaks) into mixer decibels (what the AudioMixer speaks),
    /// and applies it to the exposed mixer parameters.
    ///
    /// The mapping is LOGARITHMIC because hearing is: dB = log10(value) * 20, so
    /// 1 -> 0 dB (full), 0.5 -> ~-6 dB (half perceived power). The value 0 is special-cased:
    /// log10(0) is -infinity, so 0 (and anything at/below a tiny epsilon) is forced to
    /// the mixer floor (-80 dB = silence).
    ///
    /// PURE authority: it knows only HOW to apply a value, never WHICH value. It applies
    /// nothing on its own — VolumeSettings decides the values (from SaveData) and calls the
    /// setters here. Single global service (scene-scoped singleton) so VolumeSettings and the
    /// B4 UI reach it via VolumeController.Instance.
    /// </summary>
    public class VolumeController : MonoBehaviour
    {
        public static VolumeController Instance { get; private set; }

        // ============================================================
        // CONFIGURATION
        // ============================================================

        [Header("Mixer")]
        [Tooltip("The GameAudio mixer asset.")]
        [SerializeField] private AudioMixer mixer;

        [Tooltip("Exposed parameter name for the Music group volume (must match the mixer).")]
        [SerializeField] private string musicParam = "MusicVol";

        [Tooltip("Exposed parameter name for the SFX group volume (must match the mixer).")]
        [SerializeField] private string sfxParam = "SfxVol";

        // Mixer attenuation floor: Unity treats -80 dB as silence.
        private const float MinDecibels = -80f;
        // Below this linear value we are effectively silent (avoids log10(0) = -infinity).
        private const float SilenceThreshold = 0.0001f;

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

            if (mixer == null)
                Debug.LogError("[VolumeController] No AudioMixer assigned!", this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            // Diagnostic only — does NOT apply any value. VolumeSettings drives the actual
            // volumes from SaveData (and its Start runs after all Awakes, so Instance is ready).
            VerifyExposedParams();
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        /// <summary>Set the Music group volume from a linear 0..1 value.</summary>
        public void SetMusicVolume(float linear01) => Apply(musicParam, Mathf.Clamp01(linear01));

        /// <summary>Set the SFX group volume from a linear 0..1 value.</summary>
        public void SetSfxVolume(float linear01) => Apply(sfxParam, Mathf.Clamp01(linear01));

        // ============================================================
        // MAPPING + DISPATCH
        // ============================================================

        private void Apply(string param, float linear01)
        {
            if (mixer == null || string.IsNullOrEmpty(param)) return;

            float db = LinearToDecibels(linear01);

            // SetFloat returns false when the name does not match an exposed parameter.
            // That is the usual reason "the volume doesn't move": a silent no-op.
            bool applied = mixer.SetFloat(param, db);
            if (!applied)
                Debug.LogWarning(
                    $"[VolumeController] SetFloat('{param}') FAILED — parameter not exposed " +
                    $"or name mismatch. {linear01:0.00} -> {db:0.0} dB was NOT applied. " +
                    "Check the mixer's Exposed Parameters (exact name and case).", this);
        }

        /// <summary>
        /// Perceptual mapping: 1 -> 0 dB, 0.5 -> ~-6 dB, 0 -> -80 dB (silence).
        /// </summary>
        private static float LinearToDecibels(float linear01)
        {
            if (linear01 <= SilenceThreshold) return MinDecibels;
            return Mathf.Log10(linear01) * 20f;
        }

        /// <summary>
        /// One-time startup diagnostic: GetFloat returns false if a parameter is not exposed,
        /// so this pinpoints a naming/exposure problem the moment you press Play. Informational.
        /// </summary>
        private void VerifyExposedParams()
        {
            if (mixer == null) return;

            if (!mixer.GetFloat(musicParam, out _))
                Debug.LogWarning(
                    $"[VolumeController] Music param '{musicParam}' NOT found on the mixer. " +
                    "Expose the Music group's Volume and rename it to match (exact name/case).", this);

            if (!mixer.GetFloat(sfxParam, out _))
                Debug.LogWarning(
                    $"[VolumeController] SFX param '{sfxParam}' NOT found on the mixer. " +
                    "Expose the SFX group's Volume and rename it to match (exact name/case).", this);
        }
    }
}
