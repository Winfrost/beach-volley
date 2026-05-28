using System;
using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Core
{
    /// <summary>
    /// Singleton manager that orchestrates the entire game lifecycle.
    /// Owns the game state machine and broadcasts state changes.
    /// Persists across scene loads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ============================================================
        // SINGLETON
        // ============================================================

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    // Try to find an existing one in the scene
                    instance = FindFirstObjectByType<GameManager>();

                    // If none exists, create one automatically
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager (Auto-Created)");
                        instance = go.AddComponent<GameManager>();
                        Debug.Log("[GameManager] Auto-created because none existed in scene.");
                    }
                }
                return instance;
            }
            private set => instance = value;
        }

        // ============================================================
        // STATE MACHINE
        // ============================================================

        [Header("Debug (read-only at runtime)")]
        [SerializeField] private GameState currentState = GameState.Boot;

        public GameState CurrentState => currentState;

        /// <summary>
        /// Raised whenever the state changes. Subscribers receive (previousState, newState).
        /// </summary>
        public event Action<GameState, GameState> OnStateChanged;

        // ============================================================
        // SCORE
        // ============================================================

        [Header("Match Settings")]
        [Tooltip("Points needed to win the match.")]
        [SerializeField] private int pointsToWin = 7;

        [Header("Score (read-only at runtime)")]
        [SerializeField] private int scorePlayer1 = 0;
        [SerializeField] private int scorePlayer2 = 0;

        public int ScorePlayer1 => scorePlayer1;
        public int ScorePlayer2 => scorePlayer2;
        public int PointsToWin => pointsToWin;

        /// <summary>
        /// Raised whenever the score changes. Parameters: (scoreP1, scoreP2).
        /// </summary>
        public event Action<int, int> OnScoreChanged;

        /// <summary>
        /// Raised when the match ends. Parameter: the winning player.
        /// </summary>
        public event Action<PlayerIndex> OnMatchEnded;

        /// <summary>
        /// Which player serves next. Set after each point.
        /// </summary>
        public PlayerIndex NextServer { get; private set; } = PlayerIndex.Player1;

        // ============================================================
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            // Singleton enforcement: only one instance allowed
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance detected, destroying.");
                Destroy(gameObject);
                return;
            }

            // Ensure this is a root GameObject (required by DontDestroyOnLoad)
            if (transform.parent != null)
            {
                Debug.LogWarning("[GameManager] Was a child of " + transform.parent.name +
                    ", detaching to root for DontDestroyOnLoad to work properly.");
                transform.SetParent(null);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[GameManager] Initialized and persisted across scenes.");
        }

        private void Start()
        {
            // Initial state transition after all Awake() calls have completed
            ChangeState(GameState.MainMenu);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        // ============================================================
        // STATE MACHINE LOGIC
        // ============================================================

        /// <summary>
        /// Requests a state transition. Validates if the transition is allowed,
        /// then notifies subscribers. Logs invalid transitions but does not throw.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (newState == currentState)
            {
                Debug.LogWarning($"[GameManager] Already in state {currentState}, ignoring redundant change.");
                return;
            }

            if (!IsTransitionAllowed(currentState, newState))
            {
                Debug.LogError($"[GameManager] Invalid transition: {currentState} -> {newState}. Ignored.");
                return;
            }

            GameState previous = currentState;
            currentState = newState;

            Debug.Log($"[GameManager] State changed: {previous} -> {newState}");

            OnStateChanged?.Invoke(previous, newState);
        }

        /// <summary>
        /// Defines which state transitions are allowed.
        /// Returns true if going from `from` to `to` is permitted.
        /// </summary>
        private bool IsTransitionAllowed(GameState from, GameState to)
        {
            switch (from)
            {
                case GameState.Boot:
                    return to == GameState.MainMenu;

                case GameState.MainMenu:
                    return to == GameState.ModeSelect
                        || to == GameState.Loading
                        || to == GameState.Playing;

                case GameState.ModeSelect:
                    return to == GameState.MainMenu
                        || to == GameState.Loading
                        || to == GameState.Playing;

                case GameState.Loading:
                    return to == GameState.Playing
                        || to == GameState.MainMenu;

                case GameState.Playing:
                    return to == GameState.Paused
                        || to == GameState.PointScored
                        || to == GameState.MatchEnd
                        || to == GameState.MainMenu;  // emergency quit

                case GameState.Paused:
                    return to == GameState.Playing
                        || to == GameState.MainMenu;

                case GameState.PointScored:
                    return to == GameState.Playing
                        || to == GameState.MatchEnd;

                case GameState.MatchEnd:
                    return to == GameState.MainMenu
                        || to == GameState.Playing;  // rematch

                default:
                    return false;
            }
        }

        // ============================================================
        // SCORE LOGIC
        // ============================================================

        /// <summary>
        /// Awards a point to the specified player and handles match-end detection.
        /// </summary>
        public void AwardPoint(PlayerIndex scoringPlayer)
        {
            // Only award points during active play
            if (CurrentState != GameState.Playing && CurrentState != GameState.PointScored)
            {
                Debug.LogWarning($"[GameManager] AwardPoint called in state {CurrentState}, ignoring.");
                return;
            }

            if (scoringPlayer == PlayerIndex.Player1)
                scorePlayer1++;
            else
                scorePlayer2++;

            // The player who LOST the point serves next (standard volleyball-ish rule for our MVP)
            NextServer = scoringPlayer == PlayerIndex.Player1
                ? PlayerIndex.Player2
                : PlayerIndex.Player1;

            Debug.Log($"[GameManager] Point to {scoringPlayer}. Score: {scorePlayer1} - {scorePlayer2}");

            OnScoreChanged?.Invoke(scorePlayer1, scorePlayer2);

            // Check for match end
            if (scorePlayer1 >= pointsToWin || scorePlayer2 >= pointsToWin)
            {
                PlayerIndex winner = scorePlayer1 >= pointsToWin
                    ? PlayerIndex.Player1
                    : PlayerIndex.Player2;

                Debug.Log($"[GameManager] Match ended. Winner: {winner}");
                ChangeState(GameState.MatchEnd);
                OnMatchEnded?.Invoke(winner);
            }
            else
            {
                // Brief "point scored" state before resuming play
                ChangeState(GameState.PointScored);
            }
        }

        /// <summary>
        /// Resets the score and starts a new match.
        /// </summary>
        public void ResetMatch()
        {
            scorePlayer1 = 0;
            scorePlayer2 = 0;
            NextServer = PlayerIndex.Player1;

            OnScoreChanged?.Invoke(scorePlayer1, scorePlayer2);

            Debug.Log("[GameManager] Match reset.");
        }

    }
}