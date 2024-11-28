using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DTT.WordConnect;
using Gley.Localization;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    // public string url;

    public GameProgressData progressData;

    public MyGameData defaultGameData;
    public WordConnectConfigurationData defaultConfig;

    public List<WordHintDictionary> levels;
    public List<TournamentLevels> tournamentLevels;
    public WordConnectConfigurationData config;
    public ColorTheme colorTheme;
    public int WordHintsAvailable;
    public int LetterHintsAvailable;
    public int DescriptionHintsAvailable;
    public int ExpectedTimeCompletion;
    public int MaxBonusTimeScore;
    public int ScorePerWordFound;
    public int StreakScoreIncrement;
    public bool PrioritizeLongestWordHint;

    public TournamentPlayerData tournamentCurrentPlayerData;
    public int tournamentCurrentPrizeDistributionCount = 1;
    public int tournamentCurrentDefaultPrize = 1;
    public List<TournamentPrizes> TournamentCurrentPrizeDistributionCategory;

    public bool isTournament;
    public string pID;
    public bool music, sound;

    public string username;



    public bool fireCrackerUsed;
    public bool fireCrackerPowerPriceUsed;

    public int languageSelected;

    public int soundThemeUsed;

    public bool isLoggedIn;

    public bool spinWheelOpened;

    public string countryName;
    public string countryLanguage;

    public string refereralLink = "https://o-circle-games.itch.io/word-count";

    public LanguageCountries[] languageCountries;

    //public SupportedLanguages currentLanguage;
    public Enums.Languages currentLanguage;
    public List<SupportedLanguages> allLanguages;

    public bool openStorePanel;
    public bool openCashPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AssignConfigurationFile(ref config);
        config.CompletionPointTimeCurve = new AnimationCurve();
        config.CompletionPointTimeCurve = defaultConfig.CompletionPointTimeCurve;
        //config.CompletionPointTimeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        //config.CompletionPointTimeCurve.preWrapMode = WrapMode.Default;
        //config.CompletionPointTimeCurve.postWrapMode = WrapMode.Default;
        Application.runInBackground = true;
    }
    private void Start()
    {
        //if (PlayerPrefs.HasKey("clang"))
        //{
        //    API.SetCurrentLanguage(allLanguages[PlayerPrefs.GetInt("clang")]);

        //}
        //else
        //{
        //    CheckPhoneLanguage();
        //}

        if (!LocalSettings.isLanguageSelected)
            Invoke(nameof(CheckPhoneLanguage), 0.1f);

        openStorePanel = false;
        openCashPanel = false;
    }
    public void CheckPhoneLanguage()
    {

        SystemLanguage systemLanguage = Application.systemLanguage;
        // systemLanguage = SystemLanguage.Spanish;

        Debug.Log("System Language: " + systemLanguage);
        // add more checks for more language
        //System by default
        switch (systemLanguage)
        {
            //case SystemLanguage.Azerbaijani:
            //    currentLanguage = Enums.Languages.Azərbaycan;
            //    break;
            case SystemLanguage.Bulgarian:
                currentLanguage = Enums.Languages.български;
                break;
            case SystemLanguage.Portuguese:
                currentLanguage = Enums.Languages.Portugues_do_Brasil;
                break;
            //case SystemLanguage.Croatian:
            //    currentLanguage = Enums.Languages.Hrvatski;
            //    break;
            case SystemLanguage.Czech:
                currentLanguage = Enums.Languages.čeština;
                break;
            case SystemLanguage.Danish:
                currentLanguage = Enums.Languages.Dansk;
                break;
            case SystemLanguage.Dutch:
                currentLanguage = Enums.Languages.Nederlands;
                break;
            case SystemLanguage.English:
                currentLanguage = Enums.Languages.English;
                break;
            //case SystemLanguage.Filipino:
            //    currentLanguage = Enums.Languages.Filipino;
            //    break;
            case SystemLanguage.Finnish:
                currentLanguage = Enums.Languages.Suomalainen;
                break;
            case SystemLanguage.French:
                currentLanguage = Enums.Languages.Français;
                break;
            case SystemLanguage.German:
                currentLanguage = Enums.Languages.Deutsch;
                break;
            case SystemLanguage.Greek:
                currentLanguage = Enums.Languages.Ελληνικά;
                break;
            case SystemLanguage.Hebrew:
                currentLanguage = Enums.Languages.עִברִית;
                break;
            case SystemLanguage.Hungarian:
                currentLanguage = Enums.Languages.Magyar;
                break;
            case SystemLanguage.Icelandic:
                currentLanguage = Enums.Languages.íslenskur;
                break;
            case SystemLanguage.Indonesian:
                currentLanguage = Enums.Languages.Bahasa;
                break;
            case SystemLanguage.Italian:
                currentLanguage = Enums.Languages.Italiana;
                break;
            case SystemLanguage.Japanese:
                currentLanguage = Enums.Languages.日本語;
                break;
            //case SystemLanguage.Kazakh:
            //    currentLanguage = Enums.Languages.қазақ;
            //    break;
            case SystemLanguage.Korean:
                currentLanguage = Enums.Languages.한국인;
                break;
            case SystemLanguage.Latvian:
                currentLanguage = Enums.Languages.latviski;
                break;
            case SystemLanguage.Lithuanian:
                currentLanguage = Enums.Languages.lietuvių;
                break;
            //case SystemLanguage.Malay:
            //    currentLanguage = Enums.Languages.Melayu;
            //    break;
            case SystemLanguage.Norwegian:
                currentLanguage = Enums.Languages.norsk;
                break;
            case SystemLanguage.Polish:
                currentLanguage = Enums.Languages.Polski;
                break;
            //case SystemLanguage.Portuguese:
            //    currentLanguage = Enums.Languages.Português;
            //    break;
            case SystemLanguage.Romanian:
                currentLanguage = Enums.Languages.Română;
                break;
            case SystemLanguage.Russian:
                currentLanguage = Enums.Languages.Русский;
                break;
            case SystemLanguage.ChineseSimplified:
                currentLanguage = Enums.Languages.简体中文;
                break;
            case SystemLanguage.Slovak:
                currentLanguage = Enums.Languages.slovenský;
                break;
            case SystemLanguage.Slovenian:
                currentLanguage = Enums.Languages.Slovenščina;
                break;
            case SystemLanguage.Spanish:
                currentLanguage = Enums.Languages.Española;
                break;
            case SystemLanguage.Swedish:
                currentLanguage = Enums.Languages.svenska;
                break;
            case SystemLanguage.Thai:
                currentLanguage = Enums.Languages.แบบไทย;
                break;
            case SystemLanguage.ChineseTraditional:
                currentLanguage = Enums.Languages.繁體中文;
                break;
            case SystemLanguage.Turkish:
                currentLanguage = Enums.Languages.Türkçe;
                break;
            case SystemLanguage.Ukrainian:
                currentLanguage = Enums.Languages.українська;
                break;
            //case SystemLanguage.Uzbek:
            //    currentLanguage = Enums.Languages.ozbek;
            //    break;
            case SystemLanguage.Vietnamese:
                currentLanguage = Enums.Languages.Tiếng_Việt;
                break;
            default:
                currentLanguage = Enums.Languages.English;
                break;
        }
        Enums.currentLanguage = currentLanguage;
        LocalSettings.SelectedLanguage = ((int)Enums.currentLanguage);
    }
    public void InitDefaultGameData(MyGameData gd)
    {
        for (int i = 0; i < gd.levels.Length; i++)
        {
            WordHintDictionary newLevel = new WordHintDictionary();
            AssignLevel(ref newLevel);
            newLevel.DictionaryName = "Level" + (i + 1).ToString();
            newLevel.WordHintPairs = new WordHintPair[gd.levels[i].words.Length];
            for (int j = 0; j < gd.levels[i].words.Length; j++)
            {
                WordHintPair wp = new WordHintPair();
                AssignWord(ref wp);
                newLevel.WordHintPairs[j] = wp;
                newLevel.WordHintPairs[j].Word = gd.levels[i].words[j].word;
                newLevel.WordHintPairs[j].Hint = "";
            }
            levels.Add(newLevel);
        }
        config.DictionaryAsset = levels[0];
        config.DescriptionHintsAvailable = 100;
        config.LetterHintsAvailable = 100;
        config.WordHintsAvailable = 100;
    }
    public static void AssignLevel(ref WordHintDictionary le)
    {

        le = ScriptableObject.CreateInstance<WordHintDictionary>();


    }
    public static void AssignWord(ref WordHintPair le)
    {
        le = ScriptableObject.CreateInstance<WordHintPair>();

    }
    public static void AssignConfigurationFile(ref WordConnectConfigurationData config)
    {
        config = ScriptableObject.CreateInstance<WordConnectConfigurationData>();
    }
    private void OnApplicationFocus(bool focus)
    {

    }
    public void LoadDefaultGameData()
    {
        InitDefaultGameData(defaultGameData);
    }
    public void CheckJson(string url, string newJsonString)
    {
        if (!string.IsNullOrEmpty(newJsonString))
        {
            Debug.LogError("Other API level");
            if (!string.IsNullOrEmpty(newJsonString) /*&& newJsonString == "ddd"*/)
            {
                JArray jlevels = JArray.Parse(newJsonString);

                // Iterate through each level
                Debug.LogError("Level: " + jlevels);
                foreach (JObject level in jlevels)
                {
                    // Access the 'level' and 'words' dynamically
                    int levelNumber = (int)level["level"];
                    JArray words = (JArray)level["words"];



                    WordHintDictionary newLevel = new WordHintDictionary();
                    AssignLevel(ref newLevel);
                    newLevel.DictionaryName = "Level" + (levelNumber).ToString();
                    newLevel.WordHintPairs = new WordHintPair[words.Count];
                    int j = 0;
                    foreach (string word in words)
                    {

                        WordHintPair wp = new WordHintPair();
                        AssignWord(ref wp);
                        newLevel.WordHintPairs[j] = wp;
                        newLevel.WordHintPairs[j].Word = word;
                        newLevel.WordHintPairs[j].Hint = "";
                        j++;
                    }
                    levels.Add(newLevel);
                }
                config.DictionaryAsset = levels[0];
                config.DescriptionHintsAvailable = 100;
                config.LetterHintsAvailable = 100;
                config.WordHintsAvailable = 100;
            }
            else
            {
                LoadDefaultGameData();
            }


        }
        else
        {
            using (var w = new WebClient())
            {
                Debug.LogError("Firebase API level");

                var json_data = string.Empty;
                try
                {
                    json_data = FetchWords.newJsonString;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }


                if (!string.IsNullOrEmpty(json_data))
                {
                    JArray jlevels = JArray.Parse(json_data);

                    // Iterate through each level
                    Debug.LogError("Level: " + jlevels);
                    foreach (JObject level in jlevels)
                    {
                        // Access the 'level' and 'words' dynamically
                        int levelNumber = (int)level["level"];
                        JArray words = (JArray)level["words"];



                        WordHintDictionary newLevel = new WordHintDictionary();
                        AssignLevel(ref newLevel);
                        newLevel.DictionaryName = "Level" + (levelNumber).ToString();
                        newLevel.WordHintPairs = new WordHintPair[words.Count];
                        int j = 0;
                        foreach (string word in words)
                        {

                            WordHintPair wp = new WordHintPair();
                            AssignWord(ref wp);
                            newLevel.WordHintPairs[j] = wp;
                            newLevel.WordHintPairs[j].Word = word;
                            newLevel.WordHintPairs[j].Hint = "";
                            j++;
                        }
                        levels.Add(newLevel);
                    }
                    config.DictionaryAsset = levels[0];
                    config.DescriptionHintsAvailable = 100;
                    config.LetterHintsAvailable = 100;
                    config.WordHintsAvailable = 100;
                }
                else
                {
                    LoadDefaultGameData();
                }

            }
        }
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    if (!pause)
    //    {
    //        RestartAndroid();
    //    }
    //}
    //private static void RestartAndroid()
    //{
    //    if (Application.isEditor) return;

    //    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //    {
    //        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    //        AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
    //        AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
    //        intent.Call<AndroidJavaObject>("setFlags", 0x20000000);//Intent.FLAG_ACTIVITY_SINGLE_TOP

    //        AndroidJavaClass pendingIntent = new AndroidJavaClass("android.app.PendingIntent");
    //        AndroidJavaObject contentIntent = pendingIntent.CallStatic<AndroidJavaObject>("getActivity", currentActivity, 0, intent, 0x8000000); //PendingIntent.FLAG_UPDATE_CURRENT = 134217728 [0x8000000]
    //        AndroidJavaObject alarmManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "alarm");
    //        AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
    //        long currentTime = system.CallStatic<long>("currentTimeMillis");
    //        alarmManager.Call("set", 1, currentTime + 1000, contentIntent); // android.app.AlarmManager.RTC = 1 [0x1]

    //        Debug.LogError("alarm_manager set time " + currentTime + 1000);
    //        currentActivity.Call("finish");

    //        AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
    //        int pid = process.CallStatic<int>("myPid");
    //        process.CallStatic("killProcess", pid);
    //    }
    //}
}
[System.Serializable]
public class TournamentLevels
{
    public List<WordHintDictionary> levels;
    public TournamentLevels()
    {
        levels = new List<WordHintDictionary>();
    }

}
[System.Serializable]
public class TournamentPrizes
{
    public List<TournamentPrize> prize;
    public TournamentPrizes()
    {
        prize = new List<TournamentPrize>();
    }
}
[System.Serializable]
public class TournamentPrize
{
    public string pType;
    public int pValue;
}
[System.Serializable]
public class TournamentRewards
{
    public string tournamentName;
    public string pType;
    public int pValue;
}
[System.Serializable]
public class TournamentHistory
{
    public string tournamentName;
    public string position;
    public string timeTaken;
    public int badgeInd;
    public DateTime endDate;
    public int entryFee;
    public int prize;
}