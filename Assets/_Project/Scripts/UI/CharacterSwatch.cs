using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// One selectable character cell. Shows the character as a colour swatch (a placeholder
    /// for a portrait — later swap colorTarget.color for colorTarget.sprite) plus its name,
    /// and reports clicks back to its selector. Knows which character it represents.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class CharacterSwatch : MonoBehaviour
    {
        [SerializeField] private Image colorTarget;       // tinted with the character's colour
        [SerializeField] private TMP_Text nameLabel;      // optional
        [SerializeField] private GameObject selectedMark; // optional highlight, shown when selected

        private CharacterDefinition character;
        private Action<CharacterDefinition> onClicked;

        public CharacterDefinition Character => character;

        /// <summary>Bind this swatch to a character and a click callback. Called by the selector.</summary>
        public void Setup(CharacterDefinition def, Action<CharacterDefinition> clickCallback)
        {
            character = def;
            onClicked = clickCallback;

            if (colorTarget != null) colorTarget.color = def.tint;
            if (nameLabel != null) nameLabel.text = def.displayName;

            GetComponent<Button>().onClick.AddListener(() => onClicked?.Invoke(character));
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectedMark != null) selectedMark.SetActive(selected);
        }
    }
}
