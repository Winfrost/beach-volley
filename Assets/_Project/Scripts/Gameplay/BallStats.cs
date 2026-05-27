using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Configuration data for the ball: reset positions, serve impulse, etc.
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

        [Header("Limits")]
        [Tooltip("Minimum velocity squared below which the ball is considered 'at rest' (helps detect when point is over).")]
        [Range(0f, 2f)]
        public float restVelocitySqr = 0.1f;
    }
}