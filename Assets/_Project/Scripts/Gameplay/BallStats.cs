using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Configuration data for the ball: reset positions, serve impulse, hit control, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "BallStats_New", menuName = "BeachVolley/Ball Stats", order = 1)]
    public class BallStats : ScriptableObject
    {
        [Header("Reset / Serve")]
        [Tooltip("Y position where the ball is placed at the start of a serve.")]
        public float serveY = 3f;

        [Tooltip("Horizontal X position offset from center for serve (positive = right side serves).")]
        [Range(2f, 6f)]
        public float serveXOffset = 4f;

        [Tooltip("Initial downward velocity at serve (gentle drop, not thrown).")]
        [Range(0f, 5f)]
        public float initialServeVelocity = 0f;

        [Header("Hit Control")]
        [Tooltip("Speed at which the ball leaves the player after a directional hit. Higher = faster, harder-to-control rallies.")]
        [Range(5f, 25f)]
        public float hitForce = 13f;

        [Tooltip("Maximum launch angle (degrees) measured from straight-up. " +
                 "0 = ball always goes straight up; higher = wider, flatter shots are possible.")]
        [Range(0f, 80f)]
        public float maxHitAngle = 55f;

        [Tooltip("How much the player's horizontal velocity nudges the aim. " +
                 "0 = contact point only; higher = running into the ball steers it more.")]
        [Range(0f, 0.2f)]
        public float playerVelocityInfluence = 0.08f;

        [Header("Limits")]
        [Tooltip("Minimum velocity squared below which the ball is considered 'at rest' (helps detect when point is over).")]
        [Range(0f, 2f)]
        public float restVelocitySqr = 0.1f;
    }
}
