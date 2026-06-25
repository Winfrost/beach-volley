using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BeachVolley.Core;
using BeachVolley.Gameplay;
using BeachVolley.Content;

namespace BeachVolley.UI
{
    /// <summary>
    /// Drives the post-match flow WHEN A TOURNAMENT IS ACTIVE: advance to the next opponent,
    /// crown the champion, or eliminate the player. Listens to the same OnMatchEnded event as
    /// WinScreenController, but each guards on TournamentSession.IsActive so only one acts.
    ///
    /// Player 1 is always the human, so winner == Player1 means the player won this match.
    /// </summary>
    public class TournamentFlow : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button continueButton; // next match (mid-run win)
        [SerializeField] private Button menuButton;     // back to menu (champion / eliminated)

        private bool subscribed;

        private void OnEnable() => TrySubscribe();

        private void Start()
        {
            if (!subscribed) TrySubscribe();
            if (panel != null) panel.SetActive(false);
            if (continueButton != null) continueButton.onClick.AddListener(OnContinue);
            if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
        }

        private void OnDisable()
        {
            if (subscribed && GameManager.Instance != null)
            {
                GameManager.Instance.OnMatchEnded -= HandleMatchEnded;
                subscribed = false;
            }
            if (continueButton != null) continueButton.onClick.RemoveListener(OnContinue);
            if (menuButton != null) menuButton.onClick.RemoveListener(OnMenu);
        }

        private void TrySubscribe()
        {
            if (subscribed || GameManager.Instance == null) return;
            GameManager.Instance.OnMatchEnded += HandleMatchEnded;
            subscribed = true;
        }

        private void HandleMatchEnded(PlayerIndex winner)
        {
            if (!TournamentSession.IsActive) return; // not a tournament -> WinScreenController handles it

            TournamentRun run = TournamentSession.Active;

            if (winner == PlayerIndex.Player1)
            {
                // Player won this match -> advance the ladder.
                run.Advance();

                if (run.IsComplete)
                    Show("CAMPIONE!\nHai vinto il torneo.", showContinue: false, showMenu: true);
                else
                    Show($"Vinto! Prossimo avversario:\n{run.CurrentOpponent.DisplayName}  ({run.MatchNumber}/{run.TotalMatches})",
                         showContinue: true, showMenu: false);
            }
            else
            {
                // Player lost -> eliminated.
                Show("ELIMINATO!", showContinue: false, showMenu: true);
            }
        }

        private void Show(string message, bool showContinue, bool showMenu)
        {
            if (panel != null) panel.SetActive(true);
            if (messageText != null) messageText.text = message;
            if (continueButton != null) continueButton.gameObject.SetActive(showContinue);
            if (menuButton != null) menuButton.gameObject.SetActive(showMenu);
        }

        private void OnContinue()
        {
            // Re-arm exactly like the first match (GameManager in MainMenu -> Bootstrap starts it),
            // reset the score, write the next match, reload Gameplay.
            MatchSession.Set(TournamentSession.Active.BuildCurrentMatchConfig());
            GameManager.Instance.ResetMatch();
            GameManager.Instance.ChangeState(GameState.MainMenu);
            SceneManager.LoadScene(SceneNames.Gameplay);
        }

        private void OnMenu()
        {
            TournamentSession.Clear();
            GameManager.Instance.ResetMatch();
            GameManager.Instance.ChangeState(GameState.MainMenu);
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
