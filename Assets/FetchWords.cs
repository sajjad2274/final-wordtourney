using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class FetchWords : MonoBehaviour
{
    public static string newJsonString;
    public static bool isNewJsonReceived;
    private const int MaxLevels = 100;
    private const int BaseWordsPerLevel = 3;
    private const int MaxWordsPerLevel = 5;
    private List<string> allWords;

    private DatabaseReference databaseReference;
    private string url;

    // Custom difficulty levels fetched from Firebase JSON
    private List<DifficultySetting> difficultySettings;

    IEnumerator Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                StartCoroutine(GetCustomDifficultySettingsFromJson());
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
        yield return new WaitForSeconds(0);
    }

    private IEnumerator GetCustomDifficultySettingsFromJson()
    {
        DatabaseReference customDiffRef = databaseReference.Child("OfficialData").Child("difficultySettingsJson");
        var diffTask = customDiffRef.GetValueAsync();
        yield return new WaitUntil(() => diffTask.IsCompleted);

        if (diffTask.IsFaulted)
        {
            Debug.LogError("Failed to get custom difficulty levels from Firebase.");
            yield break;
        }

        DataSnapshot snapshot = diffTask.Result;
        if (snapshot.Exists)
        {
            string difficultyJson = snapshot.Value.ToString();
            var difficultyData = JsonConvert.DeserializeObject<DifficultyData>(difficultyJson);
            difficultySettings = difficultyData.difficultySettings;

            StartCoroutine(GetUrlAndFetchWords(((Enums.Languages)LocalSettings.SelectedLanguage).ToString()));
        }
        else
        {
            Debug.LogError("Custom difficulty data does not exist in Firebase.");
        }
    }

    private IEnumerator GetUrlAndFetchWords(string key)
    {
        DatabaseReference officialDataRef = databaseReference.Child("OfficialData").Child(key);
        var urlTask = officialDataRef.GetValueAsync();
        yield return new WaitUntil(() => urlTask.IsCompleted);

        if (urlTask.IsFaulted)
        {
            Debug.LogError("Failed to get data from Firebase.");
            yield break;
        }

        DataSnapshot snapshot = urlTask.Result;
        if (snapshot.Exists)
        {
            url = snapshot.Value.ToString();
            Debug.Log($"URL for {key}: {url}");

            yield return StartCoroutine(LoadWordsFromUrl(url));
            var anagramGroups = GetAnagramGroups(allWords);
            var wordLists = CreateWordLists(anagramGroups);
            SaveToJsonFile(wordLists, key);
        }
        else
        {
            Debug.LogError($"Key '{key}' does not exist in the database.");

            url = "https://firebasestorage.googleapis.com/v0/b/word-tourney-ef3b5.appspot.com/o/English.txt?alt=media&token=3f44bf4d-0ff8-4b1d-9fb8-a6115e619b8f";
            Debug.Log($"URL for {key}: {url}");

            yield return StartCoroutine(LoadWordsFromUrl(url));
            var anagramGroups = GetAnagramGroups(allWords);
            var wordLists = CreateWordLists(anagramGroups);
            SaveToJsonFile(wordLists, key);
        }
    }

    private IEnumerator LoadWordsFromUrl(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching words from URL: {webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                allWords = new List<string>();

                var lines = text.Split('\n');
                foreach (var line in lines)
                {
                    try
                    {
                        string word = line.Trim();
                        if (!string.IsNullOrEmpty(word) && word.All(char.IsLetter))
                        {
                            allWords.Add(word);
                        }
                        else
                        {
                            Debug.LogWarning($"Skipping invalid word: {line}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error processing line: {line}. Skipping... Exception: {ex.Message}");
                    }
                }

                allWords = allWords.Distinct().ToList(); // Deduplicate words
                Debug.Log($"Loaded {allWords.Count} valid words from the URL.");
            }
        }
    }

    private List<List<string>> GetAnagramGroups(List<string> words)
    {
        var anagramGroups = words
            .GroupBy(word => new string(word.Trim().OrderBy(c => c).ToArray()))
            .Where(group => group.Count() > 1)
            .Select(group => group.Distinct().ToList()) // Deduplicate within each group
            .ToList();

        return anagramGroups;
    }

    private List<WordList> CreateWordLists(List<List<string>> anagramGroups)
    {
        var wordLists = new List<WordList>();
        var allAnagramGroups = anagramGroups
            .Where(group => group.All(word => word.Length >= 3 && word.Length <= 10))
            .ToList();

        var usedWords = new HashSet<string>();
        int level = 1;

        while (level <= MaxLevels)
        {
            int wordsRequired;
            int minWordLength;

            var customDifficulty = difficultySettings.FirstOrDefault(d => level >= d.StartDifficultyLevel && level <= d.EndDifficultyLevel);
            if (customDifficulty != null)
            {
                wordsRequired = customDifficulty.TotalWord;
                minWordLength = customDifficulty.TotalLetter;
            }
            else
            {
                wordsRequired = Mathf.Min(BaseWordsPerLevel + (level - 1) / 10, MaxWordsPerLevel);
                minWordLength = Math.Min(3 + (level - 1) / 10, 10);
            }

            var eligibleAnagramGroups = allAnagramGroups
                .Where(group => group.All(word => word.Length >= minWordLength))
                .ToList();

            if (eligibleAnagramGroups.Count == 0)
            {
                break;
            }

            var selectedGroup = eligibleAnagramGroups.FirstOrDefault(group => group.Count >= wordsRequired);

            if (selectedGroup == null)
            {
                break;
            }

            var uniqueWords = selectedGroup.Except(usedWords).Take(wordsRequired).ToList();
            if (uniqueWords.Count < wordsRequired)
            {
                break;
            }

            var wordList = new WordList
            {
                level = level++,
                words = uniqueWords
            };
            wordLists.Add(wordList);

            foreach (var word in uniqueWords)
            {
                usedWords.Add(word);
            }

            allAnagramGroups.Remove(selectedGroup);
        }

        return wordLists;
    }

    private void SaveToJsonFile(List<WordList> wordLists, string language)
    {
        try
        {
            string filePath = $"Assets/Resources/{language}_words.json";
            var json = JsonConvert.SerializeObject(wordLists, Formatting.Indented);
            newJsonString = json;
            isNewJsonReceived = true;
            File.WriteAllText(filePath, json);

            Debug.Log($"Data saved to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data: {ex.Message}");
        }
    }

    [System.Serializable]
    public class WordList
    {
        public int level;
        public List<string> words;
    }

    [System.Serializable]
    public class DifficultySetting
    {
        public int StartDifficultyLevel;
        public int EndDifficultyLevel;
        public int TotalLetter;
        public int TotalWord;
    }

    [System.Serializable]
    public class DifficultyData
    {
        public List<DifficultySetting> difficultySettings;
    }
}
