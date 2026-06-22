using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// One selectable cell for ANY content (character, stage, ...). Shows a thumbnail if the
    /// item has one, otherwise a flat colour swatch, plus the name; reports clicks back to its
    /// selector. Generic by virtue of working on ISelectableContent, not a concrete type.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ContentSwatch : MonoBehaviour
    {
        [SerializeField] private Image colorTarget;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private GameObject selectedMark;

        private ISelectableContent content;
        private Action<ISelectableContent> onClicked;

        public ISelectableContent Content => content;

        public void Setup(ISelectableContent item, Action<ISelectableContent> clickCallback)
        {
            content = item;
            onClicked = clickCallback;

            if (colorTarget != null)
            {
                if (item.Preview != null)
                {
                    colorTarget.sprite = item.Preview;
                    colorTarget.color = Color.white; // show the thumbnail untinted
                }
                else
                {
                    colorTarget.color = item.SwatchColor;
                }
            }

            if (nameLabel != null) nameLabel.text = item.DisplayName;

            GetComponent<Button>().onClick.AddListener(() => onClicked?.Invoke(content));
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectedMark != null) selectedMark.SetActive(selected);
        }
    }
}
