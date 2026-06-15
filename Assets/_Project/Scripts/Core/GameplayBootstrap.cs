using System.Collections;
using UnityEngine;
using BeachVolley.Content;

namespace BeachVolley.Core
{
    /// <summary>
    /// Ensures the GameManager exists when this scene starts, applies each player's
    /// CharacterDefinition, then transitions to Playing once the GameManager has
    /// reached its initial MainMenu state. Place one of these in the Gameplay scene.
    ///
    /// The four "Match Setup" fields are a temporary, inline proto-MatchConfig: in a
    /// later step they will be read from a MatchConfig that survives a scene change,
    /// written by the menu. The application code below will not change — only WHERE
    /// the CharacterDefinitions come from.
    /// </summary>
    public class GameplayBootstrap : MonoBehaviour
    {
        // ============================================================
        // MATCH SETUP (temporary — becomes MatchConfig in the menu step)
        // ============================================================

        [Header("Match Setup (temporary — becomes MatchConfig in the menu step)")]
        [Tooltip("The PlayerCharacter component on each player GameObject.")]
        [SerializeField] private PlayerCharacter player1;
        [SerializeField] private PlayerCharacter player2;

        [Tooltip("Which character each player uses for this match.")]
        [SerializeField] private CharacterDefinition player1Character;
        [SerializeField] private CharacterDefinition player2Character;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            // Touching Instance triggers auto-creation if it doesn't exist yet.
            _ = GameManager.Instance;
        }

        private void Start()
        {
            // Apply characters first (all Awakes have already run, so the players'
            // PlayerCharacter components have cached their parts), then start the match.
            ApplyCharacters();
            StartCoroutine(StartMatchWhenReady());
        }

        // ============================================================
        // CHARACTER INJECTION
        // ============================================================

        private void ApplyCharacters()
        {
            if (player1 != null) player1.Apply(player1Character);
            if (player2 != null) player2.Apply(player2Character);
        }

        // ============================================================
        // MATCH START
        // ============================================================

        private IEnumerator StartMatchWhenReady()
        {
            GameManager gm = GameManager.Instance;

            // Wait until the GameManager leaves the Boot state.
            // This handles the case where the GameManager's Start() runs
            // after ours (e.g. when auto-created at runtime).
            while (gm.CurrentState == GameState.Boot)
            {
                yield return null; // wait one frame, then check again
            }

            // Now the GameManager should be in MainMenu. Transition to Playing.
            if (gm.CurrentState == GameState.MainMenu)
            {
                gm.ChangeState(GameState.Playing);
            }
            else
            {
                Debug.Log($"[GameplayBootstrap] GameManager in state {gm.CurrentState}, not forcing Playing.");
            }
        }
    }
}
