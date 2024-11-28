using UnityEngine;
using UnityEngine.UI;

public class LanguageListSummary : MonoBehaviour
{
    [SerializeField] Image flagImg;
    [SerializeField] Text originalLangTxt;
    [SerializeField] Text localLangTxt;
    [SerializeField] GameObject checkMark;

    int languageIndex;
    LanguageSelectionList languageSelectionList;
    public void FillListFields(int langIndex, LanguageSelectionList lsl)
    {
        languageSelectionList = lsl;
        FlagsContainer fc = LocalizationManagerCustom.Instance.flagsContainer;
        flagImg.sprite = fc.flagsDetaila[langIndex].flagImage;
        originalLangTxt.text = fc.flagsDetaila[langIndex].language;
        localLangTxt.text = fc.flagsDetaila[langIndex].englishLanguage;
        localLangTxt.gameObject.AddComponent<TranslatedText>();
        localLangTxt.GetComponent<TranslatedText>().keyString = fc.flagsDetaila[langIndex].englishLanguage;
        checkMark.SetActive(LocalSettings.SelectedLanguage == langIndex);
        languageIndex = langIndex;
    }

    public void ChangeLang()
    {
        languageSelectionList.ChangeLanguage(languageIndex);
    }

    public void SetCheckMark(bool istrue)
    {
        checkMark.SetActive(istrue);
    }
}
