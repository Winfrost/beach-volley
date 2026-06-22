using UnityEngine;
using BeachVolley.Gameplay; // PlayerStats
using BeachVolley.AI;       // AIStats

namespace BeachVolley.Content
{
    /// <summary>
    /// Groups a character's display identity (name, portrait, tint) with its gameplay stats
    /// (PlayerStats for a human, AIStats for the CPU). The unit of CONTENT that selection,
    /// tournament and saves all treat the same way. Implements ISelectableContent so the
    /// generic menu selector can list it.
    /// </summary>
    [CreateAssetMenu(fileName = "Character_", menuName = "Beach Volley/Character Definition")]
    public class CharacterDefinition : ScriptableObject, ISelectableContent
    {
        [Header("Identità")]
        public string displayName = "Nuovo Personaggio";
        public Sprite portrait;            // for the menu
        public Color tint = Color.white;   // white = no tint

        [Header("Gameplay")]
        public PlayerStats playerStats;    // when driven by a human
        public AIStats aiStats;            // when driven by the CPU

        // ---- ISelectableContent ----
        public string DisplayName => displayName;
        public Color SwatchColor => tint;
        public Sprite Preview => portrait;
    }
}
