using System.Collections;
using UnityEngine;
using BeachVolley.Content;

namespace BeachVolley.Core
{
    /// <summary>
    /// Composition root for the Gameplay scene. Ensures the GameManager exists, applies
    /// each player's CharacterDefinition, then transitions to Playing once the GameManager
    /// has reached its initial MainMenu state. Place one of these in the Gameplay scene.
    ///
    /// Character source priority:
    ///   1. MatchSession.Pending  — a MatchConfig handed over by the menu / tournament.
    ///   2. The serialized fields below — fallback when booting straight into Gameplay
    ///      (the in-editor test workflow). These fields ARE the inline default config.
    /// The application code does not care which source won: it just applies a MatchConfig.
    /// </summary>
    public class GameplayBootstrap : MonoBehaviour
    {
        // ============================================================
        // MATCH SETUP (fallback config when no MatchSession is pending)
        // ============================================================

        [Header("Players in scene")]
        [Tooltip("The PlayerCharacter component on each player GameObject.")]
        [SerializeField] private PlayerCharacter player1;
        [SerializeField] private PlayerCharacter player2;

        [Header("Fallback characters (used when booting straight into Gameplay)")]
        [Tooltip("Which character each player uses when no menu provided a MatchConfig.")]
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
            // Use the pending config from the menu, or fall back to our own fields.
            MatchConfig config = MatchSession.HasPending
                ? MatchSession.Pending
                : BuildFallbackConfig();

            if (player1 != null) player1.Apply(config.player1Character);
            if (player2 != null) player2.Apply(config.player2Character);
        }

        private MatchConfig BuildFallbackConfig()
        {
            return new MatchConfig
            {
                player1Character = player1Character,
                player2Character = player2Character,
            };
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
