using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scales a full-bleed UI background sprite so that it always COVERS its parent
/// rect (fill + crop the overflow), instead of Unity's built-in behaviours:
///   - stretch          -> breaks the aspect ratio
///   - Preserve Aspect  -> letterboxes (empty bars at the edges)
///
/// Pixel-art constraint
/// --------------------
/// A naive cover fit produces a fractional scale (e.g. x4.875). With Point
/// filtering that means the sprite's pixels are no longer square on screen and
/// the background shimmers. What matters is not the RectTransform scale on its
/// own, but the FINAL ratio in device pixels:
///
///     devicePixelsPerSpritePixel = uiScale * canvas.scaleFactor
///
/// When <see cref="integerPixelScale"/> is on we round that ratio UP to the next
/// integer, then derive the UI scale back from it. Result: every sprite pixel is
/// an exact NxN block of device pixels on any device. The price is a slightly
/// larger crop, which the background art is designed to absorb (nothing
/// essential lives near its edges).
///
/// Placement: this must live OUTSIDE the SafeArea subtree, so the artwork bleeds
/// under notches and rounded corners. Interactive controls stay inside SafeArea.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class BackgroundCoverFitter : MonoBehaviour
{
    [Header("Source")]
    [Tooltip("Background image. Leave empty to use the Image on this GameObject.")]
    [SerializeField] private Image target;

    [Header("Fit")]
    [Tooltip("Round the device-pixel scale up to an integer, so sprite pixels stay square. " +
             "Turn off only if you accept a shimmering background in exchange for a tighter crop.")]
    [SerializeField] private bool integerPixelScale = true;

    [Tooltip("Never scale below this many device pixels per sprite pixel.")]
    [SerializeField, Min(1)] private int minPixelScale = 1;

    [Header("Framing")]
    [Tooltip("Which part of the artwork survives the crop. 0.5 = centred. " +
             "Lower Y keeps the bottom (sand) visible, higher Y keeps the sky.")]
    [SerializeField] private Vector2 focus = new Vector2(0.5f, 0.5f);

    private RectTransform rt;
    private RectTransform parentRt;
    private Canvas rootCanvas;

    // Cached inputs: we only recompute when one of these actually changes.
    private Vector2 lastParentSize;
    private float lastScaleFactor;
    private Sprite lastSprite;

    private void OnEnable()
    {
        Cache();
        Invalidate();
        Fit();
    }

    private void OnTransformParentChanged()
    {
        Cache();
        Invalidate();
    }

    /// <summary>
    /// Our own rect changing is not enough: the parent stretches with the screen
    /// while this child keeps a fixed size, so the callback would not fire for
    /// the case we care about. We poll instead - it is a menu, the cost is noise.
    /// </summary>
    private void Update()
    {
        Fit();
    }

    private void Cache()
    {
        rt = (RectTransform)transform;
        parentRt = rt.parent as RectTransform;
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null) rootCanvas = rootCanvas.rootCanvas;
        if (target == null) target = GetComponent<Image>();
    }

    private void Invalidate()
    {
        lastParentSize = Vector2.negativeInfinity;
        lastScaleFactor = -1f;
        lastSprite = null;
    }

    private void Fit()
    {
        if (rt == null || target == null) Cache();
        if (rt == null || parentRt == null || target == null || target.sprite == null) return;

        Vector2 parentSize = parentRt.rect.size;
        if (parentSize.x <= 0f || parentSize.y <= 0f) return;

        float scaleFactor = rootCanvas != null ? rootCanvas.scaleFactor : 1f;
        if (scaleFactor <= 0f) scaleFactor = 1f;

        Sprite sprite = target.sprite;

        // Nothing changed since the last frame - skip the work.
        if (sprite == lastSprite
            && parentSize == lastParentSize
            && Mathf.Approximately(scaleFactor, lastScaleFactor))
        {
            return;
        }

        lastSprite = sprite;
        lastParentSize = parentSize;
        lastScaleFactor = scaleFactor;

        // Native size of the artwork, in sprite pixels (480x270 for our menu bg).
        Vector2 native = sprite.rect.size;
        if (native.x <= 0f || native.y <= 0f) return;

        // Cover: take the LARGER of the two ratios, so both axes are filled.
        float uiScale = Mathf.Max(parentSize.x / native.x, parentSize.y / native.y);

        if (integerPixelScale)
        {
            // Round UP in DEVICE pixels, not UI units - the canvas scaleFactor is
            // what turns UI units into real pixels.
            float devicePixelScale = uiScale * scaleFactor;
            int snapped = Mathf.Max(minPixelScale, Mathf.CeilToInt(devicePixelScale - 0.001f));
            uiScale = snapped / scaleFactor;
        }

        Vector2 size = native * uiScale;

        // Centre anchors: sizeDelta IS the size, and anchoredPosition is the crop offset.
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;

        // Shift the artwork so the chosen focus point survives the crop.
        Vector2 overflow = size - parentSize;
        rt.anchoredPosition = new Vector2(
            overflow.x * (0.5f - focus.x),
            overflow.y * (0.5f - focus.y));

        // We do our own aspect handling; Unity's would fight us.
        target.preserveAspect = false;
        target.type = Image.Type.Simple;
        target.raycastTarget = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Do NOT write game state here - OnValidate also fires when entering Play
        // mode. We only invalidate the cache so the next Fit() recomputes.
        focus.x = Mathf.Clamp01(focus.x);
        focus.y = Mathf.Clamp01(focus.y);
        Invalidate();
    }
#endif
}
