using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelectionList : MonoBehaviour
{
    public GameObject listItemPrefab;
    public GameObject parentObj;

    List<LanguageListSummary> languageListSummaries;
    // Start is called before the first frame update
    void Start()
    {
        oldIndex = LocalSettings.SelectedLanguage;
        languageListSummaries = new List<LanguageListSummary>();
        for (int i = 0; i < LocalizationManagerCustom.Instance.flagsContainer.flagsDetaila.Length; i++)
        {
            GameObject listItem = Instantiate(listItemPrefab);
            LocalSettings.SetPosAndRect(listItem, listItemPrefab.GetComponent<RectTransform>(), parentObj.transform);
            LanguageListSummary lls = listItem.GetComponent<LanguageListSummary>();
            languageListSummaries.Add(lls);
            lls.FillListFields(i, this);
            listItem.SetActive(true);
        }
    }

    int oldIndex;
    public void ChangeLanguage(int langIndex)
    {
        if (oldIndex == langIndex)
        {
            Debug.LogError("Language already changed ---------------");
            return;
        }
        else Debug.LogError("Now Changing Language");
        LocalSettings.SelectedLanguage = langIndex;
        Enums.currentLanguage = (Enums.Languages)langIndex;
        LocalizationManagerCustom.Instance.MethodsOnLanguageChange?.Invoke();

        for (int i = 0; i < languageListSummaries.Count; i++)
        {
            languageListSummaries[i].SetCheckMark(LocalSettings.SelectedLanguage == i ? true : false);
        }
        oldIndex = langIndex;
    }

    public void CloseBtn()
    {
        Destroy(gameObject);
    }
}
