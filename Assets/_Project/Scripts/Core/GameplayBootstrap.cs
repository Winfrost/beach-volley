using System.Collections;
using UnityEngine;
using BeachVolley.Content;
using BeachVolley.Gameplay;
using BeachVolley.AI;

namespace BeachVolley.Core
{
    /// <summary>
    /// Composition root for the Gameplay scene. Ensures the GameManager exists, applies each
    /// player's character, selects the human input source per platform (touch on mobile,
    /// keyboard on desktop) and player 2's input from the match mode, dresses the stage,
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
        // INPUT PLATFORM
        // ============================================================

        [Header("Input platform (human player)")]
        [Tooltip("Auto = touch on mobile, keyboard elsewhere. " +
                 "ForceTouch/ForceKeyboard override the runtime check so either path " +
                 "can be exercised in the editor, where isMobilePlatform is always false.")]
        [SerializeField] private InputPlatformMode inputPlatform = InputPlatformMode.Auto;

        [Tooltip("Container holding the on-screen touch buttons. Shown only when touch is " +
                 "the active input, hidden on keyboard so the buttons don't clutter desktop.")]
        [SerializeField] private GameObject touchControls;

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
        // RESOLVED PLAYER 1 PARTS (its input depends on the platform)
        // ============================================================

        private PlayerController p1Controller;
        private KeyboardPlayerInput p1Keyboard;
        private TouchPlayerInput p1Touch;

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

            if (player1 != null)
            {
                p1Controller = player1.GetComponent<PlayerController>();
                p1Keyboard = player1.GetComponent<KeyboardPlayerInput>();
                p1Touch = player1.GetComponent<TouchPlayerInput>();
            }

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
            ConfigurePlayer1Input();
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

        /// <summary>
        /// Player 1 is always human. Pick touch on mobile, keyboard on desktop, and show the
        /// on-screen buttons only when touch is the active source. The mode (1P/2P) doesn't
        /// matter here: P1 is a person either way.
        /// </summary>
        private void ConfigurePlayer1Input()
        {
            if (p1Controller == null)
            {
                Debug.LogError("[GameplayBootstrap] Player 1 has no PlayerController.", this);
                return;
            }

            bool useTouch = ResolveUseTouch();

            if (useTouch)
            {
                if (p1Touch == null)
                {
                    Debug.LogError("[GameplayBootstrap] Touch requested but Player 1 has no TouchPlayerInput.", this);
                    return;
                }

                p1Controller.SetInput(p1Touch);
            }
            else
            {
                if (p1Keyboard == null)
                {
                    Debug.LogError("[GameplayBootstrap] Keyboard requested but Player 1 has no KeyboardPlayerInput.", this);
                    return;
                }

                p1Controller.SetInput(p1Keyboard);
            }

            // Single source of truth for "is touch active" drives button visibility too.
            if (touchControls != null) touchControls.SetActive(useTouch);
        }

        /// <summary>
        /// Resolve the human input source from the serialized mode, falling back to the
        /// runtime platform check. Centralised so the decision lives in one place.
        /// </summary>
        private bool ResolveUseTouch()
        {
            switch (inputPlatform)
            {
                case InputPlatformMode.ForceTouch: return true;
                case InputPlatformMode.ForceKeyboard: return false;
                default: return Application.isMobilePlatform; // Auto
            }
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

    /// <summary>
    /// How GameplayBootstrap chooses the human player's input source.
    /// Auto follows the runtime platform; the Force* modes let you exercise either
    /// path in the editor, where Application.isMobilePlatform is always false.
    /// </summary>
    public enum InputPlatformMode
    {
        Auto,
        ForceTouch,
        ForceKeyboard
    }
}