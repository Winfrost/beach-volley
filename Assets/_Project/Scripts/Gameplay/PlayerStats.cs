using UnityEngine;

namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Configuration data for a player. Stats that can be tweaked from the Inspector
    /// without recompiling code. Multiple instances allow different character types.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStats_New", menuName = "BeachVolley/Player Stats", order = 0)]
    public class PlayerStats : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Maximum horizontal speed in units per second.")]
        [Range(1f, 20f)]
        public float moveSpeed = 7f;

        [Header("Jump")]
        [Tooltip("Initial upward velocity when the player jumps.")]
        [Range(5f, 30f)]
        public float jumpForce = 14f;

        [Tooltip("Extra gravity multiplier applied while falling, for snappier jumps.")]
        [Range(1f, 5f)]
        public float fallGravityMultiplier = 2f;

        [Tooltip("Extra gravity multiplier when jumping but the player releases the jump button (variable jump height).")]
        [Range(1f, 5f)]
        public float lowJumpMultiplier = 2.5f;

        [Header("Ground Check")]
        [Tooltip("Radius of the circle used to detect the ground under the player.")]
        [Range(0.05f, 0.5f)]
        public float groundCheckRadius = 0.15f;

        [Tooltip("Vertical offset of the ground check from the player's pivot (negative = below).")]
        public float groundCheckOffsetY = -0.95f;
    }
}