namespace BeachVolley.Content
{
    /// <summary>
    /// The DATA that describes a single match: who plays whom (and, later, on which
    /// stage and under which rules). This is the stable "form" of a match setup.
    ///
    /// It is a plain serializable class, NOT a ScriptableObject: a match setup is
    /// transient session state (decided at runtime, valid for one match), not authored
    /// tuning data. The menu writes one; the Gameplay scene reads it via MatchSession.
    /// </summary>
    [System.Serializable]
    public class MatchConfig
    {
        public CharacterDefinition player1Character;
        public CharacterDefinition player2Character;

        // Room to grow without touching the consumer:
        //   public StageDefinition stage;
        //   public MatchRules rules;
        //   public TournamentContext tournament;
    }
}
