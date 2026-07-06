using UnityEngine;

namespace BeachVolley.UI
{
    /// <summary>
    /// Fits its RectTransform to the device safe area (avoiding notches and
    /// rounded corners) by converting Screen.safeArea into normalized anchors.
    ///
    /// Working in normalized anchors makes this independent of the Canvas Scaler
    /// reference resolution: only the safe-area proportions matter, not pixels.
    /// On a screen without a cutout, Screen.safeArea covers the whole screen,
    /// so the anchors resolve to 0..1 and nothing moves.
    ///
    /// Usage: put this on an empty full-stretch child of the Canvas and parent
    /// the UI that must stay inside the safe area under it.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rect;

        // Cached so we only recompute when the safe area or screen actually
        // changes (rotation, resolution change, split-screen, foldables).
        private Rect _lastSafeArea = Rect.zero;
        private Vector2Int _lastScreenSize = Vector2Int.zero;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (HasChanged())
            {
                Apply();
            }
        }

        private bool HasChanged()
        {
            return Screen.safeArea != _lastSafeArea
                || Screen.width != _lastScreenSize.x
                || Screen.height != _lastScreenSize.y;
        }

        private void Apply()
        {
            Rect safeArea = Screen.safeArea;
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // Guard against a zero-sized screen (can happen for a frame at init).
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                return;
            }

            // Convert the safe area (in pixels) into normalized anchors [0..1].
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;

            // Stretched anchors + zero offsets => the rect matches the safe area exactly.
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;

            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(screenWidth, screenHeight);
        }
    }
}
