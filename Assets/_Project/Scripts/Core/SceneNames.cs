namespace BeachVolley.Core
{
    /// <summary>
    /// Centralized scene names. Avoids magic strings scattered across scene-load calls.
    /// IMPORTANT: each value must match BOTH the scene's file name AND its entry in
    /// Build Settings (LoadScene by name requires the scene to be in the build list).
    /// </summary>
    public static class SceneNames
    {
        public const string MainMenu = "MainMenu";
        public const string Gameplay = "Gameplay";
    }
}
