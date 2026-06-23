using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeachVolley.Content;
using BeachVolley.Core;
using BeachVolley.AI;

namespace BeachVolley.UI
{
    /// <summary>
    /// Orchestrates the main menu: collects the player's choices (mode, difficulty, the two
    /// characters, the stage), builds a MatchConfig, hands it to MatchSession, and loads the
    /// Gameplay scene. Pure policy: no game logic.
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

        [Header("Mode buttons")]
        [SerializeField] private Button onePlayerButton;
        [SerializeField] private Button twoPlayersButton;

        [Header("Difficulty UI (shown only in 1P)")]
        [SerializeField] private GameObject difficultyRow;
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;

        [Header("Selection tint (set button Transition = None so this sticks)")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.4f);

        private MatchMode selectedMode = MatchMode.TwoPlayers;
        private AIStats selectedCpuStats;

        private void Start()
        {
            selectedCpuStats = mediumStats;
            ApplyMode(MatchMode.TwoPlayers);
            HighlightDifficulty(mediumButton);
        }

        // ---- Mode ----

        public void OnSelectTwoPlayers() => ApplyMode(MatchMode.TwoPlayers);
        public void OnSelectOnePlayer() => ApplyMode(MatchMode.OnePlayerVsCPU);

        private void ApplyMode(MatchMode mode)
        {
            selectedMode = mode;
            bool cpu = mode == MatchMode.OnePlayerVsCPU;

            if (difficultyRow != null) difficultyRow.SetActive(cpu);

            Tint(onePlayerButton, cpu);
            Tint(twoPlayersButton, !cpu);
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
                mode = selectedMode,
                cpuStats = selectedMode == MatchMode.OnePlayerVsCPU ? selectedCpuStats : null,
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
