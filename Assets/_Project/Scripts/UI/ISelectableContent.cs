using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// A piece of content the player can pick in a menu (a character, a stage, ...).
    /// Exposes just what a selectable cell needs: a name and a visual — a thumbnail if it has
    /// one, otherwise a flat colour. Lets ONE generic selector/swatch serve every content type.
    /// </summary>
    public interface ISelectableContent
    {
        string DisplayName { get; }
        Color SwatchColor { get; }   // placeholder fill when there is no preview
        Sprite Preview { get; }      // null -> the swatch falls back to SwatchColor
    }
}
