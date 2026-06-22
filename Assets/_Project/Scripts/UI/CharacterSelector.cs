using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// Character picker. All behaviour lives in ContentSelector; this subclass only fixes the
    /// type so Unity serializes a CharacterDefinition roster and Selected returns one.
    /// </summary>
    public class CharacterSelector : ContentSelector<CharacterDefinition> { }
}
