using System;
using UnityEngine;

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

        public static GameManager Instance { get; private set; }

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
        // UNITY LIFECYCLE
        // ============================================================

        private void Awake()
        {
            // Singleton enforcement: only one instance allowed
            if (Instance != null && Instance != this)
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
            if (Instance == this)
            {
                Instance = null;
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
    }
}