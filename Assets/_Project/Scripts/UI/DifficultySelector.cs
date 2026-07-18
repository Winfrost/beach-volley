using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Concrete UnityEvent so the difficulty callback shows up in the Inspector.
/// (A bare UnityEvent&lt;int&gt; is NOT serialized/visible without this subclass.)
/// </summary>
[System.Serializable]
public class DifficultyChangedEvent : UnityEvent<int> { }

/// <summary>
/// Radio-style selector for the three sand-castle difficulty buttons
/// (0 = Facile, 1 = Medio, 2 = Difficile). Exactly one is selected at a time.
///
/// Scope: this component owns only the SELECTION STATE and its visual feedback.
/// It is deliberately agnostic about how difficulty is consumed by the game:
/// it emits the chosen index through <see cref="onDifficultyChanged"/>, which
/// you wire (in the Inspector) to whatever sink DifficultyRow_G used before.
/// That keeps the menu art decoupled from MatchConfig / the difficulty enum.
///
/// Audio: the castle buttons are normal Buttons, so the existing UIClickFeedback
/// plays the click SFX for them - nothing to do here.
/// </summary>
[DisallowMultipleComponent]
public class DifficultySelector : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        [Tooltip("The castle button.")]
        public Button button;
        [Tooltip("Sprite to tint for the selected/deselected look. Usually the button's own Image.")]
        public Image castle;
        [Tooltip("Caption under the castle (Facile / Medio / Difficile). Kept as a SIBLING, " +
                 "so it is never scaled and stays crisp.")]
        public TMP_Text label;
    }

    [Header("Options (order = index: 0 Facile, 1 Medio, 2 Difficile)")]
    [SerializeField] private Option[] options;

    [Header("Startup")]
    [Tooltip("Which castle is selected when the menu opens.")]
    [SerializeField] private int defaultIndex = 1;
    [Tooltip("Fire onDifficultyChanged once at Start, so the game receives the default " +
             "difficulty even if the player never taps a castle. Turn off if your config " +
             "already defaults on its own and you want to avoid a double-set.")]
    [SerializeField] private bool notifyOnStart = true;

    [Header("Selected look (pure tint by default -> fully crisp)")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color deselectedColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    [SerializeField] private Color selectedLabelColor = Color.white;
    [SerializeField] private Color deselectedLabelColor = new Color(0.70f, 0.70f, 0.70f, 1f);

    [Header("Optional scale pop")]
    [Tooltip("Leave both at 1 for pixel-perfect castles. Set deselected < 1 for a 'pop' on the " +
             "selected one, accepting that non-selected castles use a non-integer scale (slight " +
             "softening). Scale pivots on the RectTransform pivot: set castle pivots to (0.5, 0) " +
             "so they grow from the sand line, not the centre.")]
    [SerializeField, Range(0.5f, 1.2f)] private float selectedScale = 1f;
    [SerializeField, Range(0.5f, 1.2f)] private float deselectedScale = 1f;

    [Header("Output")]
    [SerializeField] private DifficultyChangedEvent onDifficultyChanged;

    /// <summary>Currently selected index, or -1 before the first selection.</summary>
    public int Current { get; private set; } = -1;

    private void Awake()
    {
        // Wire each button to its own index. No external dependencies touched here,
        // so this is safe regardless of script execution order (load-order discipline).
        // Do NOT also wire onClick in the Inspector, or selection fires twice.
        if (options == null) return;
        for (int i = 0; i < options.Length; i++)
        {
            int index = i; // capture per iteration
            var btn = options[i].button;
            if (btn == null) continue;
            btn.transition = Selectable.Transition.None; // selection tint is manual, not the Button's
            btn.onClick.AddListener(() => Select(index));
        }
    }

    private void Start()
    {
        // Apply the initial selection now that all refs are live. Optionally notify
        // the game so the default difficulty propagates without a user tap.
        SelectInternal(Mathf.Clamp(defaultIndex, 0, SafeLength - 1), notifyOnStart);
    }

    /// <summary>Select by index and notify listeners. Wired to the buttons' onClick.</summary>
    public void Select(int index) => SelectInternal(index, notify: true);

    /// <summary>Select without firing the event (e.g. when restoring a saved value).</summary>
    public void SelectWithoutNotify(int index) => SelectInternal(index, notify: false);

    private void SelectInternal(int index, bool notify)
    {
        if (options == null || index < 0 || index >= options.Length) return;
        Current = index;

        for (int i = 0; i < options.Length; i++)
            ApplyVisual(i, i == index);

        if (notify) onDifficultyChanged?.Invoke(index);
    }

    private void ApplyVisual(int i, bool selected)
    {
        var o = options[i];
        if (o == null) return;

        if (o.castle != null)
            o.castle.color = selected ? selectedColor : deselectedColor;

        if (o.button != null)
        {
            float s = selected ? selectedScale : deselectedScale;
            o.button.transform.localScale = new Vector3(s, s, 1f);
        }

        if (o.label != null)
            o.label.color = selected ? selectedLabelColor : deselectedLabelColor;
    }

    private int SafeLength => options != null ? options.Length : 0;
}
