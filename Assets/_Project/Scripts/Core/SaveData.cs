namespace BeachVolley.Core
{
    /// <summary>
    /// The player's persistent data — the explicit schema of what survives between sessions.
    /// Only player-owned, changing things belong here (records, and later settings/unlocks).
    /// Content (characters, stages) is NOT saved: it lives in the build.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public int tournamentsWon;   // times the player became champion
        public int longestStreak;    // most opponents beaten in a single run

        // Room to grow when there is something concrete to bind:
        //   public float musicVolume = 1f;
        //   public float sfxVolume = 1f;
        //   public string[] unlockedCharacters;
    }
}
