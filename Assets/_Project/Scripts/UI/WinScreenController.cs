using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeachVolley.Core;
using BeachVolley.Gameplay;
using BeachVolley.Content; // TournamentSession

namespace BeachVolley.UI
{
    /// <summary>
    /// Shows the win screen when the match ends, and handles the rematch button.
    /// Stays out of the way during a tournament: TournamentFlow handles the post-match flow
    /// then, so this controller bails out if a tournament is active.
    /// </summary>
    public class WinScreenController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The panel GameObject shown on match end. Should be inactive at start.")]
        [SerializeField] private GameObject winPanel;

        [Tooltip("Text showing who won.")]
        [SerializeField] private TextMeshProUGUI winnerText;

        [Tooltip("Button to start a rematch.")]
        [SerializeField] private Button rematchButton;

        [Tooltip("The ball, to reset on rematch.")]
        [SerializeField] private Ball ball;

        private bool isSubscribed = false;

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            if (!isSubscribed)
                TrySubscribe();

            if (winPanel != null)
                winPanel.SetActive(false);

            if (rematchButton != null)
                rematchButton.onClick.AddListener(OnRematchClicked);
        }

        private void OnDisable()
        {
            if (isSubscribed && GameManager.Instance != null)
            {
                GameManager.Instance.OnMatchEnded -= HandleMatchEnded;
                isSubscribed = false;
            }

            if (rematchButton != null)
                rematchButton.onClick.RemoveListener(OnRematchClicked);
        }

        private void TrySubscribe()
        {
            if (isSubscribed) return;
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnMatchEnded += HandleMatchEnded;
            isSubscribed = true;
        }

        private void HandleMatchEnded(PlayerIndex winner)
        {
            // During a tournament, TournamentFlow owns the post-match flow.
            if (TournamentSession.IsActive) return;

            if (winPanel != null)
                winPanel.SetActive(true);

            if (winnerText != null)
            {
                string winnerName = winner == PlayerIndex.Player1 ? "Player 1" : "Player 2";
                winnerText.text = $"{winnerName} vince!";
            }
        }

        private void OnRematchClicked()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            GameManager.Instance.ResetMatch();
            GameManager.Instance.ChangeState(GameState.Playing);

            if (ball != null)
                ball.ResetBall(GameManager.Instance.NextServer);
        }
    }
}
