using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeachVolley.Content;
using BeachVolley.Core;
using BeachVolley.AI;

namespace BeachVolley.UI
{
    /// <summary>
    /// Orchestrates the main menu: collects the player's choices (difficulty, the two
    /// characters, the stage), builds a single-match MatchConfig vs CPU, hands it to
    /// MatchSession, and loads the Gameplay scene. Pure policy: no game logic.
    ///
    /// v1 is single-player only: every match built here is OnePlayerVsCPU. Local 2P and
    /// online are parked; the input layer stays abstracted so they can attach later without
    /// touching this policy. MatchMode.TwoPlayers still exists in Core for that future.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Character selection")]
        [SerializeField] private CharacterSelector player1Selector;
        [SerializeField] private CharacterSelector player2Selector;

        [Header("Stage selection")]
        [SerializeField] private StageSelector stageSelector;

        [Header("Difficulty brains")]
        [SerializeField] private AIStats easyStats;
        [SerializeField] private AIStats mediumStats;
        [SerializeField] private AIStats hardStats;

        [Header("Difficulty UI")]
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;

        [Header("Selection tint (set button Transition = None so this sticks)")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.4f);

        private AIStats selectedCpuStats;

        private void Start()
        {
            // Difficulty is always relevant now (every match is vs CPU), so the row stays
            // visible; we just set the default selection.
            selectedCpuStats = mediumStats;
            HighlightDifficulty(mediumButton);
        }

        // ---- Difficulty ----

        public void OnSelectEasy() { selectedCpuStats = easyStats; HighlightDifficulty(easyButton); }
        public void OnSelectMedium() { selectedCpuStats = mediumStats; HighlightDifficulty(mediumButton); }
        public void OnSelectHard() { selectedCpuStats = hardStats; HighlightDifficulty(hardButton); }

        private void HighlightDifficulty(Button selected)
        {
            Tint(easyButton, selected == easyButton);
            Tint(mediumButton, selected == mediumButton);
            Tint(hardButton, selected == hardButton);
        }

        // ---- Start ----

        public void OnPlayPressed()
        {
            CharacterDefinition p1 = player1Selector != null ? player1Selector.Selected : null;
            CharacterDefinition p2 = player2Selector != null ? player2Selector.Selected : null;
            StageDefinition stage = stageSelector != null ? stageSelector.Selected : null;

            if (p1 == null || p2 == null)
            {
                Debug.LogError("[MainMenuController] A character selection is missing!", this);
                return;
            }

            MatchSession.Set(new MatchConfig
            {
                player1Character = p1,
                player2Character = p2,
                mode = MatchMode.OnePlayerVsCPU,
                cpuStats = selectedCpuStats,
                stage = stage, // null is fine -> Gameplay keeps its current sprites
            });

            SceneManager.LoadScene(SceneNames.Gameplay);
        }

        // ---- Helper ----

        private void Tint(Button button, bool selected)
        {
            if (button == null || button.image == null) return;
            button.image.color = selected ? selectedColor : unselectedColor;
        }
    }
}
