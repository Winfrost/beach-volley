using System.Collections;
using UnityEngine;

namespace BeachVolley.Core
{
    /// <summary>
    /// Ensures the GameManager exists when this scene starts, then transitions
    /// to Playing once the GameManager has reached its initial MainMenu state.
    /// Place one of these in the Gameplay scene.
    /// </summary>
    public class GameplayBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // Touching Instance triggers auto-creation if it doesn't exist yet.
            _ = GameManager.Instance;
        }

        private void Start()
        {
            // Wait until the GameManager has finished its own initialization
            // (reached MainMenu), then transition to Playing.
            StartCoroutine(StartMatchWhenReady());
        }

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