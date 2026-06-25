using UnityEngine;
using TMPro;
using BeachVolley.Core;

namespace BeachVolley.UI
{
    /// <summary>
    /// Shows the player's saved record in the menu. Reads the persistent save on Start, so the
    /// value is visible the moment the game launches — proof that the data survived the restart.
    /// </summary>
    public class RecordDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private void Start()
        {
            SaveData save = SaveSystem.Load();
            if (label != null)
                label.text = $"Tornei vinti: {save.tournamentsWon}    Record: {save.longestStreak}";
        }
    }
}
