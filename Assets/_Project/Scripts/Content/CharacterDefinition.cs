using UnityEngine;
using BeachVolley.Gameplay; // PlayerStats
using BeachVolley.AI;       // AIStats

namespace BeachVolley.Content
{
    [CreateAssetMenu(
        fileName = "Character_",
        menuName = "Beach Volley/Character Definition")]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identità")]
        public string displayName = "Nuovo Personaggio";
        public Sprite portrait;            // per il menu della Fase 3
        public Color tint = Color.white;   // bianco = nessun tint

        [Header("Gameplay")]
        public PlayerStats playerStats;    // quando lo guida un umano
        public AIStats aiStats;            // quando lo guida la CPU
    }
}