using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.WordConnect.Demo
{
    /// <summary>
    /// Updates the UI to reflect the current game time 
    /// </summary>
    public class TimeUI : MonoBehaviour
    {
        /// <summary>
        /// The game manager being listened to.
        /// </summary>
        private WordConnectManager _gameManager;

        /// <summary>
        /// The text field to be updated.
        /// </summary>
        [SerializeField]
        private Text _textField;

        /// <summary>
        /// If the text should be set to a minutes & second format.
        /// </summary>
        [SerializeField]
        private bool _useMinuteFormatting = true;


        [SerializeField] bool isMyTimer;
        [SerializeField] bool canShowTime;
        [SerializeField] StopwatchMy GameTimer;

        /// <summary>
        /// Get the grid manager script instance.
        /// </summary>
        private void Start()
        {
            _gameManager = WordConnectManager.Instance;
            if (GameHandler.Instance.isTournament)
            {
                TimeSpan timeDifference = GameHandler.Instance.tournamentCurrentPlayerData.endDate.ToLocalTime() - DateTime.Now;
                GameTimer.RemainTime = timeDifference.TotalSeconds;

                GameTimer.IsRunning = true;
            }
            else
                _textField.gameObject.SetActive(false);
        }


        /// <summary>
        /// Update the text each frame.
        /// </summary>
        private void Update()
        {
            //if (!_gameManager.Configuration)
            //    return;

            UpdateText();
        }

        /// <summary>
        /// Update the text field with the timer.
        /// </summary>
        /// 
        bool isLessThen30;
        private void UpdateText()
        {
            if (isMyTimer && canShowTime)
            {
                string timeText;

                // Modify the time text based on the settings.
                if (_useMinuteFormatting)
                {
                    timeText = TournamentWaitingPanel.FormatTime(GameTimer.RemainTime);
                }
                else
                {
                    timeText = ((int)GameTimer.RemainTime).ToString();
                }
                if (GameTimer.RemainTime <= 30 && !isLessThen30)
                {
                    isLessThen30 = true;
                    _textField.color = Color.red;
                    GamePlayHandler.Instance.soundTournamentLast30Second.Play();
                    if (_textField.transform.parent.GetComponent<DOTweenAnimation>())
                    {
                        _textField.transform.parent.GetComponent<DOTweenAnimation>().DOPlay();
                    }
                }

                // Update the text field.
                _textField.text = timeText;
            }
            else if (canShowTime)
            {
                string timeText;

                // Modify the time text based on the settings.
                if (_useMinuteFormatting)
                {
                    Debug.LogError("Rem 3");
                    TimeSpan currentTime = TimeSpan.FromSeconds(_gameManager.GameTimer.ElapsedTime);
                    timeText = string.Format("{0:D2}:{1:D2}", currentTime.Minutes, currentTime.Seconds);
                }
                else
                {
                    timeText = ((int)_gameManager.GameTimer.ElapsedTime).ToString();
                    Debug.LogError("Rem 4");
                }

                // Update the text field.
                _textField.text = timeText;
            }
            // Declare local string.

        }
    }
}