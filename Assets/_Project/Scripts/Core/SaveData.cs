namespace BeachVolley.Core
{
    /// <summary>
    /// The player's persistent data — the explicit schema of what survives between sessions.
    /// Only player-owned, changing things belong here (records, settings, and later unlocks).
    /// Content (characters, stages) is NOT saved: it lives in the build.
    ///
    /// Field initializers below double as the retrocompat baseline: SaveSystem.Load uses
    /// FromJsonOverwrite on a fresh instance, so any field MISSING from an older save.json
    /// keeps the default set here (e.g. volumes stay at 1, not 0 = muted).
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public int tournamentsWon;   // times the player became champion
        public int longestStreak;    // most opponents beaten in a single run

        // Settings (0 = silent, 1 = full). Defaults matter: see class summary.
        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        // Room to grow when there is something concrete to bind:
        //   public string[] unlockedCharacters;
    }
}
