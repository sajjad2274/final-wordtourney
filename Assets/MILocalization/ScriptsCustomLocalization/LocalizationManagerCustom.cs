using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LocalizationManagerCustom : MonoBehaviour
{
    #region Variables
    private Dictionary<string, Dictionary<string, string>> localizationDictionary;
    public TextAsset localizationCSV;
    public Action MethodsOnLanguageChange;
    public Font simpleTextFont;

    public FlagsContainer flagsContainer;
    public FlagProperties flagPropertiesPrefab;
    public GameObject languageListPanelPrefab;
    #endregion

    #region Creating Instance;
    private static LocalizationManagerCustom _instance;
    public static LocalizationManagerCustom Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LocalizationManagerCustom>();
                if (_instance != null)
                    DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public string Getlanguage;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        InitializeLocalization();


        Enums.currentLanguage = (Enums.Languages)LocalSettings.SelectedLanguage;

        Debug.LogError("defaulte language index...." + Enums.currentLanguage.ToString());
        Getlanguage = Enums.currentLanguage.ToString();
    }
    #endregion

    #region Parsing and initialization of CSV file

    void InitializeLocalization()
    {
        localizationDictionary = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        // Load the CSV file
        if (localizationCSV != null)
        {
            string[] lines = SplitCSV(localizationCSV.text);
            if (lines.Length > 0)
            {
                string[] headers = SplitCSVLine(lines[1]);
                for (int i = 2; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] fields = SplitCSVLine(lines[i]);
                    string key = fields[0];

                    Dictionary<string, string> translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    for (int j = 1; j < fields.Length; j++)
                    {
                        if (headers[j].Trim().Length > 0 && fields[j].Trim().Length > 0)
                        {
                            translations.Add(headers[j].Trim(), fields[j].Trim());
                        }
                    }

                    localizationDictionary.Add(key, translations);
                }
            }
        }
    }

    #endregion

    #region CSV Parsing Helpers
    // Split CSV file into lines
    string[] SplitCSV(string csvText)
    {
        // Split into lines, handling different newline types
        return Regex.Split(csvText, "\r\n|\r|\n");
    }

    // Split CSV line into fields
    string[] SplitCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string field = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (c == '"' && inQuotes)
            {
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    field += '"';
                    i++; // Skip the next quote
                }
                else
                {
                    inQuotes = false;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field);
                field = "";
            }
            else
            {
                field += c;
            }
        }

        if (!string.IsNullOrEmpty(field))
        {
            fields.Add(field);
        }

        return fields.ToArray();
    }
    #endregion

    #region Get translation of specific word
    public string GetTranslation(string currentVariable)
    {
        string language = Enums.currentLanguage.ToString();

        if (localizationDictionary.ContainsKey(currentVariable) && localizationDictionary[currentVariable].ContainsKey(language))
        {
            return localizationDictionary[currentVariable][language];
        }
        else
        {
            Debug.LogWarning("Translation not found for key: " + currentVariable + " and language: " + language);
            return null; // Or return some default string
        }
    }
    #endregion
}