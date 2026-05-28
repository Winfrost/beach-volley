using System.Collections;
using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Core
{
    /// <summary>
    /// Bridges gameplay objects (the Ball) with match logic (the GameManager).
    /// Listens to ball events, awards points, and handles ball reset after a point.
    /// Lives in the Gameplay scene.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The ball in the scene. Assign in Inspector.")]
        [SerializeField] private Ball ball;

        [Header("Timing")]
        [Tooltip("Seconds to wait after a point before resetting the ball for serve.")]
        [Range(0.5f, 4f)]
        [SerializeField] private float pauseAfterPoint = 1.5f;

        private bool isResolvingPoint = false;

        private void OnEnable()
        {
            if (ball != null)
            {
                ball.OnGroundTouched += HandleGroundTouched;
            }
            else
            {
                Debug.LogError("[MatchController] Ball reference not assigned!", this);
            }

            // Subscribe to state changes to reset our flag when play (re)starts
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (ball != null)
            {
                ball.OnGroundTouched -= HandleGroundTouched;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameState previous, GameState current)
        {
            // When we (re)enter Playing from anywhere that isn't a normal point flow,
            // make sure we're ready to resolve the next point.
            // In particular: after a rematch (MatchEnd -> Playing), reset the flag.
            if (current == GameState.Playing && previous == GameState.MatchEnd)
            {
                isResolvingPoint = false;
            }
        }

        /// <summary>
        /// Called when the ball touches the ground.
        /// The OPPOSITE player of the court side scores the point.
        /// </summary>
        private void HandleGroundTouched(PlayerIndex sideOfCourt)
        {
            // Guard: if we're already resolving a point, ignore extra bounces.
            // This fixes the "multiple points per bounce" issue.
            if (isResolvingPoint)
                return;

            // Only score during active play
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

            isResolvingPoint = true;

            // The ball landed on `sideOfCourt`'s half → the OTHER player scores.
            PlayerIndex scoringPlayer = sideOfCourt == PlayerIndex.Player1
                ? PlayerIndex.Player2
                : PlayerIndex.Player1;

            GameManager.Instance.AwardPoint(scoringPlayer);

            // If the match didn't end, schedule a ball reset and resume play.
            if (GameManager.Instance.CurrentState == GameState.PointScored)
            {
                StartCoroutine(ResetBallAfterDelay());
            }
            else
            {
                // Match ended (MatchEnd state). No reset needed, but clear the flag
                // so a future rematch starts clean.
                isResolvingPoint = false;
            }
        }

        private IEnumerator ResetBallAfterDelay()
        {
            // Wait for the configured pause (uses unscaled time so it works
            // even if we later pause the game with Time.timeScale = 0).
            yield return new WaitForSeconds(pauseAfterPoint);

            // Reset the ball above the serving player's side
            PlayerIndex server = GameManager.Instance.NextServer;
            ball.ResetBall(server);

            // Resume play
            GameManager.Instance.ChangeState(GameState.Playing);

            isResolvingPoint = false;
        }
    }
}