namespace BeachVolley.Core
{
    /// <summary>
    /// All possible game states. The GameManager is always in exactly one of these.
    /// </summary>
    public enum GameState
    {
        Boot,           // Initial state at app launch
        MainMenu,       // Player is in the main menu
        ModeSelect,     // Player is choosing 1P vs CPU or 2P (future)
        Loading,        // Loading a scene (future)
        Playing,        // A match is in progress
        Paused,         // Match is paused
        PointScored,    // Brief state after a point (replay, celebration)
        MatchEnd        // Match is over, showing winner screen
    }
}