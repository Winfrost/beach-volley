using UnityEngine;
using TMPro;
using BeachVolley.Core;

namespace BeachVolley.UI
{
    /// <summary>
    /// Displays the current score on screen. Listens to GameManager.OnScoreChanged.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The TextMeshPro element that shows the score.")]
        [SerializeField] private TextMeshProUGUI scoreText;

        private bool isSubscribed = false;

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            // Fallback in case GameManager wasn't ready in OnEnable
            if (!isSubscribed)
                TrySubscribe();

            // Initialize display with current score
            if (GameManager.Instance != null)
            {
                UpdateScoreDisplay(GameManager.Instance.ScorePlayer1, GameManager.Instance.ScorePlayer2);
            }
        }

        private void OnDisable()
        {
            if (isSubscribed && GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                isSubscribed = false;
            }
        }

        private void TrySubscribe()
        {
            if (isSubscribed) return;
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            isSubscribed = true;
        }

        private void UpdateScoreDisplay(int scoreP1, int scoreP2)
        {
            if (scoreText != null)
            {
                scoreText.text = $"{scoreP1}  -  {scoreP2}";
            }
        }
    }
}