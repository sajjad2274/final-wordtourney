using System;
using UnityEngine;
using UnityEngine.UI;
using DTT.Tweening;
//using TMPro;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using Firebase.Extensions;
using Unity.VisualScripting;

namespace DTT.WordConnect.Demo
{
    /// <summary>
    /// Class that manages UI related elements for the end game results popup.
    /// </summary>
    public class ResultsUI : MonoBehaviour
    {
        public GameProgressData progressData;
        /// <summary>
        /// The crossword game manager being listened to.
        /// </summary>
        private WordConnectManager _gameManager;

        /// <summary>
        /// The results UI object that should be toggled.
        /// </summary>
        [SerializeField]
        private GameObject _resultsFullUI;

        /// <summary>
        /// The results time text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsTimeText;

        /// <summary>
        /// The results score text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsTotalScoreText;

        [Header("Optional Text Fields")]
        /// <summary>
        /// The results score text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsScoreText;

        /// <summary>
        /// The results streak score text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsStreakScoreText;

        /// <summary>
        /// The results time score text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsTimeScoreText;

        /// <summary>
        /// The results cash text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsCashText;

        /// <summary>
        /// The results keys text to be updated.
        /// </summary>
        [SerializeField]
        private Text _resultsKeysText;

        [SerializeField]
        private GameObject _resultsCongratsPage;

        [SerializeField]
        private Text[] _resultsCongratsStageTxt;

        [SerializeField]
        private Text[] _resultsCongratsLevelTxt;

        [SerializeField]
        private Text _resultsCongratsRewardTxt;

        [SerializeField]
        private Image _resultsCongratsRewardFillBar;

        [SerializeField]
        private Text[] _resultsCongratsPlayerLevelTxt;

        [SerializeField]
        private Image[] _resultsCongratsPlayerLevelFillBar;

        [SerializeField]
        private GameObject _resultsRewardPage;
        [SerializeField]
        private GameObject _resultsRewardCollectBtn;

        [SerializeField]
        private Text _resultsRewardBulbTxt;

        [SerializeField]
        private Text _resultsRewardKeysTxt;

        [SerializeField]
        private Sprite[] _resultRewardsImages;

        [SerializeField]
        private Image _resultRewardsImage;

        [SerializeField]
        public GameObject tournamentEndedPanel;

        [Space]
        /// <summary>
        /// If the text should be set to a minutes & second format.
        /// </summary>
        [SerializeField]
        private bool _useMinuteFormatting = true;

        /// <summary>
        /// Defines if the UI should wait and listen for game results before displaying.
        /// </summary>
        [SerializeField]
        private bool _revealOnFinish = false;

        /// <summary>
        /// Defines if the UI should wait and listen for game results before displaying.
        /// </summary>
        [SerializeField]
        private bool _revealOnPause = false;

        /// <summary>
        /// The canvas group attached to the UI object to fade in or out.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [Space]
        [Header("Level Up")]
        public GameObject _levelUpPanel;
        public Text _levelUpTxt;
        private void Start()
        {
            CheckTournamentLevelCompleted();
        }
        public void CheckTournamentLevelCompleted()
        {
            if (GameHandler.Instance.isTournament)
            {
                if (PlayerPrefs.GetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0) >= GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels.Count - 1)
                {
                    CheckForTournamentWinLevelCompleted(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName);
                    tournamentEndedPanel.SetActive(true);

                }

            }
        }
        /// <summary>
        /// Subscribe to the game managers finish event if listening.
        /// </summary>
        private void OnEnable()
        {

            _gameManager = WordConnectManager.Instance;

            // Ensure UI is disabled at the start.
            _resultsFullUI.SetActive(false);

            if (_revealOnFinish)
                _gameManager.Finish += DisplayResults;
            if (_revealOnPause)
                _gameManager.Paused += DisplayPartialResults;
        }

        /// <summary>
        /// Unsubscribe from the game managers game finish event if listening.
        /// </summary>
        private void OnDisable()
        {
            if (_revealOnFinish)
                _gameManager.Finish -= DisplayResults;
            if (_revealOnPause)
                _gameManager.Paused -= DisplayPartialResults;


        }

        /// <summary>
        /// Update the text field with the timer.
        /// </summary>
        /// <param name="totalTime">Time in seconds to be formatted.</param>
        /// <returns>The given time in mm:ss format.</returns>
        private string GetFormattedTime(float totalTime)
        {
            // Helper string to be returned after modification.
            string timeText;

            // Modify the time text based on the settings.
            if (_useMinuteFormatting)
            {
                TimeSpan currentTime = TimeSpan.FromSeconds(totalTime);
                timeText = string.Format("{0:D2}:{1:D2}", currentTime.Minutes, currentTime.Seconds);
            }
            else
            {
                timeText = ((int)totalTime).ToString();
            }

            // Return the result.
            return timeText;
        }

        /// <summary>
        /// Displays the current time elapsed and score.
        /// </summary>
        private void DisplayPartialResults()
        {
            WordConnectState state = WordConnectManager.Instance.WordConnectState;

            // Get new values and update the text.
            float timeElapsed = (float)WordConnectManager.Instance.GameTimer.ElapsedTime;
            string scoreEarned = (state.CurrentScore + state.CurrentStreakBonusScore).ToString();
            UpdateText(GetFormattedTime(timeElapsed), scoreEarned);
            FadeInUI();
        }

        /// <summary>
        /// Displays the results of the crossword game received.
        /// </summary>
        /// <param name="results">The game result data to be displayed.</param>
        /// 
        bool isGiftPanel = false;
        public Action showLevelUpAction;
        private void DisplayResults(WordConnectResult results)
        {
            WordConnectState endState = WordConnectManager.Instance.WordConnectState;
            isGiftPanel = false;
            // Calculate all scores.
            string baseScore = endState.CurrentScore.ToString();
            string streakScore = endState.CurrentStreakBonusScore.ToString();
            // string timeScore = ((int)WordConnectManager.Instance.Configuration.CalculatePoints(results.timeTaken)).ToString();
            string totalScore = results.displayScore.ToString();

            // Populate the result UI object with our scores and time data.
            int currentScoreEarned = (int)results.displayScore;
            if (currentScoreEarned > 2) currentScoreEarned = currentScoreEarned / 2;
            UpdateText(GetFormattedTime(results.timeTaken), baseScore, streakScore, totalScore, currentScoreEarned.ToString(), "1");
            // Fade in the results UI after a short delay to let the last word animation complete.
            foreach (Text t in _resultsCongratsStageTxt) t.text = (GameHandler.Instance.progressData.levelCompleted + 2).ToString();
            foreach (Text t in _resultsCongratsLevelTxt) t.text = (GameHandler.Instance.progressData.level).ToString();
            GamePlayHandler.Instance.UpdateTxts();
            if (!GameHandler.Instance.isTournament)
            {
                GameHandler.Instance.progressData.currentRewardGiftPoints += 1;
                if (GamePlayHandler.Instance.progressData.currentRewardGiftPoints >= GamePlayHandler.Instance.progressData.requiredRewardGiftPoints)
                {
                    GamePlayHandler.Instance.progressData.currentRewardGiftPoints = 0;

                    int bulb = UnityEngine.Random.Range(1, 15);
                    int valLevel = (int)GamePlayHandler.Instance.progressData.level / 10;
                    int keys = UnityEngine.Random.Range(1, 4) * (valLevel > 0 ? valLevel : 1);
                    int rewardNo = UnityEngine.Random.Range(0, 3);
                    switch (rewardNo)
                    {
                        case 0:
                            _resultRewardsImage.sprite = _resultRewardsImages[0];
                            GamePlayHandler.Instance.progressData.bulb += (int)bulb;
                            break;
                        case 1:
                            _resultRewardsImage.sprite = _resultRewardsImages[1];
                            GamePlayHandler.Instance.progressData.hammer += (int)bulb;
                            break;
                        case 2:
                            _resultRewardsImage.sprite = _resultRewardsImages[2];
                            GamePlayHandler.Instance.progressData.fireCracker += (int)bulb;
                            break;

                    }
                    _resultsRewardBulbTxt.text = "x" + bulb;
                    _resultsRewardKeysTxt.text = "x" + keys;
                    _resultsRewardPage.SetActive(true);
                    _resultsRewardCollectBtn.SetActive(true);
                    isGiftPanel = true;
                }
                GameHandler.Instance.progressData.currentLevelPoints += (int)100;
                if (GameHandler.Instance.progressData.currentLevelPoints >= GameHandler.Instance.progressData.requiredLevelPoints)
                {
                    GameHandler.Instance.progressData.level++;
                    ///////// 
                    showLevelUpAction += ShowLevelUpScreen;
                    GameHandler.Instance.progressData.requiredLevelPoints += 500;
                    GameHandler.Instance.progressData.currentLevelPoints = 0;
                }
                foreach (Text t in _resultsCongratsPlayerLevelTxt) t.text = GameHandler.Instance.progressData.currentLevelPoints + " / " + GameHandler.Instance.progressData.requiredLevelPoints;
                foreach (Image t in _resultsCongratsPlayerLevelFillBar) t.fillAmount = (float)GamePlayHandler.Instance.progressData.currentLevelPoints / (float)GamePlayHandler.Instance.progressData.requiredLevelPoints;

            }
            else
            {
                CheckTournamentLevelCompleted();




            }

            _resultsCongratsRewardFillBar.fillAmount = (float)GamePlayHandler.Instance.progressData.currentRewardGiftPoints / (float)GamePlayHandler.Instance.progressData.requiredRewardGiftPoints;
            _resultsCongratsRewardTxt.text = GamePlayHandler.Instance.progressData.currentRewardGiftPoints.ToString() + "/" + GamePlayHandler.Instance.progressData.requiredRewardGiftPoints.ToString();
            if (!isGiftPanel)
                Invoke(nameof(OpenFinalPanel), 1.5f);
            FirebaseManager.Instance.SaveProgressData();
            GamePlayHandler.Instance.UpdateTxts();
            GamePlayHandler.Instance.CheckPowers();
            // FadeInUI(1f);
        }
        /// <summary>
        /// sajjad 
        /// </summary>
        /// 
        public void ShowWinPanelAfterGift()
        {
            if (isGiftPanel)
            {
                isGiftPanel = false;
                Invoke(nameof(OpenFinalPanel), 3);
            }
        }
        public void ShowLevelUpBtnClick()
        {
            showLevelUpAction?.Invoke();
        }
        public void ShowLevelUpScreen()
        {
            showLevelUpAction = null;
            // Level Up Functionality here Sajjad
            _levelUpPanel.SetActive(true);
            _levelUpTxt.text = progressData.level.ToString();
            Invoke(nameof(disableLevelUpPanel), 4);
        }
        void disableLevelUpPanel()
        {
            _levelUpPanel.SetActive(false);
        }
        public void OpenFinalPanel()
        {
            if (!GameHandler.Instance.isTournament)
            {
                _resultsCongratsPage.SetActive(true);
                _resultsCongratsPage.GetComponent<Canvas>().enabled = true;
                GamePlayHandler.Instance.PlaySoundWinFreeMode();
            }
            else
            {
                if (PlayerPrefs.GetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0) >= GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels.Count - 1)
                {
                    tournamentEndedPanel.SetActive(true);

                }
                else
                {
                    _resultsFullUI.SetActive(true);
                }




            }
        }
        public void CheckForTournamentWin(string tName)
        {
          TournamentHistory th = new TournamentHistory();
            FirebaseManager.Instance.dbf.Collection("Tournaments").Document(tName).Collection("Detail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
              {
                  if (task.IsCompleted)
                  {

                      QuerySnapshot snapshot1 = task.Result;

                      DocumentSnapshot detailDocument = task.Result.Documents.First();
                      DocumentSnapshot playersDocument = task.Result.Documents.First();



                      foreach (DocumentSnapshot document in snapshot1.Documents)
                      {


                          if (document.Id == "PrimaryDetail")
                          {
                              detailDocument = document;


                          }
                          else if (document.Id == "Players")
                          {
                              playersDocument = document;
                          }

                      }

                      Dictionary<string, object> detailData = detailDocument.ToDictionary();
                      Debug.Log(detailData["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime() + " D" + DateTime.Now);

                      if (detailData["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime() <= DateTime.Now)
                      {
                          Debug.Log(detailData["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime() + " D" + DateTime.Now);
                          bool rewardGot = false;

                          DocumentSnapshot snapshot = playersDocument;

                          Dictionary<string, object> dt = snapshot.ToDictionary();

                          Dictionary<string, int> dtforPrize = new Dictionary<string, int>();

                          foreach (var v in dt)
                          {
                              TournamentPlayerData tt = new TournamentPlayerData(JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()));


                              dtforPrize.Add(tt.playerID, tt.level);
                          }





                          var sortedDictForPrize = dtforPrize.OrderByDescending(pair => pair.Value);


                          List<KeyValuePair<string, int>> sortedListForPrize = sortedDictForPrize.ToList();



                          int iRange = 0;



                          //Loop through every users UID

                          if (GameHandler.Instance.tournamentCurrentPlayerData != null)
                              if (!GameHandler.Instance.tournamentCurrentPlayerData.prizeObtained)
                              {
                                  Debug.Log("player not obtained prize gaem");
                                  int prizeRange = 0;
                                  int lastLevel = -1;
                                  int playerPos = 1;
                                  foreach (KeyValuePair<string, int> childSnapshot in sortedDictForPrize)
                                  {
                                      Debug.Log(childSnapshot.Key + " " + FirebaseManager.Instance.User.UserId);
                                      if (childSnapshot.Key == FirebaseManager.Instance.User.UserId)
                                      {
                                          Debug.Log("keymatched");
                                          bool tied = false;
                                          if (lastLevel != -1)
                                          {
                                              if (lastLevel == childSnapshot.Value) tied = true;
                                          }
                                          if (prizeRange < GameHandler.Instance.TournamentCurrentPrizeDistributionCategory.Count)
                                          {
                                              List<TournamentRewards> tournamentRewardPrizes = new List<TournamentRewards>();
                                              foreach (TournamentPrize tpr in GameHandler.Instance.TournamentCurrentPrizeDistributionCategory[prizeRange].prize)
                                              {
                                                  TournamentRewards tp = new TournamentRewards();
                                                  switch (tpr.pType)
                                                  {
                                                      case "keys":
                                                          progressData.keys += (int)tpr.pValue;
                                                          break;
                                                      case "gems":
                                                          progressData.gems += (int)tpr.pValue;
                                                          break;
                                                      case "cash":
                                                          progressData.tickets += (int)tpr.pValue;
                                                          break;
                                                      default:
                                                          progressData.gems += (int)tpr.pValue;
                                                          break;
                                                  }
                                                  tp.pValue = tpr.pValue;
                                                  tp.pType = tpr.pType;

                                                  tp.tournamentName = tName;
                                                  if (tied)
                                                  {
                                                      tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                                  }
                                                  tournamentRewardPrizes.Add(tp);
                                              }
                                              GamePlayHandler.Instance.tournamentRewardPrizes = tournamentRewardPrizes;
                                              GamePlayHandler.Instance.ShowRewardTournament();
                                              th.tournamentName = tName;
                                              th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                              th.badgeInd = 0;
                                              th.position = playerPos.ToString();
                                              th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                              th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                              th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                              FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                                          }
                                          else
                                          {
                                              TournamentRewards tp = new TournamentRewards();
                                              progressData.tickets += (int)GameHandler.Instance.tournamentCurrentDefaultPrize;
                                              tp.pValue = GameHandler.Instance.tournamentCurrentDefaultPrize;
                                              tp.pType = "keys";
                                              tp.tournamentName = tName;
                                              if (tied)
                                              {
                                                  tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                              }

                                              GamePlayHandler.Instance.ShowRewardTournament(tp.pType, tp.pValue.ToString(), tName + "\nPosition: " + playerPos);
                                              th.tournamentName = tName;
                                              th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                              th.badgeInd = 0;
                                              th.position = playerPos.ToString();
                                              th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                              th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                              th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                              FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                                          }



                                          GameHandler.Instance.tournamentCurrentPlayerData.prizeObtained = true;
                                          progressData.wonTournament++;
                                          FirebaseManager.Instance.UpdateTournamentProgress(tName, GameHandler.Instance.tournamentCurrentPlayerData);
                                          FirebaseManager.Instance.SaveProgressData();

                                          rewardGot = true;
                                          break;
                                      }
                                      else
                                      {
                                          lastLevel = childSnapshot.Value;
                                      }




                                      prizeRange++;
                                      if (prizeRange >= GameHandler.Instance.tournamentCurrentPrizeDistributionCount || prizeRange >= GameHandler.Instance.TournamentCurrentPrizeDistributionCategory.Count)
                                      {
                                          break;
                                      }


                                      playerPos++;

                                  }




                              }

                          if (!rewardGot)
                          {
                              GamePlayHandler.Instance.ShowRewardTournament("-1", "-1", tName);
                              th.tournamentName = tName;
                              th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                              th.badgeInd = 0;
                              th.position = "No Reward";
                              th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                              th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                              th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                              FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                          }

                      }



                  }
                  else
                  {

                      Debug.LogError("Failed to get nested documents: " + task.Exception);
                  }

              });

        }
        public void CheckForTournamentWinLevelCompleted(string tName)
        {
         TournamentHistory th = new TournamentHistory();
            FirebaseManager.Instance.dbf.Collection("Tournaments").Document(tName).Collection("Detail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {

                    QuerySnapshot snapshot1 = task.Result;

                    DocumentSnapshot detailDocument = task.Result.Documents.First();
                    DocumentSnapshot playersDocument = task.Result.Documents.First();



                    foreach (DocumentSnapshot document in snapshot1.Documents)
                    {


                        if (document.Id == "PrimaryDetail")
                        {
                            detailDocument = document;


                        }
                        else if (document.Id == "Players")
                        {
                            playersDocument = document;
                        }

                    }

                    Dictionary<string, object> detailData = detailDocument.ToDictionary();
                    Debug.Log(detailData["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime() + " D" + DateTime.Now);


                    Debug.Log(detailData["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime() + " D" + DateTime.Now);
                    bool rewardGot = false;

                    DocumentSnapshot snapshot = playersDocument;

                    Dictionary<string, object> dt = snapshot.ToDictionary();

                    Dictionary<string, int> dtforPrize = new Dictionary<string, int>();

                    foreach (var v in dt)
                    {
                        TournamentPlayerData tt = new TournamentPlayerData(JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()));


                        dtforPrize.Add(tt.playerID, tt.level);
                    }





                    var sortedDictForPrize = dtforPrize.OrderByDescending(pair => pair.Value);


                    List<KeyValuePair<string, int>> sortedListForPrize = sortedDictForPrize.ToList();







                    //Loop through every users UID

                    if (GameHandler.Instance.tournamentCurrentPlayerData != null)
                        if (!GameHandler.Instance.tournamentCurrentPlayerData.prizeObtained)
                        {
                            Debug.Log("player not obtained prize gaem");
                            int prizeRange = 0;
                            int lastLevel = -1;
                            int playerPos = 1;
                            foreach (KeyValuePair<string, int> childSnapshot in sortedDictForPrize)
                            {
                                Debug.Log(childSnapshot.Key + " " + FirebaseManager.Instance.User.UserId);
                                if (childSnapshot.Key == FirebaseManager.Instance.User.UserId)
                                {
                                    Debug.Log("keymatched");
                                    bool tied = false;
                                    if (lastLevel != -1)
                                    {
                                        if (lastLevel == childSnapshot.Value) tied = true;
                                    }
                                    if (prizeRange < GameHandler.Instance.TournamentCurrentPrizeDistributionCategory.Count)
                                    {
                                        List<TournamentRewards> tournamentRewardPrizes = new List<TournamentRewards>();
                                        foreach (TournamentPrize tpr in GameHandler.Instance.TournamentCurrentPrizeDistributionCategory[prizeRange].prize)
                                        {
                                            TournamentRewards tp = new TournamentRewards();
                                            switch (tpr.pType)
                                            {
                                                case "keys":
                                                    progressData.keys += (int)tpr.pValue;
                                                    break;
                                                case "gems":
                                                    progressData.gems += (int)tpr.pValue;
                                                    break;
                                                case "cash":
                                                    progressData.tickets += (int)tpr.pValue;
                                                    break;
                                                default:
                                                    progressData.gems += (int)tpr.pValue;
                                                    break;
                                            }
                                            tp.pValue = tpr.pValue;
                                            tp.pType = tpr.pType;

                                            tp.tournamentName = tName;
                                            if (tied)
                                            {
                                                tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                            }
                                            tournamentRewardPrizes.Add(tp);
                                        }
                                        GamePlayHandler.Instance.tournamentRewardPrizes = tournamentRewardPrizes;
                                        GamePlayHandler.Instance.ShowRewardTournament();
                                        th.tournamentName = tName;
                                        th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                        th.badgeInd = 0;
                                        th.position = playerPos.ToString();
                                        th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                        th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                        th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                        FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                                    }
                                    else
                                    {
                                        TournamentRewards tp = new TournamentRewards();
                                        progressData.tickets += (int)GameHandler.Instance.tournamentCurrentDefaultPrize;
                                        tp.pValue = GameHandler.Instance.tournamentCurrentDefaultPrize;
                                        tp.pType = "keys";
                                        tp.tournamentName = tName;
                                        if (tied)
                                        {
                                            tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                        }

                                        GamePlayHandler.Instance.ShowRewardTournament(tp.pType, tp.pValue.ToString(), tName + "\nPosition: " + playerPos);
                                        th.tournamentName = tName;
                                        th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                        th.badgeInd = 0;
                                        th.position = playerPos.ToString();
                                        th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                        th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                        th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                        FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                                    }



                                    GameHandler.Instance.tournamentCurrentPlayerData.prizeObtained = true;
                                    progressData.wonTournament++;
                                    FirebaseManager.Instance.UpdateTournamentProgress(tName, GameHandler.Instance.tournamentCurrentPlayerData);
                                    FirebaseManager.Instance.SaveProgressData();

                                    rewardGot = true;
                                    break;
                                }
                                else
                                {
                                    lastLevel = childSnapshot.Value;
                                }




                                prizeRange++;
                                if (prizeRange >= GameHandler.Instance.tournamentCurrentPrizeDistributionCount || prizeRange >= GameHandler.Instance.TournamentCurrentPrizeDistributionCategory.Count)
                                {
                                    break;
                                }


                                playerPos++;

                            }






                        }

                    if (!rewardGot)
                    {
                        GamePlayHandler.Instance.ShowRewardTournament("-1", "-1", tName);
                        th.tournamentName = tName;
                        th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                     
                        th.badgeInd = 0;
                        th.position = "No Reward";
                        th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                        th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                        th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                        FirebaseManager.Instance.UpdateTournamentHistoryData(tName, JsonConvert.SerializeObject(th));
                    }





                }
                else
                {

                    Debug.LogError("Failed to get nested documents: " + task.Exception);
                }

            });

        }
        /// <summary>
        /// Fades the Results UI In.
        /// </summary>
        /// <param name="delay">The delay in seconds before starting fading in the UI.</param>
        public void FadeInUI(float delay = 0)
        {
            _resultsFullUI.SetActive(true);
            DTTween.Value(0f, 1f, 0.5f, delay, Easing.EASE_OUT_EXPO, (value) => _canvasGroup.alpha = value, () => _canvasGroup.alpha = 1);
        }

        /// <summary>
        /// Fades the Results UI In.
        /// </summary>
        public void FadeOutUI()
        {
            DTTween.Value(1f, 0f, 0.5f, 0f, Easing.EASE_OUT_EXPO, (value) => _canvasGroup.alpha = value, () => _resultsFullUI.SetActive(false));
        }

        /// <summary>
        /// Update the total score and time text, the other score fields are optional.
        /// </summary>
        /// <param name="timeText">The time to display.</param>
        /// <param name="scoreText">The total score to display.</param>
        private void UpdateText(string timeText, string scoreText)
        {
            _resultsTimeText.text = timeText;
            _resultsTotalScoreText.text = scoreText;
        }

        /// <summary>
        /// Updates all score and time texts.
        /// </summary>
        /// <param name="timeText">The time the game has been playing for.</param>
        /// <param name="scoreText">The base score.</param>
        /// <param name="streakScoreText">The bonus score earned from a streak.</param>
        /// <param name="timeScoreText">The bonus score earned from time.</param>
        /// <param name="totalScoreText">All the previous scores added together.</param>
        private void UpdateText(string timeText, string scoreText, string streakScoreText, string totalScoreText)
        {
            _resultsTimeText.text = timeText;
            _resultsScoreText.text = scoreText;
            _resultsStreakScoreText.text = streakScoreText;
            // _resultsTimeScoreText.text = timeScoreText;
            _resultsTotalScoreText.text = totalScoreText;
        }
        private void UpdateText(string timeText, string scoreText, string streakScoreText, string totalScoreText, string cashTxt, string keysTxt)
        {
            _resultsTimeText.text = timeText;
            _resultsScoreText.text = scoreText;
            _resultsStreakScoreText.text = streakScoreText;
            // _resultsTimeScoreText.text = timeScoreText;
            _resultsTotalScoreText.text = totalScoreText;
            _resultsCashText.text = cashTxt;
            _resultsKeysText.text = keysTxt;
        }
    }
}