using BeachVolley.AI; // AIStats

namespace BeachVolley.Content
{
    /// <summary>
    /// How many humans play — decides whether player 2 is a second human or the CPU.
    /// </summary>
    public enum MatchMode
    {
        TwoPlayers,      // both sides are human
        OnePlayerVsCPU   // player 2 is driven by the AI
    }

    /// <summary>
    /// The DATA that describes a single match: who plays whom, in which mode, at what
    /// difficulty, and on which stage. Written by the menu (via MatchSession), read by the
    /// Gameplay scene. Plain serializable class, NOT a ScriptableObject: transient session state.
    /// </summary>
    [System.Serializable]
    public class MatchConfig
    {
        public CharacterDefinition player1Character;
        public CharacterDefinition player2Character;

        // How many humans play. Default = 2 players (both human).
        public MatchMode mode = MatchMode.TwoPlayers;

        // The CPU's "brain" = difficulty. Used ONLY when mode == OnePlayerVsCPU.
        // null -> fall back to the character's own default aiStats.
        public AIStats cpuStats;

        // Which stage the match is played on (aesthetic for v1). null -> scene keeps its
        // current sprites (e.g. before the menu offers a stage picker).
        public StageDefinition stage;
    }
}
