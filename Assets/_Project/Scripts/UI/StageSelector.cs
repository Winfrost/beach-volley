using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// Stage picker. All behaviour lives in ContentSelector; this subclass only fixes the
    /// type so Unity serializes a StageDefinition roster and Selected returns one.
    /// Two lines — the payoff of the generic selector.
    /// </summary>
    public class StageSelector : ContentSelector<StageDefinition> { }
}
