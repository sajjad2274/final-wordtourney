using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LanguageChangerMI : MonoBehaviour
{
    TMP_Dropdown languageDropDown;
    Dropdown languageDropDownSimple;

    private void Start()
    {
        if (GetComponent<TMP_Dropdown>() != null)
        {
            languageDropDown = GetComponent<TMP_Dropdown>();
            PopulateDropdownWithEnum<Enums.Languages>();
            languageDropDown.value = LocalSettings.SelectedLanguage;
        }
        else if (GetComponent<Dropdown>() != null)
        {
            languageDropDownSimple = GetComponent<Dropdown>();
            PopulateDropdownWithEnum<Enums.Languages>();
            //Debug.LogError("default ")
            languageDropDownSimple.value = LocalSettings.SelectedLanguage;
           
        }

    }
    private void OnEnable()
    {
     
        OnChangelanguageItemChange();
    }
    public void OnChangelanguageItemChange()
    {
        if (languageDropDownSimple != null)
            languageDropDownSimple.value = LocalSettings.SelectedLanguage;
        else if (languageDropDown != null)
            languageDropDown.value = LocalSettings.SelectedLanguage;
    }
    public bool isOnetimerun = false;
    private GameObject obj;
    public void OnValueChange()
    {
        if (languageDropDown != null)
        {
            Enums.currentLanguage = (Enums.Languages)languageDropDown.value;
            LocalizationManagerCustom.Instance.MethodsOnLanguageChange?.Invoke();
            LocalSettings.SelectedLanguage = languageDropDown.value;
          
        }
        else
        {
            Enums.currentLanguage = (Enums.Languages)languageDropDownSimple.value;
            LocalizationManagerCustom.Instance.MethodsOnLanguageChange?.Invoke();
            LocalSettings.SelectedLanguage = languageDropDownSimple.value;
            if (isOnetimerun)
            {
                
                GameObject obj = GameObject.Find("GameHandler");
                if (obj != null)
                {
                    Destroy(obj);
                }
                SceneManager.LoadScene(0);
            }
            isOnetimerun = true;
        }
    }

    #region Population of dropdown
    void PopulateDropdownWithEnum<T>() where T : Enum
    {
        if (languageDropDown != null)
            languageDropDown.ClearOptions();
        else languageDropDownSimple.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(T)));
        if (languageDropDown != null)
            languageDropDown.AddOptions(enumNames);
        else
            languageDropDownSimple.AddOptions(enumNames);
    }
    #endregion
}
