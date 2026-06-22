using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// Applies a StageDefinition's AESTHETICS to the scene: swaps the background and sand
    /// sprites. Mechanism for "dressing" the court; WHICH stage is decided elsewhere
    /// (GameplayBootstrap today, a MatchConfig from the menu later).
    ///
    /// IMPORTANT: this swaps ONLY the SpriteRenderers (the visual skin). The sand renderer
    /// shares its GameObject with the Ground's BoxCollider2D — the collider is the game
    /// geometry and is NEVER touched here. Changing collision is "functional", out of scope
    /// for v1 aesthetic-only stages. (Stage sand sprites should share the same dimensions as
    /// the current one, so the Ground's existing scale/position keep working.)
    /// </summary>
    public class StageDresser : MonoBehaviour
    {
        [Header("Scene renderers to dress")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer sandRenderer; // on the Ground object (collider untouched)

        public void Apply(StageDefinition stage)
        {
            if (stage == null)
            {
                Debug.LogError("[StageDresser] No StageDefinition to apply.", this);
                return;
            }

            if (backgroundRenderer != null && stage.backgroundSprite != null)
                backgroundRenderer.sprite = stage.backgroundSprite;

            if (sandRenderer != null && stage.sandSprite != null)
                sandRenderer.sprite = stage.sandSprite;
        }
    }
}
