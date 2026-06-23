using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeachVolley.Content;
using BeachVolley.Core;
using BeachVolley.AI;

namespace BeachVolley.UI
{
    /// <summary>
    /// Starts a gauntlet tournament from the menu: the player's chosen character faces every
    /// other character (CPU) in a random sequence. Builds the run, stores it in TournamentSession,
    /// writes the FIRST match to MatchSession, and loads Gameplay. Pure setup — the per-match
    /// advance/champion/eliminated flow lives in the post-match step (7b).
    /// </summary>
    public class TournamentLauncher : MonoBehaviour
    {
        [Header("Player choice")]
        [SerializeField] private CharacterSelector playerSelector;

        [Header("Tournament cast & pools")]
        [Tooltip("The full character roster. Opponents = everyone except the player's pick.")]
        [SerializeField] private CharacterDefinition[] allCharacters;
        [Tooltip("Difficulty ladder, ASCENDING (easy -> hard).")]
        [SerializeField] private AIStats[] difficultyLadder;
        [Tooltip("Stages cycled across matches for variety.")]
        [SerializeField] private StageDefinition[] stagePool;

        /// <summary>Wire this to the Tournament button's OnClick.</summary>
        public void OnTournamentPressed()
        {
            CharacterDefinition player = playerSelector != null ? playerSelector.Selected : null;
            if (player == null)
            {
                Debug.LogError("[TournamentLauncher] No player character selected!", this);
                return;
            }

            // Opponents = all characters except the player's pick, shuffled.
            List<CharacterDefinition> opponents = new();
            foreach (CharacterDefinition c in allCharacters)
                if (c != null && c != player) opponents.Add(c);

            if (opponents.Count == 0)
            {
                Debug.LogError("[TournamentLauncher] No opponents available (need >= 2 characters).", this);
                return;
            }

            Shuffle(opponents);

            TournamentRun run = new TournamentRun(player, opponents, difficultyLadder, stagePool);
            TournamentSession.Set(run);

            // Launch the first match. The Bootstrap just plays this MatchConfig — it does not
            // know a tournament is in progress.
            MatchSession.Set(run.BuildCurrentMatchConfig());
            SceneManager.LoadScene(SceneNames.Gameplay);
        }

        // Fisher-Yates shuffle.
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
