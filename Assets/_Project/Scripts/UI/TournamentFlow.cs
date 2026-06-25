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
    /// Drives the post-match flow WHEN A TOURNAMENT IS ACTIVE: advance, crown the champion, or
    /// eliminate the player. Records the result to the persistent save on champion/elimination.
    /// Listens to OnMatchEnded alongside WinScreenController; each guards on
    /// TournamentSession.IsActive so only one acts.
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
                {
                    RecordChampion(run);
                    SaveData save = SaveSystem.Load();
                    Show($"CAMPIONE!\nTornei vinti: {save.tournamentsWon}", showContinue: false, showMenu: true);
                }
                else
                {
                    Show($"Vinto! Prossimo avversario:\n{run.CurrentOpponent.DisplayName}  ({run.MatchNumber}/{run.TotalMatches})",
                         showContinue: true, showMenu: false);
                }
            }
            else
            {
                // Player lost -> eliminated. CurrentIndex = opponents beaten before this loss.
                RecordStreak(run.CurrentIndex);
                SaveData save = SaveSystem.Load();
                Show($"ELIMINATO!\nRecord avversari battuti: {save.longestStreak}", showContinue: false, showMenu: true);
            }
        }

        private void RecordChampion(TournamentRun run)
        {
            SaveData save = SaveSystem.Load();
            save.tournamentsWon++;
            if (run.TotalMatches > save.longestStreak) save.longestStreak = run.TotalMatches;
            SaveSystem.Save(save);
        }

        private void RecordStreak(int beaten)
        {
            SaveData save = SaveSystem.Load();
            if (beaten > save.longestStreak) save.longestStreak = beaten;
            SaveSystem.Save(save);
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
