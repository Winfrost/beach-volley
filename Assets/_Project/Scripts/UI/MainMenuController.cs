using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeachVolley.Content;
using BeachVolley.Core;
using BeachVolley.AI;

namespace BeachVolley.UI
{
    /// <summary>
    /// Orchestrates the main menu: collects the player's choices (mode, difficulty, and —
    /// later — characters), builds a MatchConfig, hands it to MatchSession, and loads the
    /// Gameplay scene. Pure policy: it holds NO game logic. The engine that acts on these
    /// choices (apply difficulty, pick player 2's input) lives in GameplayBootstrap.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Characters (temporary defaults — replaced by character select in 5b)")]
        [SerializeField] private CharacterDefinition player1Character;
        [SerializeField] private CharacterDefinition player2Character;

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
            // Sensible defaults: 2 players, and (when 1P is chosen) Medium difficulty.
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
            if (player1Character == null || player2Character == null)
            {
                Debug.LogError("[MainMenuController] Default characters not assigned!", this);
                return;
            }

            MatchSession.Set(new MatchConfig
            {
                player1Character = player1Character,
                player2Character = player2Character,
                mode = selectedMode,
                // Difficulty matters only vs CPU; in 2P leave it null.
                cpuStats = selectedMode == MatchMode.OnePlayerVsCPU ? selectedCpuStats : null,
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
