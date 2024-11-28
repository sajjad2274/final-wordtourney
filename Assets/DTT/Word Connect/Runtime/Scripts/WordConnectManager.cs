using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DTT.MinigameBase;
using DTT.WordConnect.Demo;
using DTT.WordConnect.Editor;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace DTT.WordConnect
{
    /// <summary>
    /// Managing script which ties together all the data and UI, controls start, pause and finish behaviour.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class WordConnectManager : MonoBehaviour, IMinigame<WordConnectConfigurationData, WordConnectResult>
    {
        /// <summary>
        /// The singleton instance of this script which other behviours can use to attach to events.
        /// </summary>
        public static WordConnectManager Instance { get; set; }

        /// <summary>
        /// The config file that will be used in this game.
        /// </summary>
        public WordConnectConfigurationData Configuration { get; private set; }

        /// <summary>
        /// Class which holds the state of the word connect game.
        /// </summary>
        public WordConnectState WordConnectState { get; private set; }

        /// <summary>
        /// Letters which are available to spell the words in the game.
        /// </summary>
        public List<LetterInput> AvailableLetters { get; private set; }

        /// <summary>
        /// The stopwatch being used to track the current amount of time spent on a Word Connect game.
        /// </summary>
        public StopwatchMy GameTimer;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsGameActive { get; private set; }

        /// <summary>
        /// Event is invoked whenever the WordConnectState changes. (input changes, word is found or hinted)
        /// Any UI needing this data can listen to this event for changes.
        /// </summary>
        public Action<WordConnectState> StateUpdated;

        /// <summary>
        /// Is called once the the game manager has initialized its variables.
        /// Listen to this event if you require to do something before the game grid is built.
        /// </summary>
        public event Action Initialized;

        /// <summary>
        /// Is called once the game grid should be built.
        /// </summary>
        public event Action BuildGame;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action Started;

        /// <summary>
        /// Is called when the game has been paused.
        /// </summary>
        public event Action Paused;

        /// <summary>
        /// Is called when the game has been continued.
        /// </summary>
        public event Action Continued;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<WordConnectResult> Finish;

        /// <summary>
        /// Event called right before ending the game.
        /// </summary>
        public event Action Cleanup;

        /// <summary>
        /// Initialize the game stopwatch.
        /// </summary>
      //  float timePassed = 0f;
        private void Awake()
        {
            Instance = this;
            //GameTimer = new Stopwatch();

        }

        public WordConnectConfigurationData GetLevelData(WordConnectConfigurationData config)
        {
            GetComponent<WordConnectGridManager>()._gameData = config;
            GetComponent<WordConnectGridManager>().GenerateNewVectorLayout();
            GetComponent<WordConnectGridManager>().BuildActiveLayouts();
            GetComponent<WordConnectGridManager>().SaveLayoutToGameData();
            return GetComponent<WordConnectGridManager>()._gameData;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="config">The config file used to start the game.</param>
        public void StartGame(WordConnectConfigurationData config)
        {
            if (IsGameActive)
                return;
            StartCoroutine(StartGameEnem(config));
            Invoke(nameof(RevealLetterHintAfterSaving), 0.1f);
        }
        System.Collections.IEnumerator StartGameEnem(WordConnectConfigurationData config)
        {
            GamePlayHandler.Instance.doubleRewardAdBtn.SetActive(true);





            // Set game related elements.
            IsGameActive = true;
            IsPaused = false;
            GameTimer.Restart();

            if (GameHandler.Instance.isTournament)
            {
                UnityEngine.Debug.Log(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + " Tournament ramain time");

                GameTimer.ElapsedTime += PlayerPrefs.GetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, 0);
                //if (PlayerPrefs.HasKey(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "LastExitTime"))
                //{
                //    DateTime lastPlayedDate;

                //    if(DateTime.TryParse(PlayerPrefs.GetString(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "LastExitTime",""),out lastPlayedDate))
                //    {
                //        GameTimer.ElapsedTime += (float)(DateTime.Now - lastPlayedDate).TotalSeconds;
                //    }
                //}
                Configuration = GetLevelData(config);
            }
            else
            {
                //while (true)
                //{
                //    yield return null;
                //    Configuration = GetLevelData(config);
                //    if (Configuration.WordVectors.Count >= config.DictionaryAsset.WordHintPairs.Length)
                //    {
                //        break;
                //    }
                //}
                Configuration = GetLevelData(config);
            }







            // Create a new game state object.
            WordConnectState = new WordConnectState();
            WordConnectState.SetHintBalance(Configuration.LetterHintsAvailable, Configuration.WordHintsAvailable, Configuration.DescriptionHintsAvailable);

            // Initialize two new lists.
            List<string> wordsInCrossword = new List<string>();
            List<WordHintPair> wordsHintsInCrossword = new List<WordHintPair>();

            // Get all words and word hint pairs from the configuration object and add them to the lists.
            foreach (WordVector wordVector in Configuration.WordVectors)
            {
                wordsInCrossword.Add(wordVector.WordHintPair.Word);
                wordsHintsInCrossword.Add(wordVector.WordHintPair);
            }

            // Update the game state object with the words currently in the game and build the hint options with the given words.
            WordConnectState.SetAvailableWords(wordsInCrossword);
            WordConnectState.SetHintOptions(wordsHintsInCrossword);

            // Get the necessary letters from the words list.
            List<char> availableLetters = GetAllLettersFromWords(WordConnectState.WordsInCrossword);
            SetAvailableLetters(availableLetters);



            // Invoke the initialization event. 
            Initialized?.Invoke();
            // Invoke the build event.
            BuildGame?.Invoke();
            // Invoke the start of the game.
            Started?.Invoke();
            if (GameHandler.Instance.fireCrackerUsed)
            {
                if (GameHandler.Instance.fireCrackerPowerPriceUsed)
                {
                    GameHandler.Instance.fireCrackerPowerPriceUsed = false;
                    GameHandler.Instance.progressData.gems += GamePlayHandler.Instance.fireCrackerPowerPrice;
                }
                GameHandler.Instance.fireCrackerUsed = false;
                RevealWordHint();
            }
            if (PlayerPrefs.GetInt("gamesPlayedW", 0) == 0 && !GameHandler.Instance.isTournament)
            {
                GamePlayHandler.Instance.StartWatchAdBtnTime();
                PlayerPrefs.SetInt("gamesPlayedW", PlayerPrefs.GetInt("gamesPlayedW", 0) + 1);
                PlayerPrefs.Save();
            }
            else if (!GameHandler.Instance.isTournament)
            {
                GamePlayHandler.Instance.StopWatchAdBtnTime();
                PlayerPrefs.SetInt("gamesPlayedW", PlayerPrefs.GetInt("gamesPlayedW", 0) + 1);
                PlayerPrefs.Save();
                if (PlayerPrefs.GetInt("gamesPlayedW", 0) >= 3)
                {

                    PlayerPrefs.SetInt("gamesPlayedW", 0);
                    PlayerPrefs.Save();
                }
            }
            GamePlayHandler.Instance.InitWordsFound();
            foreach (string w in GamePlayHandler.Instance.wordsFound.Words)
            {

                SubmitWordStart(w);
            }
            GamePlayHandler.Instance.UpdateTxts();
            if (PlayerPrefs.GetInt("gamesPlayed", 0) < 5)
            {
                PlayerPrefs.SetInt("gamesPlayed", PlayerPrefs.GetInt("gamesPlayed", 0) + 1);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt("gamesPlayed", 0);
                PlayerPrefs.Save();
                if (!PlayerPrefs.HasKey("ratedone"))
                    GamePlayHandler.Instance.OpenReviewPanel();
            }
            yield return null;
            //Invoke(nameof(RevealLetters), 5);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Pause()
        {
            if (!IsGameActive)
                return;

            // Set game related elements.
            IsPaused = true;
            // GameTimer.Stop();

            // Invoke the pause of the game.
            Paused?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Continue()
        {
            if (!IsGameActive)
                return;

            // Set game related elements.
            IsPaused = false;
            // GameTimer.StartWatch();

            // Invoke the end of the game.
            Continued?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ForceFinish()
        {
            if (!IsGameActive)
                return;

            // Stops the game.
            ForceStop();

            float timeElapsed = (float)GameTimer.ElapsedTime;
            // The score that has been earned by the player.
            int scoreEarned = WordConnectState.CurrentScore + WordConnectState.CurrentStreakBonusScore /*+ (int)Configuration.CalculatePoints(timeElapsed)*/;

            // Calculate the maximum achievable streak score.
            int maxPossibleStreakScore = 0;
            for (int i = 0; i < WordConnectState.WordsInCrossword.Count; i++) maxPossibleStreakScore += i * Configuration.StreakScoreIncrement;

            // The maximum achievable score in this level.
            int maxPossibleScore = Configuration.ScorePerWordFound * WordConnectState.WordsInCrossword.Count + maxPossibleStreakScore + Configuration.MaxBonusTimeScore;

            // Generate a results struct.
            WordConnectResult results = new WordConnectResult((int)timeElapsed, (float)scoreEarned / maxPossibleScore, scoreEarned);
            int currentScoreEarned = scoreEarned;
            if (currentScoreEarned > 2) currentScoreEarned = currentScoreEarned / 2;
            else
            {
                GamePlayHandler.Instance.doubleRewardAdBtn.SetActive(false);
            }
            //  GameHandler.Instance.progressData.gems += (uint)currentScoreEarned;
            if (GameHandler.Instance.isTournament)
            {

                GameHandler.Instance.progressData.keys += 0;
            }
            else
            {
                GameHandler.Instance.progressData.keys += 1;
            }


            //GameHandler.Instance.progressData.keys += 1;
            GamePlayHandler.Instance.ClearWordFound();
            ClearHintsFile();
            UnityEngine.Debug.Log(results.finalScore + " FinalScoere " + scoreEarned);
            GamePlayHandler.Instance.particlesTime = 0f;
            GamePlayHandler.Instance.UpdateTxts();
            PlayerPrefs.DeleteKey("LevelSaveData");
            PlayerPrefs.Save();
            // Invoke the end of the game.
            if (GameHandler.Instance.isTournament)
            {
                GameHandler.Instance.tournamentCurrentPlayerData.wordsFound = "";
                FirebaseManager.Instance.UpdateTournamentProgress(timeElapsed, 1);
                PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, 0f);
                PlayerPrefs.Save();
            }
            else
            {
                GameHandler.Instance.progressData.wordsFound = "";
            }
            Finish?.Invoke(results);

        }
        private void ClearHintsFile()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "hints.json");
            filledData.Clear();
            if (File.Exists(path))
            {
                // Option 1: Delete the file
                File.Delete(path);


                // Option 2: Clear the contents of the file
                // File.WriteAllText(path, string.Empty);
                // Debug.Log("Hints file cleared.");
            }
        }
        /// <summary>
        /// Is called when the user want to force stop the game.
        /// </summary>
        public void ForceStop()
        {
            // Set game related elements.
            IsGameActive = false;
            IsPaused = false;
            GameTimer.Stop();

            Cleanup?.Invoke();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (GameHandler.Instance.isTournament)
                {
                    PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, (float)GameTimer.ElapsedTime);
                    PlayerPrefs.Save();
                }
            }
        }
        private void OnApplicationQuit()
        {
            if (GameHandler.Instance.isTournament)
            {
                PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, (float)GameTimer.ElapsedTime);
                PlayerPrefs.Save();
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (GameHandler.Instance.isTournament)
                {
                    PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, (float)GameTimer.ElapsedTime);
                    PlayerPrefs.Save();
                }
            }
        }
        /// <summary>
        /// Is called when the user wants to restart the current game.
        /// </summary>
        public void ForceRestart()
        {
            // Stop the ongoing game.
            ForceStop();

            // Restart using the current configuration.
            StartGame(Configuration);
        }
        public void ForceNextlevel()
        {
            // Stop the ongoing game.
            ForceStop();
            if (GameHandler.Instance.progressData.levelCompleted < GameHandler.Instance.levels.Count - 1) GameHandler.Instance.progressData.levelCompleted++;
            FirebaseManager.Instance.SaveProgressData();
            // Restart using the current configuration.
            StartGame(GetConfig());
        }
        public WordConnectConfigurationData GetConfig()
        {
            if (GameHandler.Instance.isTournament)
            {
                if (GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels.Count > 0)
                {
                    if (PlayerPrefs.GetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0) < GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels.Count)
                    {
                        GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels[PlayerPrefs.GetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0)];

                    }
                    else
                    {
                        PlayerPrefs.SetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0);
                        PlayerPrefs.Save();
                        GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.tournamentLevels[GameHandler.Instance.tournamentCurrentPlayerData.tournamentNo].levels[PlayerPrefs.GetInt(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "Level", 0)];

                    }
                }
                else
                {
                    GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[UnityEngine.Random.Range(0, GameHandler.Instance.levels.Count)];
                }
            }
            else if (GameHandler.Instance.progressData.levelCompleted < GameHandler.Instance.levels.Count - 1) GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[GameHandler.Instance.progressData.levelCompleted % GameHandler.Instance.levels.Count];
            else
            {
                GameHandler.Instance.progressData.levelCompleted = 0;
                GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[GameHandler.Instance.progressData.levelCompleted % GameHandler.Instance.levels.Count];
            }
            // GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[PlayerPrefs.GetInt("levelsUnlocked", 0) % GameHandler.Instance.levels.Count];
            //if (GameHandler.Instance.progressData.levelCompleted < GameHandler.Instance.levels.Count - 1) GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[GameHandler.Instance.progressData.levelCompleted % GameHandler.Instance.levels.Count];
            //else GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[UnityEngine.Random.Range(0, GameHandler.Instance.levels.Count)];
            return GameHandler.Instance.config;
        }
        /// <summary>
        /// Checks whether the letter input is valid and handles accordingly.
        /// </summary>
        /// <param name="letterInput">Information on what letter was pressed.</param>
        public void HandleLetterInput(LetterInput letterInput)
        {
            // Check if letter has a valid char.
            if (!char.IsLetter(letterInput.letter)) return;

            // Get the current sequence of letters which were pressed.
            List<LetterInput> currentLetterSequence = WordConnectState.CurrentWordInput;

            // If the previous letter was inputted, remove the input. (erase the last letter)
            if (currentLetterSequence.Count > 1 && currentLetterSequence[currentLetterSequence.Count - 2].id == letterInput.id)
            {
                WordConnectState.RemoveLastLetter();
                StateUpdated?.Invoke(WordConnectState);
            }
            // If this LetterInput isn't already added in the sequence (each letter can only be use once), add it to the end.
            else if (!WordConnectState.CurrentWordInput.Contains(letterInput))
            {
                WordConnectState.AddLetterInput(letterInput);
                StateUpdated?.Invoke(WordConnectState);
            }
        }

        /// <summary>
        /// Reveals a single letter of a word.
        /// </summary>
        /// 
        HintOption hintOptions;
        public List<HintsSaving> filledData = new List<HintsSaving>();
        public void RevealLetterHint()
        {
            // Check if there are any hint options left.
            if (WordConnectState.HintOptions.Count == 0)
                return;

            // Get a random word hint option object from the word connect state.
            int indexOfWord = UnityEngine.Random.Range(0, WordConnectState.HintOptions.Count);
            HintOption hint = WordConnectState.HintOptions[indexOfWord];

            // the index list representing the letter indexes of the word which are available to reveal.
            List<int> availableLetters = new List<int>();

            // Get all the letters of this word that have not yet been revealed or found.
            for (int i = 0; i < hint.LetterRevealed.Count; i++)
            {
                if (!hint.LetterRevealed[i])
                {
                    if (Configuration.LetterHasOverlap(hint.Word, i))
                    {
                        string overlappingWord = Configuration.GetLetterOverlap(hint.Word, i).Item1;
                        if (!WordConnectState.CorrectlyAddedWords.Contains(overlappingWord))
                        {
                            availableLetters.Add(i);
                        }
                    }
                    else
                    {
                        availableLetters.Add(i);
                    }
                }
            }

            // Get a random index of one of the available letters to reveal.
            int chosenHintLetter = availableLetters[UnityEngine.Random.Range(0, availableLetters.Count)];
            // Reveal the chosen letter.
            WordConnectState.SetHintLetterRevealed(hint, chosenHintLetter);

            if (Configuration.LetterHasOverlap(hint.Word, chosenHintLetter))
            {
                (string, int) wordLetterIndex = Configuration.GetLetterOverlap(hint.Word, chosenHintLetter);
                if (WordConnectState.HintOptions.Any(x => x.Word == wordLetterIndex.Item1))
                {
                    HintOption overlappingHint = WordConnectState.HintOptions.Find(x => x.Word == wordLetterIndex.Item1);
                    WordConnectState.SetHintLetterRevealed(overlappingHint, wordLetterIndex.Item2);
                }
            }

            WordConnectState.UseLetterHint();
            StateUpdated?.Invoke(WordConnectState);

            if (availableLetters.Count == 1)
            {
                WordConnectState.AddFoundWord(hint.Word);
                WordConnectState.RemoveHintOption(hint);
                GamePlayHandler.Instance.AddWordFound(hint.Word);
                RemoveFoundWord(hint.Word);
            }

            if (CheckGameComplete())
                ForceFinish();

            // Save hint data
            HintsSaving hintsSaving = new HintsSaving
            {
                wordIndex = indexOfWord,
                word = hint.Word,
                letterIndex = chosenHintLetter
            };
            filledData.Add(hintsSaving);
            SaveHintsToFile();
        }


        private void SaveHintsToFile()
        {
            string json = JsonConvert.SerializeObject(filledData);

            // Use persistentDataPath for mobile devices
            string path = Path.Combine(Application.persistentDataPath, "hints.json");
            File.WriteAllText(path, json);

            UnityEngine.Debug.Log("Hints saved to " + path);
        }


        /// <summary>
        /// Sajjad Highlighted retrieving
        /// </summary>
        /// 

        public void RevealLetterHintAfterSaving()
        {
            if (WordConnectState.HintOptions.Count == 0)
                return;

            // Load the hint data from file
            LoadHintsFromFile();

            if (filledData == null || filledData.Count <= 0)
                return;

            // Create a list to track hints that need to be updated
            List<HintsSaving> hintsToUpdate = new List<HintsSaving>();

            foreach (var hintData in filledData)
            {
                // Validate the hint index
                if (hintData.wordIndex >= WordConnectState.HintOptions.Count)
                    continue;

                // Get the hint option
                HintOption hint = WordConnectState.HintOptions[hintData.wordIndex];

                // Only process if the letter hasn't been revealed yet
                if (!hint.LetterRevealed[hintData.letterIndex])
                {
                    // Reveal the letter in the hint
                    WordConnectState.SetHintLetterRevealed(hint, hintData.letterIndex);

                    // Check for overlapping words and reveal their letters
                    if (Configuration.LetterHasOverlap(hint.Word, hintData.letterIndex))
                    {
                        (string, int) wordLetterIndex = Configuration.GetLetterOverlap(hint.Word, hintData.letterIndex);
                        if (WordConnectState.HintOptions.Any(x => x.Word == wordLetterIndex.Item1))
                        {
                            HintOption overlappingHint = WordConnectState.HintOptions.Find(x => x.Word == wordLetterIndex.Item1);
                            if (!overlappingHint.LetterRevealed[wordLetterIndex.Item2])
                            {
                                WordConnectState.SetHintLetterRevealed(overlappingHint, wordLetterIndex.Item2);
                                hintsToUpdate.Add(new HintsSaving
                                {
                                    wordIndex = filledData.IndexOf(hintData),
                                    word = overlappingHint.Word,
                                    letterIndex = wordLetterIndex.Item2
                                });
                            }
                        }
                    }

                    // Add the current hint to the update list
                    hintsToUpdate.Add(hintData);
                }
            }

            // Update the UI or game state if necessary
            StateUpdated?.Invoke(WordConnectState);



            // Check if the game is complete
            if (CheckGameComplete())
                ForceFinish();
        }
        private void LoadHintsFromFile()
        {
            // Use persistentDataPath for mobile devices
            string path = Path.Combine(Application.persistentDataPath, "hints.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                filledData = JsonConvert.DeserializeObject<List<HintsSaving>>(json);
                UnityEngine.Debug.Log("Hints loaded from " + path);
            }
            else
            {
                UnityEngine.Debug.LogWarning("No saved hints found at " + path);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////
        public void RevealLetterHint(int lNo, HintOption hopt)
        {
            // Check if there are any hint options left.
            if (WordConnectState.HintOptions.Count == 0)
                // No hints options left.
                return;

            // Get a random word hint option object from the word connect state.
            HintOption hint = new HintOption(hopt.Word, hopt.WordDescription);
            foreach (HintOption hintOption in WordConnectState.HintOptions)
            {
                if (hintOption.Word == hopt.Word && hintOption.WordDescription == hopt.WordDescription)
                {
                    hint = hintOption;
                    break;
                }
            }

            // the index list representing the letter indexes of the word which are available to reveal.
            List<int> availableLetters = new List<int>();

            // Get all the letters of this word that have not yet been revealed or found.
            for (int i = 0; i < hint.LetterRevealed.Count; i++)
            {
                // Check if this letter has been revealed.
                if (!hint.LetterRevealed[i])
                {
                    // Check if this letter has overlap.
                    if (Configuration.LetterHasOverlap(hint.Word, i))
                    {
                        // Get the other word that this letter overlaps.
                        string overlappingWord = Configuration.GetLetterOverlap(hint.Word, i).Item1;
                        // If the overlapped word is already found this is not a viable letter to reveal as it has already been found!
                        if (!WordConnectState.CorrectlyAddedWords.Contains(overlappingWord))
                        {
                            availableLetters.Add(i);
                        }
                    }
                    // If the letter doesnt have overlap it is a viable letter to reveal.
                    else
                    {
                        availableLetters.Add(i);
                    }
                }
            }

            // Get a random index of one of the available letters to reveal.
            int chosenHintLetter = lNo;
            // Reveal the chosen letter.
            WordConnectState.SetHintLetterRevealed(hint, chosenHintLetter);
            // Check if the chosen letter has an overlapping word in the grid.
            if (Configuration.LetterHasOverlap(hint.Word, chosenHintLetter))
            {
                // Gets the word string for the overlapping word and the index of the letter which overlaps.
                (string, int) wordLetterIndex = Configuration.GetLetterOverlap(hint.Word, chosenHintLetter);
                // If this word is not hintable skip the next part.
                if (WordConnectState.HintOptions.Any(x => x.Word == wordLetterIndex.Item1))
                {
                    // Find the HintOption object for the overlapping word.
                    HintOption overlappingHint = WordConnectState.HintOptions.Find(x => x.Word == wordLetterIndex.Item1);
                    // Set the overlapping letter index to revealed.
                    WordConnectState.SetHintLetterRevealed(overlappingHint, wordLetterIndex.Item2);
                }
            }
            // Subtract one of the letter hint balance.
            WordConnectState.UseLetterHint();
            // Game state updated, invoke the event for UI to respond accordingly.
            StateUpdated?.Invoke(WordConnectState);
            // If this was the last available letter of this hint remove it *after* the StateUpdated event so UI can update their state before the hint is removed completely.
            if (availableLetters.Count == 1)
            {
                // All letters have been revealed or found, set this word to found.
                WordConnectState.AddFoundWord(hint.Word);
                WordConnectState.RemoveHintOption(hint);
                GamePlayHandler.Instance.AddWordFound(hint.Word);
                RemoveFoundWord(hint.Word);
            }
            // Check if the game has been completed.
            if (CheckGameComplete())
                ForceFinish();

            HintsSaving hintsSaving = new HintsSaving
            {
                wordIndex = WordConnectState.HintOptions.IndexOf(hint),
                word = hint.Word,
                letterIndex = chosenHintLetter
            };
            filledData.Add(hintsSaving);
            SaveHintsToFile();
        }
        /// <summary>
        /// Reveals an entire word.
        /// </summary>
        public void RevealWordHint()
        {
            // Check if there are any hint options left.
            if (WordConnectState.HintOptions.Count == 0)
                return;

            HintOption hint;
            // Prioritization, find the longest word to reveal.
            if (Configuration.PrioritizeLongestWordHint)
            {
                // Set the first hint as the selected hint.
                hint = WordConnectState.HintOptions[0];

                // Loop over all other hints to check if they have more unrevealed letters.
                for (int i = 1; i < WordConnectState.HintOptions.Count; i++)
                {
                    // Count the amount of letters that haven't been revealed yet.
                    int hintableLettersInWord = WordConnectState.HintOptions[i].LetterRevealed.Count(revealed => !revealed);
                    // If this hint has more unrevealed letters than the selected hint, set this hint as the new selected hint.
                    if (hintableLettersInWord > hint.LetterRevealed.Count(revealed => !revealed))
                        hint = WordConnectState.HintOptions[i];
                }
            }
            else
            {
                // Get a random word hint option object from the word connect state.
                hint = WordConnectState.HintOptions[UnityEngine.Random.Range(0, WordConnectState.HintOptions.Count)];
            }
            // Set the chosen hint to revealed.
            WordConnectState.SetHintWordRevealed(hint);
            GamePlayHandler.Instance.AddWordFound(hint.Word);
            RemoveFoundWord(hint.Word);

            // Loop over all the letters in the chosen word hint.
            for (int i = 0; i < hint.Word.Length; i++)
            {
                // Check if the letter overlaps with any other word in the grid.
                if (Configuration.LetterHasOverlap(hint.Word, i))
                {
                    // Gets the word string for the overlapping word and the index of the letter which overlaps.
                    (string, int) wordLetterIndex = Configuration.GetLetterOverlap(hint.Word, i);
                    // If this word is not a hintable, skip the next part.
                    if (!WordConnectState.HintOptions.Any(x => x.Word == wordLetterIndex.Item1)) continue;
                    // Find the HintOption object for the overlapping word.
                    HintOption overlappingHint = WordConnectState.HintOptions.Find(x => x.Word == wordLetterIndex.Item1);
                    // Set the overlapping letter index to revealed.
                    WordConnectState.SetHintLetterRevealed(overlappingHint, wordLetterIndex.Item2);
                }
            }
            // Subtract one of the word hint balance.
            WordConnectState.UseWordHint();
            // Game state updated, invoke the event for UI to respond accordingly.
            StateUpdated?.Invoke(WordConnectState);

            // Check if the game has been completed.
            if (CheckGameComplete())
                ForceFinish();
        }
        /// <summary>
        /// Reveals the description of a word.
        /// </summary>
        public void RevealDescriptiveHint()
        {
            // If there is already an active hint, cancel.
            if (WordConnectState.CurrentRevealedDescriptiveHint != null)
                return;

            // Get all hint options which haven't had their description revealed.
            List<HintOption> availableDescriptiveHints = WordConnectState.HintOptions.FindAll(hint => !hint.DescriptionRevealed);

            if (availableDescriptiveHints.Count == 0)
                // There are no available descriptive hints available, return.
                return;

            // Get a random hint object from the list of available hints.
            HintOption chosenHint = availableDescriptiveHints[UnityEngine.Random.Range(0, availableDescriptiveHints.Count)];
            // Set the chosen hint.
            WordConnectState.SetDescriptiveHint(chosenHint);

            // Subtract one of the descriptive hints balance.
            WordConnectState.UseDescriptiveHint();
            // Game state updated, invoke the event for UI to respond accordingly.
            StateUpdated?.Invoke(WordConnectState);
        }


        /// <summary>
        /// Checks whether the inputted word is valid and is in this game layout.
        /// </summary>
        public void SubmitWord()
        {
            // Word input is empty or null.
            if (WordConnectState == null || WordConnectState.CurrentWordInput == null || WordConnectState.CurrentWordInput.Count == 0) return;

            string spelledWord = WordConnectState.GetCurrentWordInput().ToLower();
            // Check if the crossword contains the spelled word.
            if (WordConnectState.WordsInCrossword.Contains(spelledWord, StringComparer.OrdinalIgnoreCase))
            {
                // Check if this word hasn't already been found.
                if (WordConnectState.CorrectlyAddedWords.Contains(spelledWord, StringComparer.OrdinalIgnoreCase))
                {
                    // Word has already been found, clear the streak.
                    WordConnectState.ClearStreak();
                }
                else
                {
                    GamePlayHandler.Instance.soundFindNewWord.Play();
                    GamePlayHandler.Instance.SpawnParticle((float)GameTimer.ElapsedTime);
                    GamePlayHandler.Instance.AddWordFound(spelledWord);
                    // Word has been found, add the found word to the state object.
                    WordConnectState.AddFoundWord(spelledWord);
                    RemoveFoundWord(spelledWord);
                    // Remove the word from the available hint options.
                    WordConnectState.RemoveHintOption(spelledWord);
                    // Add score for finding the word.
                    WordConnectState.AddScore(Configuration.ScorePerWordFound);
                    WordConnectState.AddStreakScore(Configuration.StreakScoreIncrement * WordConnectState.CorrectAnswerStreak);
                    WordConnectState.IncreaseStreak(1);
                }
            }
            else
            {
                GamePlayHandler.Instance.soundIncorrect.Play();
                WordConnectState.ClearStreak();
            }

            // Clear the input sequence.
            WordConnectState.ClearInput();
            StateUpdated?.Invoke(WordConnectState);

            if (CheckGameComplete())
                // All words in the game have been found, finish the game.
                ForceFinish();
        }

        void RemoveFoundWord(string word)
        {
            StartCoroutine(RemoveFoundWordAfterDealy(word));
        }

        IEnumerator RemoveFoundWordAfterDealy(string word)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = filledData.Count - 1; i >= 0; i--)
            {
                if (filledData[i].word == word)
                {
                    filledData.RemoveAt(i);
                }
            }

            SaveHintsToFile();
        }
        public void SubmitWordStart(string spelledWord)
        {



            // Check if the crossword contains the spelled word.
            if (WordConnectState.WordsInCrossword.Contains(spelledWord, StringComparer.OrdinalIgnoreCase))
            {
                // Check if this word hasn't already been found.
                if (WordConnectState.CorrectlyAddedWords.Contains(spelledWord, StringComparer.OrdinalIgnoreCase))
                {
                    // Word has already been found, clear the streak.
                    WordConnectState.ClearStreak();
                }
                else
                {

                    // Word has been found, add the found word to the state object.
                    WordConnectState.AddFoundWord(spelledWord);
                    // Remove the word from the available hint options.
                    WordConnectState.RemoveHintOption(spelledWord);
                    // Add score for finding the word.
                    WordConnectState.AddScore(Configuration.ScorePerWordFound);
                    WordConnectState.AddStreakScore(Configuration.StreakScoreIncrement * WordConnectState.CorrectAnswerStreak);
                    WordConnectState.IncreaseStreak(1);
                }
            }
            else
            {
                WordConnectState.ClearStreak();
            }

            // Clear the input sequence.
            WordConnectState.ClearInput();
            StateUpdated?.Invoke(WordConnectState);

            if (CheckGameComplete())
                // All words in the game have been found, finish the game.
                ForceFinish();
        }
        /// <summary>
        /// Checks if all words in the game have been found.
        /// </summary>
        /// <returns>Bool whether all words have been found.</returns>
        private bool CheckGameComplete()
        {
            // If no words have been found, return.
            if (WordConnectState.CorrectlyAddedWords.Count == 0)
                return false;

            // Get all the words in the game.
            List<string> wordsInCrossword = WordConnectState.WordsInCrossword;

            // Remove all words that have been added from the list.
            foreach (string word in WordConnectState.CorrectlyAddedWords)
                wordsInCrossword.Remove(word);

            // Remove all words that have been completely revealed by hinting.
            foreach (HintOption hint in WordConnectState.CompletelyHintedWords)
                wordsInCrossword.Remove(hint.Word);

            // If the list has no words left, this indicates all words have either been found or completely hinted.
            return wordsInCrossword.Count == 0;
        }

        /// <summary>
        /// Returns a list of characters which are necessary to spell all words in the game.
        /// </summary>
        /// <param name="words">List of strings containing the words.</param>
        /// <returns>List of chars required to spell all words in the given list</returns>
        private List<char> GetAllLettersFromWords(List<string> words)
        {
            List<char> neededLetters = new List<char>();

            // All letters in first word can be added.
            foreach (char letter in words[0])
                neededLetters.Add(letter);

            // Loop over the remaining words.
            for (int i = 1; i < words.Count; i++)
            {
                foreach (char letter in words[i])
                {
                    if (neededLetters.Contains(letter))
                    {
                        // There is already at least 1 occurence of this letter in the list.
                        // Count the amount of times this letter appears in the letter list and in the current word.
                        int unaddedLetterOccurences = words[i].Count(letterInWord => (letterInWord == letter));
                        int addedLetterOccurences = neededLetters.Count(addedLetter => (addedLetter == letter));

                        // If the letter appears more times in the unadded word than the list we need more of this letter to spell all words. Add the remainder.
                        if (unaddedLetterOccurences > addedLetterOccurences)
                        {
                            for (int l = 0; l < unaddedLetterOccurences - addedLetterOccurences; l++)
                                neededLetters.Add(letter);
                        }
                    }
                    else
                    {
                        // The current letter list doesnt have the current letter. Add it.
                        neededLetters.Add(letter);
                    }
                }
            }
            return neededLetters;
        }

        /// <summary>
        /// Populates the LetterInput array whch represents which inputs are valid in this game.
        /// </summary>
        /// <param name="letters">List of characters representing the letters which we need to spell the words.</param>
        private void SetAvailableLetters(List<char> letters)
        {
            AvailableLetters = new List<LetterInput>();

            for (int i = 0; i < letters.Count; i++)
            {
                AvailableLetters.Add(new LetterInput(i, letters[i]));
            }
        }

        public void RevealLetters()
        {
            HintOption h = new HintOption("not", "");
            //WordConnectManager.Instance.RevealLetterHint(2, h);
            //WordConnectManager.Instance.RevealLetterHint(1, h);
            WordConnectState.SetHintLetterRevealed(h, 0);
            WordConnectState.SetHintLetterRevealed(h, 1);
        }

        int BoolToInt(bool isTrue) => isTrue ? 1 : 0;
        bool IntToBool(int val) => val == 1;
    }
    [Serializable]
    public class HintsSaving
    {
        public int wordIndex;
        public string word;
        public int letterIndex;
    }


}
