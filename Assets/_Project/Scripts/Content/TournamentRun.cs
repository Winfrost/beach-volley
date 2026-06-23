using System.Collections.Generic;
using UnityEngine;
using BeachVolley.AI; // AIStats

namespace BeachVolley.Content
{
    /// <summary>
    /// The run-state of a single-player "gauntlet" tournament: the player's character and the
    /// shuffled queue of CPU opponents to beat in sequence. Produces the MatchConfig for the
    /// current opponent (difficulty rises along the ladder, stage changes each match). Knows
    /// nothing about scenes or screens — it is pure run logic. Advanced by the post-match flow.
    /// </summary>
    public class TournamentRun
    {
        public CharacterDefinition PlayerCharacter { get; }

        private readonly List<CharacterDefinition> opponents;   // already shuffled
        private readonly AIStats[] difficultyLadder;            // ascending: easy .. hard
        private readonly StageDefinition[] stagePool;
        private readonly int stageOffset;

        public int CurrentIndex { get; private set; }

        public int TotalMatches => opponents.Count;
        public int MatchNumber => CurrentIndex + 1;             // 1-based, for UI
        public bool IsComplete => CurrentIndex >= opponents.Count;
        public CharacterDefinition CurrentOpponent => opponents[CurrentIndex];

        public TournamentRun(CharacterDefinition player,
                             List<CharacterDefinition> shuffledOpponents,
                             AIStats[] difficultyLadder,
                             StageDefinition[] stagePool)
        {
            PlayerCharacter = player;
            opponents = shuffledOpponents;
            this.difficultyLadder = difficultyLadder;
            this.stagePool = stagePool;
            stageOffset = (stagePool != null && stagePool.Length > 0)
                ? Random.Range(0, stagePool.Length)
                : 0;
        }

        /// <summary>The match setup for the opponent the player is currently facing.</summary>
        public MatchConfig BuildCurrentMatchConfig()
        {
            return new MatchConfig
            {
                player1Character = PlayerCharacter,
                player2Character = CurrentOpponent,
                mode = MatchMode.OnePlayerVsCPU,
                cpuStats = DifficultyForStep(CurrentIndex),
                stage = StageForStep(CurrentIndex),
            };
        }

        /// <summary>Move to the next opponent. Call after the player WINS a match.</summary>
        public void Advance() => CurrentIndex++;

        // Difficulty rises so the LAST match is always the hardest, scaled over the ladder.
        private AIStats DifficultyForStep(int step)
        {
            if (difficultyLadder == null || difficultyLadder.Length == 0) return null;
            if (TotalMatches <= 1) return difficultyLadder[difficultyLadder.Length - 1];

            float t = (float)step / (TotalMatches - 1);               // 0..1 across the run
            int idx = Mathf.RoundToInt(t * (difficultyLadder.Length - 1));
            return difficultyLadder[idx];
        }

        // Cycle through the stage pool from a random offset -> different stage each match,
        // and a different sequence each run.
        private StageDefinition StageForStep(int step)
        {
            if (stagePool == null || stagePool.Length == 0) return null;
            return stagePool[(stageOffset + step) % stagePool.Length];
        }
    }
}
