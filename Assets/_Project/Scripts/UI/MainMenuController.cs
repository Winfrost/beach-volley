using UnityEngine;
using UnityEngine.SceneManagement;
using BeachVolley.Content;
using BeachVolley.Core;

namespace BeachVolley.UI
{
    /// <summary>
    /// Drives the main menu. For now: a single "Play" button that builds a MatchConfig
    /// from default characters, hands it to MatchSession, and loads the Gameplay scene.
    ///
    /// The character selection screen will later replace these serialized defaults with
    /// the players' actual choices — but this controller's job (build a config, load the
    /// scene) stays the same. That is the whole point of the MatchSession seam.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Default characters (temporary — replaced by character select in 3b)")]
        [SerializeField] private CharacterDefinition player1Character;
        [SerializeField] private CharacterDefinition player2Character;

        /// <summary>Wire this to the Play button's OnClick event in the Inspector.</summary>
        public void OnPlayPressed()
        {
            if (player1Character == null || player2Character == null)
            {
                Debug.LogError("[MainMenuController] Default characters not assigned!", this);
                return;
            }

            // Hand the match setup to the seam, then load the gameplay scene.
            // The persisted GameManager survives the load; GameplayBootstrap reads
            // this config and transitions MainMenu -> Playing on the other side.
            MatchSession.Set(new MatchConfig
            {
                player1Character = player1Character,
                player2Character = player2Character,
            });

            SceneManager.LoadScene(SceneNames.Gameplay);
        }
    }
}
