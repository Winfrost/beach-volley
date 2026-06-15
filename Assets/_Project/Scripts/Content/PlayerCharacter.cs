using UnityEngine;
using BeachVolley.Gameplay; // PlayerController

namespace BeachVolley.Content
{
    /// <summary>
    /// Applies a CharacterDefinition to this player's parts: pushes the PlayerStats
    /// onto the PlayerController and the tint onto the visual SpriteRenderer.
    /// This is the MECHANISM for "wearing" a character; WHICH character is decided
    /// elsewhere (GameplayBootstrap today, a MatchConfig later).
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCharacter : MonoBehaviour
    {
        private PlayerController controller;
        private SpriteRenderer visual;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            visual = GetComponentInChildren<SpriteRenderer>(); // the Visual child
        }

        /// <summary>Apply a character definition. Safe to call after Awake.</summary>
        public void Apply(CharacterDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogError($"[PlayerCharacter:{name}] No CharacterDefinition to apply.", this);
                return;
            }

            if (definition.playerStats != null)
                controller.SetStats(definition.playerStats);

            if (visual != null)
                visual.color = definition.tint;
        }
    }
}
