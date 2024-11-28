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

public class FetchWordsTournament : MonoBehaviour
{
    public static string newJsonString;
    public static bool isNewJsonReceived;
    private const int MaxLevels = 100; // Number of levels
    private const int BaseWordsPerLevel = 3; // Base number of words per level
    private const int MaxWordsPerLevel = 5; // Maximum number of words per level
    private List<string> allWords;

    public static FetchWordsTournament instance;

    private DatabaseReference databaseReference;
    private List<DifficultySetting> difficultySettings; // Custom difficulty levels

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                StartCoroutine(GetCustomDifficultySettingsFromJson()); // Fetch custom difficulty settings
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
        DatabaseReference customDiffRef = databaseReference.Child("OfficialData").Child("TournamentdifficultySettingsJson");
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
        }
        else
        {
            Debug.LogError("Custom difficulty data does not exist in Firebase.");
        }
    }

    public void LoadWordsFromExternalUrl(string url, string tournamentId)
    {
        StartCoroutine(LoadWordsFromUrl(url, tournamentId));
    }

    public IEnumerator LoadWordsFromUrl(string url, string tournamentId)
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
                var lines = text.Split('\n'); // Split into lines

                allWords = lines
                    .Select(line => line.Trim().ToLower()) // Trim and normalize to lowercase
                    .Where(word => !string.IsNullOrEmpty(word)) // Exclude empty words
                    .Distinct() // Remove duplicates
                    .ToList();

                Debug.Log($"Loaded {allWords.Count} unique valid words from the URL.");

                var anagramGroups = GetAnagramGroups(allWords);
                var wordLists = CreateWordLists(anagramGroups);
                SaveToJsonFile(wordLists, tournamentId); // Use tournamentId to create unique files
            }
        }
    }

    private List<List<string>> GetAnagramGroups(List<string> words)
    {
        var anagramGroups = words
            .GroupBy(word => new string(word.OrderBy(c => c).ToArray())) // Group by sorted letters
            .Select(group => group.Distinct().ToList()) // Ensure unique words in each group
            .Where(group => group.Count > 1) // Only keep groups with more than one word
            .ToList();

        return anagramGroups;
    }

    private List<WordList> CreateWordLists(List<List<string>> anagramGroups)
    {
        var wordLists = new List<WordList>();
        var usedWords = new HashSet<string>(); // Track used words

        var allAnagramGroups = anagramGroups
            .Where(group => group.All(word => word.Length >= 3 && word.Length <= 10)) // Filter groups by word length
            .ToList();

        int level = 1;

        while (level <= MaxLevels)
        {
            int wordsRequired;
            int minWordLength;

            // Apply custom difficulty for levels based on the settings
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
                break; // No more eligible anagram groups left to fill levels
            }

            var selectedGroup = eligibleAnagramGroups.FirstOrDefault(group => group.Count >= wordsRequired && group.All(word => !usedWords.Contains(word)));

            if (selectedGroup == null)
            {
                break; // No group with enough unique anagram words for this level
            }

            var wordList = new WordList
            {
                level = level++,
                words = selectedGroup.Take(wordsRequired).ToList()
            };
            wordLists.Add(wordList);

            // Mark these words as used
            foreach (var word in wordList.words)
            {
                usedWords.Add(word);
            }

            // Remove the used anagram group from future selections
            allAnagramGroups.Remove(selectedGroup);
        }

        return wordLists;
    }

    private void SaveToJsonFile(List<WordList> wordLists, string tournamentId)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, $"{tournamentId}_words.json");
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
