using System.Collections.Generic;
using UnityEngine;
public static class LocalSettings
{
    const string selectedLanguage = "selectedlanguage";
    const string isFirstTimeLanguageChanges = "first_time_lang_changed";
    public static int SelectedLanguage
    {
        get { return PlayerPrefs.GetInt(selectedLanguage, 7); }
        set { PlayerPrefs.SetInt(selectedLanguage, value); }
    }

    public static bool isLanguageSelected
    {
        get { return PlayerPrefs.HasKey(selectedLanguage); }
    }

    public static bool IsFirstTimeLanguageChange
    {
        get { return PlayerPrefs.GetInt(isFirstTimeLanguageChanges) == 1 ? true : false; }
        set { PlayerPrefs.SetInt(isFirstTimeLanguageChanges, value == true ? 1 : 0); }
    }
    public static void SetPosAndRect(GameObject InstantiatedObj, RectTransform ALReadyObjPos, Transform Parentobj)
    {
        InstantiatedObj.transform.parent = Parentobj;
        RectTransform MyPlayerRectTransform = InstantiatedObj.GetComponent<RectTransform>();
        MyPlayerRectTransform.localScale = ALReadyObjPos.localScale;
        MyPlayerRectTransform.localPosition = ALReadyObjPos.localPosition;
        MyPlayerRectTransform.anchorMin = ALReadyObjPos.anchorMin;
        MyPlayerRectTransform.anchorMax = ALReadyObjPos.anchorMax;
        MyPlayerRectTransform.anchoredPosition = ALReadyObjPos.anchoredPosition;
        MyPlayerRectTransform.sizeDelta = ALReadyObjPos.sizeDelta;
        MyPlayerRectTransform.localRotation = ALReadyObjPos.localRotation;
    }
}