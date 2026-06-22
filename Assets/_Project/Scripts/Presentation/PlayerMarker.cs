using UnityEngine;
using BeachVolley.Gameplay; // PlayerIndex

namespace BeachVolley.Presentation
{
    /// <summary>
    /// Shows a small P1/P2 marker above the player, coloured by the player SLOT (not the
    /// character). This is the DISTINCTION channel — separate from the character tint, which
    /// is the IDENTITY channel. So two players who pick the SAME character stay distinguishable.
    ///
    /// Intrinsic to the slot (Player1 is always P1), so it is set in the Inspector and applied
    /// in Awake — it does NOT come from the MatchConfig or the Bootstrap.
    /// </summary>
    public class PlayerMarker : MonoBehaviour
    {
        [Header("Slot")]
        [Tooltip("Which player this GameObject is. Drives the marker colour.")]
        [SerializeField] private PlayerIndex playerIndex;

        [Header("Marker visual")]
        [Tooltip("The SpriteRenderer of the marker child above the head.")]
        [SerializeField] private SpriteRenderer markerRenderer;

        [Header("Slot colours")]
        [SerializeField] private Color player1Color = Color.red;
        [SerializeField] private Color player2Color = new Color(0.2f, 0.4f, 1f); // blue

        private void Awake()
        {
            if (markerRenderer == null)
            {
                Debug.LogError($"[PlayerMarker:{name}] markerRenderer not assigned!", this);
                return;
            }

            markerRenderer.color = playerIndex == PlayerIndex.Player1
                ? player1Color
                : player2Color;
        }
    }
}
