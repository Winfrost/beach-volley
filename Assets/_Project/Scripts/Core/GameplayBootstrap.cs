using System.Collections;
using UnityEngine;
using BeachVolley.Content;
using BeachVolley.Gameplay;
using BeachVolley.AI;

namespace BeachVolley.Core
{
    /// <summary>
    /// Composition root for the Gameplay scene. Ensures the GameManager exists, applies each
    /// player's character, selects player 2's input from the match mode, dresses the stage,
    /// then transitions to Playing once the GameManager has reached MainMenu.
    ///
    /// Config priority: MatchSession.Pending (menu) else the serialized fallback fields below
    /// (boot-direct test workflow). The application code is identical either way.
    /// </summary>
    public class GameplayBootstrap : MonoBehaviour
    {
        // ============================================================
        // PLAYERS IN SCENE
        // ============================================================

        [Header("Players in scene")]
        [SerializeField] private PlayerCharacter player1;
        [SerializeField] private PlayerCharacter player2;

        [Header("Stage")]
        [SerializeField] private StageDresser stageDresser;

        // ============================================================
        // FALLBACK CONFIG (used when no MatchSession is pending)
        // ============================================================

        [Header("Fallback characters (boot-direct testing)")]
        [SerializeField] private CharacterDefinition player1Character;
        [SerializeField] private CharacterDefinition player2Character;

        [Header("Fallback match settings (boot-direct testing)")]
        [SerializeField] private MatchMode fallbackMode = MatchMode.OnePlayerVsCPU;
        [SerializeField] private AIStats fallbackCpuStats;
        [SerializeField] private StageDefinition fallbackStage;

        // ============================================================
        // RESOLVED PLAYER 2 PARTS (its input depends on the match mode)
        // ============================================================

        private PlayerController p2Controller;
        private KeyboardPlayerInput p2Keyboard;
        private AIPlayerInput p2Ai;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            _ = GameManager.Instance;

            if (player2 != null)
            {
                p2Controller = player2.GetComponent<PlayerController>();
                p2Keyboard = player2.GetComponent<KeyboardPlayerInput>();
                p2Ai = player2.GetComponent<AIPlayerInput>();
            }
        }

        private void Start()
        {
            MatchConfig config = MatchSession.HasPending
                ? MatchSession.Pending
                : BuildFallbackConfig();

            ApplyCharacters(config);
            ConfigurePlayer2Input(config);
            ApplyStage(config);

            StartCoroutine(StartMatchWhenReady());
        }

        // ============================================================
        // CONFIG
        // ============================================================

        private MatchConfig BuildFallbackConfig()
        {
            return new MatchConfig
            {
                player1Character = player1Character,
                player2Character = player2Character,
                mode = fallbackMode,
                cpuStats = fallbackCpuStats,
                stage = fallbackStage,
            };
        }

        // ============================================================
        // APPLICATION
        // ============================================================

        private void ApplyCharacters(MatchConfig config)
        {
            if (player1 != null) player1.Apply(config.player1Character);
            if (player2 != null) player2.Apply(config.player2Character);
        }

        private void ConfigurePlayer2Input(MatchConfig config)
        {
            if (p2Controller == null)
            {
                Debug.LogError("[GameplayBootstrap] Player 2 has no PlayerController.", this);
                return;
            }

            if (config.mode == MatchMode.OnePlayerVsCPU)
            {
                if (p2Ai == null)
                {
                    Debug.LogError("[GameplayBootstrap] 1P mode but Player 2 has no AIPlayerInput.", this);
                    return;
                }

                AIStats brain = config.cpuStats != null
                    ? config.cpuStats
                    : (config.player2Character != null ? config.player2Character.aiStats : null);

                if (brain != null) p2Ai.SetStats(brain);
                p2Controller.SetInput(p2Ai);
            }
            else // TwoPlayers
            {
                if (p2Keyboard == null)
                {
                    Debug.LogError("[GameplayBootstrap] 2P mode but Player 2 has no KeyboardPlayerInput.", this);
                    return;
                }

                p2Controller.SetInput(p2Keyboard);
            }
        }

        private void ApplyStage(MatchConfig config)
        {
            // null stage -> leave the scene's current sprites (e.g. before the menu picks a stage).
            if (stageDresser != null && config.stage != null)
                stageDresser.Apply(config.stage);
        }

        // ============================================================
        // MATCH START
        // ============================================================

        private IEnumerator StartMatchWhenReady()
        {
            GameManager gm = GameManager.Instance;

            while (gm.CurrentState == GameState.Boot)
            {
                yield return null;
            }

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
