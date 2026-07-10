using UnityEngine;
using UnityEngine.UI;

namespace BeachVolley.Presentation
{
    /// <summary>
    /// UI click SFX. Presentation-side reactor: the UI announces a click,
    /// this plays the sound. One per Canvas.
    ///
    /// On Start it wires the click sound to every Button found under this object
    /// (GetComponentsInChildren), routing through SfxPlayer so the tap respects
    /// the SFX mixer group (and therefore the SfxVol slider).
    ///
    /// NOTE: this wires the Buttons that exist at Start. Buttons generated at
    /// runtime (e.g. the content swatches created by the selectors) are NOT
    /// covered by this scan, by design — give those their sound at creation
    /// time if you want it later. Custom in-game touch controls that are not
    /// UnityEngine.UI.Button (e.g. TouchButton) are naturally excluded, so the
    /// move/jump buttons stay silent.
    /// </summary>
    public sealed class UIClickFeedback : MonoBehaviour
    {
        [SerializeField] private AudioClip clickClip;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        private void Start()
        {
            // includeInactive: also wire Buttons inside panels that start disabled
            // (e.g. the win / tournament end-of-match panels).
            Button[] buttons = GetComponentsInChildren<Button>(includeInactive: true);
            foreach (Button button in buttons)
            {
                button.onClick.AddListener(PlayClick);
            }
        }

        private void PlayClick()
        {
            // Read the instance at click time so a scene reload can't leave a stale ref.
            if (SfxPlayer.Instance != null)
            {
                SfxPlayer.Instance.Play(clickClip, volume);
            }
        }
    }
}
