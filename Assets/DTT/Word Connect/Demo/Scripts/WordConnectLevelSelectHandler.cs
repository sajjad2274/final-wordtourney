using DTT.MinigameBase.LevelSelect;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace DTT.WordConnect.Editor
{

    /// <summary>
    /// Level select handler that allows for easy management of word connect level transitioning.
    /// </summary>
    public class WordConnectLevelSelectHandler : LevelSelectHandler<WordConnectConfigurationData, WordConnectResult, WordConnectManager>
    {
        /// <summary>
        /// The WordConnect configurations that are associated to the different levels.
        /// </summary>
        [SerializeField]
        private WordConnectConfigurationData[] _wordConnectConfigurations;

        /// <summary>
        /// Calculates the score of a given Word Connect Result.
        /// </summary>
        /// <param name="result">The result data of the word connect level.</param>
        /// <returns>The calculated score for the level.</returns>
        protected override float CalculateScore(WordConnectResult result) => Mathf.Clamp01(Mathf.Round(result.finalScore * 3) / 3);

        /// <summary>
        /// Gets a configuration file based on a given level number from the level select screen.
        /// </summary>
        /// <param name="levelNumber">The level being fetched.</param>
        /// <returns>The word connect configuration that was fetched.</returns>
       // protected override WordConnectConfigurationData GetConfig(int levelNumber) => _wordConnectConfigurations[(levelNumber - 1) % _wordConnectConfigurations.Length];
        protected override WordConnectConfigurationData GetConfig()
        {
            // GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[PlayerPrefs.GetInt("levelsUnlocked", 0) % GameHandler.Instance.levels.Count];
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
                    GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[Random.Range(0, GameHandler.Instance.levels.Count)];
                }
            }
            else if (GameHandler.Instance.progressData.levelCompleted < GameHandler.Instance.levels.Count) GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[GameHandler.Instance.progressData.levelCompleted % GameHandler.Instance.levels.Count];
            else
            {
                GameHandler.Instance.progressData.levelCompleted = 0;
                GameHandler.Instance.config.DictionaryAsset = GameHandler.Instance.levels[GameHandler.Instance.progressData.levelCompleted % GameHandler.Instance.levels.Count];
            }
            return GameHandler.Instance.config;
        }
    }
}
