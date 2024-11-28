using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AutoLocalization;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] Text[] btnsTxts;
    [SerializeField] string[] words;
    [SerializeField] Languages langs;
    [SerializeField]  TMP_Dropdown lcDropown;

    private void Start()
    {
        lcDropown.value = GameHandler.Instance.languageSelected;
    }
    public void OnChangedLanguage(int val)
    {
        setLanguage(lcDropown.options[val].text);
    }
    public void setLanguage(string lang)
    {
        GameHandler.Instance.languageSelected = lcDropown.value;
        switch (lang)
        {
            case "arabic":
                langs = Languages.Arabic;

                break;
            case "chinese":
                langs = Languages.Chinese;

                break;
            case "dutch":
                langs = Languages.Dutch;

                break;
            case "english":
                langs = Languages.English;

                break;
            case "filipino":
                langs = Languages.Filipino;

                break;
            case "french":
                langs = Languages.French;

                break;
            case "german":
                langs = Languages.German;

                break;
            case "hindi":
                langs = Languages.Hindi;

                break;
            case "indonesian":
                langs = Languages.Indonesian;

                break;
            case "italian":
                langs = Languages.Italian;

                break;
            case "japanese":
                langs = Languages.Japanese;

                break;
            case "portuguese":
                langs = Languages.Portuguese;

                break;
            case "russian":
                langs = Languages.Russian;

                break;
            case "spanish":
                langs = Languages.Spanish;

                break;
            case "turkish":
                langs = Languages.Turkish;

                break;
            default:
                break;
        }
        changeAll();
    }


    public void changeAll()
    {
        for(int i =0;i<btnsTxts.Length;i++)
        {

            changeOne(words[i], btnsTxts[i]);
        }
      
    }
    public void changeOne(string wrd,Text btn)
    {
        btn.text = LanguageManager.instance.GetMeaning(wrd, langs);
    }

}

[System.Serializable]
public class LanguageCountries
{
    public string languageName;
    public int languageNumber;
    public List<string> languageCountries;

}
