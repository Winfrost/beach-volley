using UnityEngine;

namespace BeachVolley.Core
{
    /// <summary>
    /// Debug-only listener that logs state changes from the GameManager.
    /// Demonstrates how to subscribe to events. Remove or disable for production.
    /// </summary>
    public class GameStateDebugListener : MonoBehaviour
    {
        private bool isSubscribed = false;

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            // Fallback: if GameManager wasn't ready in OnEnable, try again now.
            // Start runs after all Awake/OnEnable of the scene.
            if (!isSubscribed)
            {
                TrySubscribe();
            }
        }

        private void OnDisable()
        {
            // ALWAYS unsubscribe! Forgetting to do this is the #1 cause of memory leaks
            // and "phantom listener" bugs in Unity.
            if (isSubscribed && GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
                Debug.Log("[DebugListener] Unsubscribed from GameManager.OnStateChanged");
                isSubscribed = false;
            }
        }

        private void TrySubscribe()
        {
            if (isSubscribed)
                return;

            if (GameManager.Instance == null)
            {
                // Don't warn: it's normal that GameManager may not be ready in OnEnable.
                // Start() will retry. If still null at Start, we'll warn then.
                return;
            }

            GameManager.Instance.OnStateChanged += HandleStateChanged;
            isSubscribed = true;
            Debug.Log("[DebugListener] Subscribed to GameManager.OnStateChanged");
        }

        private void HandleStateChanged(GameState previous, GameState current)
        {
            Debug.Log($"[DebugListener] Reacting to state change: {previous} -> {current}");
        }
    }
}