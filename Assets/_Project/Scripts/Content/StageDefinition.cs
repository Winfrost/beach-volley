using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// A stage = the "dressing" of the court. v1 is AESTHETIC ONLY: background and sand
    /// sprites. Structured in sections so future "functional" stage data (geometry, physics,
    /// music) can be added without reshaping the asset. Implements ISelectableContent so the
    /// generic menu selector can list it alongside characters.
    /// </summary>
    [CreateAssetMenu(fileName = "Stage_", menuName = "Beach Volley/Stage Definition")]
    public class StageDefinition : ScriptableObject, ISelectableContent
    {
        [Header("Identità")]
        public string displayName = "Nuovo Stadio";
        public Sprite preview;                 // menu thumbnail (optional)
        public Color swatchColor = Color.white; // menu chip colour when there is no preview

        [Header("Estetica")]
        public Sprite backgroundSprite;        // sky/sea backdrop
        public Sprite sandSprite;              // the sand STRIP (visual only — never the collider)

        // ---- Future "Gameplay" section — deliberately NOT implemented for v1 ----
        //   public AudioClip music;            // needs a music system first (today: SFX only)
        //   public float courtWidth;
        //   public float netHeight;
        //   public PhysicsMaterial2D groundMaterial;

        // ---- ISelectableContent ----
        public string DisplayName => displayName;
        public Color SwatchColor => swatchColor;
        public Sprite Preview => preview;
    }
}
