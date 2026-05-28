using UnityEngine;
using BeachVolley.Gameplay;

namespace BeachVolley.Core
{
    /// <summary>
    /// Bridges gameplay objects (the Ball) with match logic (the GameManager).
    /// Listens to ball events and translates them into score changes.
    /// Lives in the Gameplay scene.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The ball in the scene. Assign in Inspector.")]
        [SerializeField] private Ball ball;

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
        }

        private void OnDisable()
        {
            if (ball != null)
            {
                ball.OnGroundTouched -= HandleGroundTouched;
            }
        }

        /// <summary>
        /// Called when the ball touches the ground.
        /// The OPPOSITE player of the court side scores the point.
        /// </summary>
        private void HandleGroundTouched(PlayerIndex sideOfCourt)
        {
            // The ball landed on `sideOfCourt`'s half → the OTHER player scores.
            PlayerIndex scoringPlayer = sideOfCourt == PlayerIndex.Player1
                ? PlayerIndex.Player2
                : PlayerIndex.Player1;

            GameManager.Instance.AwardPoint(scoringPlayer);
        }
    }
}