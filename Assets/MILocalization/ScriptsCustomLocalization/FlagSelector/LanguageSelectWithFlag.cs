using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelectWithFlag : MonoBehaviour
{
    public GameObject languageSelectionWithFlagPanel;
    public RectTransform flagContainerParent;
    List<FlagProperties> flagList;
    // Start is called before the first frame update
    void Start()
    {
        if (!LocalSettings.IsFirstTimeLanguageChange)
            languageSelectionWithFlagPanel.SetActive(true);
        else
            languageSelectionWithFlagPanel.SetActive(false);
        flagList = DestroyAndClearList(flagList);
        GameObject ob = LocalizationManagerCustom.Instance.flagPropertiesPrefab.gameObject;
        for (int i = 0; i < LocalizationManagerCustom.Instance.flagsContainer.flagsDetaila.Length; i++)
        {
            GameObject flagProp = Instantiate(ob);
            LocalSettings.SetPosAndRect(flagProp, flagContainerParent.transform.GetChild(0).GetComponent<RectTransform>(), flagContainerParent.transform);
            flagProp.SetActive(true);
            FlagProperties flagProps = flagProp.GetComponent<FlagProperties>();
            flagProps.SetFlagFields(i, this);
            flagList.Add(flagProps);
        }
        HighlightSelectedFlag();
    }

    public void ShowLanguageSelectWithFlags()
    {
        languageSelectionWithFlagPanel.SetActive(true);
        HighlightSelectedFlag();
    }
    public void HighlightSelectedFlag()
    {
        for (int i = 0; i < flagList.Count; i++)
            flagList[i].HighlightLanguageFlag(LocalSettings.SelectedLanguage == i);
    }
    public List<FlagProperties> DestroyAndClearList(List<FlagProperties> list)
    {
        if (list == null) return list = new List<FlagProperties>();
        if (list.Count == 0) return list;
        foreach (FlagProperties obj in list)
        {
            Destroy(obj.gameObject);
        }
        list.Clear();
        return list;
    }
}
