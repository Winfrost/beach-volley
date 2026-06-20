using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// One side's character picker (P1 left / P2 right). Builds a swatch per roster entry at
    /// runtime, tracks the selected character, and updates the highlight + optional preview.
    /// Data-driven: change the roster and the UI follows — no manual per-character wiring.
    /// </summary>
    public class CharacterSelector : MonoBehaviour
    {
        [Header("Roster")]
        [SerializeField] private CharacterDefinition[] roster;

        [Header("Swatch generation")]
        [SerializeField] private CharacterSwatch swatchPrefab;
        [SerializeField] private Transform swatchContainer;

        [Header("Preview (optional)")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TMP_Text previewName;

        private readonly List<CharacterSwatch> swatches = new();
        private CharacterDefinition selected;

        public CharacterDefinition Selected => selected;

        private void Start()
        {
            BuildSwatches();

            // Default selection = first character in the roster.
            if (roster != null && roster.Length > 0) Select(roster[0]);
        }

        private void BuildSwatches()
        {
            if (swatchPrefab == null || swatchContainer == null)
            {
                Debug.LogError("[CharacterSelector] swatchPrefab or swatchContainer not assigned!", this);
                return;
            }

            foreach (CharacterDefinition def in roster)
            {
                CharacterSwatch swatch = Instantiate(swatchPrefab, swatchContainer);
                swatch.Setup(def, Select);
                swatches.Add(swatch);
            }
        }

        private void Select(CharacterDefinition def)
        {
            selected = def;

            foreach (CharacterSwatch s in swatches)
                s.SetSelected(s.Character == def);

            if (previewImage != null) previewImage.color = def.tint;
            if (previewName != null) previewName.text = def.displayName;
        }
    }
}
