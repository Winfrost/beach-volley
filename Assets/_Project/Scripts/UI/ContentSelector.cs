using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// Generic picker for one column of selectable content. Builds a swatch per roster entry,
    /// tracks the selected item, highlights it, updates an optional preview. Subclass with a
    /// concrete type (e.g. CharacterSelector : ContentSelector&lt;CharacterDefinition&gt;) so
    /// Unity serializes the typed roster and Selected returns that type — one logic, many types.
    /// </summary>
    public abstract class ContentSelector<T> : MonoBehaviour where T : ScriptableObject, ISelectableContent
    {
        [Header("Roster")]
        [SerializeField] private T[] roster;

        [Header("Swatch generation")]
        [SerializeField] private ContentSwatch swatchPrefab;
        [SerializeField] private Transform swatchContainer;

        [Header("Preview (optional)")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TMP_Text previewName;

        private readonly List<ContentSwatch> swatches = new();
        private T selected;

        public T Selected => selected;

        private void Start()
        {
            BuildSwatches();
            if (roster != null && roster.Length > 0) Select(roster[0]);
        }

        private void BuildSwatches()
        {
            if (swatchPrefab == null || swatchContainer == null)
            {
                Debug.LogError($"[{GetType().Name}] swatchPrefab or swatchContainer not assigned!", this);
                return;
            }

            foreach (T item in roster)
            {
                ContentSwatch swatch = Instantiate(swatchPrefab, swatchContainer);
                swatch.Setup(item, OnSwatchClicked);
                swatches.Add(swatch);
            }
        }

        // Swatch reports an ISelectableContent; the roster is T[], so the cast back to T is safe.
        private void OnSwatchClicked(ISelectableContent item) => Select((T)item);

        private void Select(T item)
        {
            selected = item;

            foreach (ContentSwatch s in swatches)
                s.SetSelected(ReferenceEquals(s.Content, item));

            if (previewImage != null)
            {
                if (item.Preview != null) { previewImage.sprite = item.Preview; previewImage.color = Color.white; }
                else previewImage.color = item.SwatchColor;
            }
            if (previewName != null) previewName.text = item.DisplayName;
        }
    }
}
