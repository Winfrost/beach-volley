using BeachVolley.AI; // AIStats

namespace BeachVolley.Content
{
    /// <summary>
    /// How many humans play — decides whether player 2 is a second human or the CPU.
    /// A structural branch (two distinct setups), hence an enum.
    /// </summary>
    public enum MatchMode
    {
        TwoPlayers,      // both sides are human
        OnePlayerVsCPU   // player 2 is driven by the AI
    }

    /// <summary>
    /// The DATA that describes a single match. The stable "form" of a match setup,
    /// written by the menu (via MatchSession) and read by the Gameplay scene.
    /// Plain serializable class, NOT a ScriptableObject: transient session state.
    /// </summary>
    [System.Serializable]
    public class MatchConfig
    {
        public CharacterDefinition player1Character;
        public CharacterDefinition player2Character;

        // How many humans play. Default = 2 players (both human).
        public MatchMode mode = MatchMode.TwoPlayers;

        // The CPU's "brain" = difficulty. Used ONLY when mode == OnePlayerVsCPU.
        // This is the BRAVURA axis, separate from the character's PHYSICAL stats.
        // null  -> fall back to the character's own default aiStats (field-level
        //          config-or-fallback: "no difficulty chosen" still behaves sanely).
        // asset -> overrides the character's default brain with the chosen difficulty.
        public AIStats cpuStats;

        // Room to grow without touching the consumer:
        //   public StageDefinition stage;
        //   public MatchRules rules;
    }
}
