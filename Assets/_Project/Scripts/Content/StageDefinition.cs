using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// A stage = the "dressing" of the court. For v1 this is AESTHETIC ONLY: background and
    /// sand sprites. Structured in sections so the future "functional" stage data (geometry,
    /// physics, music) can be added without reshaping the asset — the door is left open, not
    /// a wall built that must be torn down later.
    /// </summary>
    [CreateAssetMenu(fileName = "Stage_", menuName = "Beach Volley/Stage Definition")]
    public class StageDefinition : ScriptableObject
    {
        [Header("Identità")]
        public string displayName = "Nuovo Stadio";
        public Sprite preview;             // for the menu

        [Header("Estetica")]
        public Sprite backgroundSprite;    // sky/sea backdrop
        public Sprite sandSprite;          // the sand STRIP (visual only — never the collider)

        // ---- Future "Gameplay" section — deliberately NOT implemented for v1 ----
        // These would touch the Ground collider / net / physics, i.e. change how the game
        // plays. Aesthetic stages above touch only sprites. When v1 is done:
        //   public AudioClip music;        // needs a music system first (today: SFX only)
        //   public float courtWidth;
        //   public float netHeight;
        //   public PhysicsMaterial2D groundMaterial;
    }
}
