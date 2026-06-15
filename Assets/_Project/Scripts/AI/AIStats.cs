using UnityEngine;

namespace BeachVolley.AI
{
    /// <summary>
    /// Configuration for the CPU player. Tweakable from the Inspector, like PlayerStats/BallStats.
    /// Multiple assets allow different difficulty profiles (e.g. AIStats_Easy, AIStats_Hard).
    /// </summary>
    [CreateAssetMenu(fileName = "AIStats_New", menuName = "BeachVolley/AI Stats", order = 2)]
    public class AIStats : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Stop moving when within this X distance of the target (prevents jitter at the target).")]
        [Range(0.02f, 0.5f)]
        public float chaseDeadzone = 0.12f;

        [Header("Prediction")]
        [Tooltip("If on, the CPU moves to where the ball WILL be (ballistic), not where it is now.")]
        public bool usePrediction = true;

        [Tooltip("Height above the player's centre at which the CPU aims to intercept the ball.")]
        [Range(0f, 3f)]
        public float interceptHeight = 1f;

        [Tooltip("Ignore predictions further than this many seconds into the future.")]
        [Range(0.5f, 5f)]
        public float maxPredictTime = 3f;

        [Header("Hitting")]
        [Tooltip("Horizontal distance within which the CPU attempts to jump and hit the ball.")]
        [Range(0.2f, 1.5f)]
        public float hitReachX = 0.7f;

        [Tooltip("How high above the player the ball can be and still trigger a jump.")]
        [Range(0.5f, 5f)]
        public float jumpTriggerHeight = 2.5f;

        [Tooltip("Minimum seconds between jump attempts (prevents bouncing on the spot).")]
        [Range(0.1f, 1.5f)]
        public float jumpCooldown = 0.4f;
    }
}
