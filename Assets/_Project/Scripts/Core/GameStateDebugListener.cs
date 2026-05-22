using UnityEngine;

namespace BeachVolley.Core
{
    /// <summary>
    /// Debug-only listener that logs state changes from the GameManager.
    /// Demonstrates how to subscribe to events. Remove or disable for production.
    /// </summary>
    public class GameStateDebugListener : MonoBehaviour
    {
        private void OnEnable()
        {
            // Subscribe to the GameManager's state change event.
            // We do this in OnEnable (not Awake) so we can also re-subscribe
            // if the GameObject gets disabled and re-enabled.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                Debug.Log("[DebugListener] Subscribed to GameManager.OnStateChanged");
            }
            else
            {
                Debug.LogWarning("[DebugListener] GameManager.Instance is null in OnEnable. " +
                    "Make sure GameManager exists in the scene and runs Awake() first.");
            }
        }

        private void OnDisable()
        {
            // ALWAYS unsubscribe! Forgetting to do this is the #1 cause of memory leaks
            // and "phantom listener" bugs in Unity. If GameManager survives but this
            // listener is destroyed, the event still holds a reference to a dead object.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
                Debug.Log("[DebugListener] Unsubscribed from GameManager.OnStateChanged");
            }
        }

        /// <summary>
        /// Called whenever the GameManager's state changes.
        /// The GameManager doesn't know we exist — it just broadcasts, and we react.
        /// </summary>
        private void HandleStateChanged(GameState previous, GameState current)
        {
            Debug.Log($"[DebugListener] Reacting to state change: {previous} -> {current}");
        }
    }
}