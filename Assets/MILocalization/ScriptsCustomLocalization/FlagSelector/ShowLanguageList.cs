using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLanguageList : MonoBehaviour
{
    public void ShowLanguageListPanel()
    {
        GameObject panel = Instantiate(LocalizationManagerCustom.Instance.languageListPanelPrefab);
        panel.SetActive(true);
        LocalSettings.SetPosAndRect(panel, GetComponent<RectTransform>(), transform);
    }
}
