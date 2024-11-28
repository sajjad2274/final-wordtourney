using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlagProperties : MonoBehaviour
{
    public string language;
    public int langEnumIndex;
    [SerializeField] Image flagImg;
    [SerializeField] Text countryNameTxt;
    [SerializeField] GameObject highligher;
    LanguageSelectWithFlag languageSelectWithFlag;
    public void SetFlagFields(int index, LanguageSelectWithFlag lswf)
    {
        language = LocalizationManagerCustom.Instance.flagsContainer.flagsDetaila[index].language;
        flagImg.sprite = LocalizationManagerCustom.Instance.flagsContainer.flagsDetaila[index].flagImage;
        countryNameTxt.text = language;
        langEnumIndex = LocalizationManagerCustom.Instance.flagsContainer.flagsDetaila[index].LangEnumIndex;
        languageSelectWithFlag = lswf;
    }

    public void SelectLang()
    {
        Enums.currentLanguage = (Enums.Languages)langEnumIndex;
        LocalizationManagerCustom.Instance.MethodsOnLanguageChange?.Invoke();
        LocalSettings.SelectedLanguage = langEnumIndex;
        LocalSettings.IsFirstTimeLanguageChange = true;
        languageSelectWithFlag.HighlightSelectedFlag();
    }
    public void HighlightLanguageFlag(bool isTrue)
    {
        highligher.SetActive(isTrue);
    }
}
