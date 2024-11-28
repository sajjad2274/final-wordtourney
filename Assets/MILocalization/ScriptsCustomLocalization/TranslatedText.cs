using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslatedText : MonoBehaviour
{
    #region Variables
    public string keyString;
    TMP_Text tmpLabel;
    Text simpleTxt;

    Enums.Languages locallang;

    #endregion

    #region Unity Event functions
    private void OnEnable()
    {
        if (LocalizationManagerCustom.Instance == null)
            return;
        if (locallang != Enums.currentLanguage)
            StartCoroutine(FillTextField());
        LocalizationManagerCustom.Instance.MethodsOnLanguageChange += actionAdding;
    }

    private void OnDisable()
    {
        if (LocalizationManagerCustom.Instance)
            LocalizationManagerCustom.Instance.MethodsOnLanguageChange -= actionAdding;
    }
    #endregion


    #region Translating and filling text field
    void actionAdding()
    {
        StartCoroutine(FillTextField());
    }
    IEnumerator FillTextField()
    {

        if (tmpLabel == null && simpleTxt == null)
        {
            GetTextField();
        }
        if (keyString == "")
        {
            if (tmpLabel != null)
                keyString = tmpLabel.text;
            else if (simpleTxt != null)
                keyString = simpleTxt.text;
        }
        if (!isLanguageSupported)
        {
            if (simpleTxt == null)
            {
                Color clr = tmpLabel.color;
                float fontsize = tmpLabel.fontSize;
                bool isbestFit = tmpLabel.autoSizeTextContainer;
                TextAlignmentOptions tmpAlignment = tmpLabel.alignment;
                if (tmpLabel != null)
                {
                    DestroyImmediate(tmpLabel);
                }
                yield return new WaitForEndOfFrame();
                simpleTxt = gameObject.AddComponent<Text>();
                simpleTxt.font = LocalizationManagerCustom.Instance.simpleTextFont;
                simpleTxt.resizeTextForBestFit = isbestFit;
                simpleTxt.fontSize = (int)fontsize;
                simpleTxt.color = clr;
                simpleTxt.alignment = ConvertAlignment(tmpAlignment);
                tmpLabel = null;
            }
        }

        if (tmpLabel != null)
        {
            string translatedText = LocalizationManagerCustom.Instance.GetTranslation(keyString);
            yield return new WaitForSeconds(0.1f);
            if (!string.IsNullOrWhiteSpace(translatedText))
                tmpLabel.text = translatedText;
        }
        else if (simpleTxt != null)
        {
            string translatedText = LocalizationManagerCustom.Instance.GetTranslation(keyString);
            yield return new WaitForSeconds(0.1f);
            if (!string.IsNullOrWhiteSpace(translatedText))
                simpleTxt.text = translatedText;
        }
        else
            Debug.LogError("No text component attached on: " + name);
        locallang = Enums.currentLanguage;
    }

    private void GetTextField()
    {
        if (tmpLabel == null)
        {
            if (GetComponent<TMP_Text>() != null)
                tmpLabel = GetComponent<TMP_Text>();
            else if (GetComponent<Text>() != null)
                simpleTxt = GetComponent<Text>();
        }
    }

    bool isLanguageSupported
    {
        get
        {
            return Enums.currentLanguage != Enums.Languages.แบบไทย && Enums.currentLanguage != Enums.Languages.日本語 && Enums.currentLanguage != Enums.Languages.简体中文 && Enums.currentLanguage != Enums.Languages.繁體中文 && Enums.currentLanguage != Enums.Languages.한국인;
        }
    }

    #endregion


    #region Alignment Getter
    TextAnchor ConvertAlignment(TextAlignmentOptions tmpAlignment)
    {
        switch (tmpAlignment)
        {
            case TextAlignmentOptions.TopLeft:
                return TextAnchor.UpperLeft;
            case TextAlignmentOptions.Top:
                return TextAnchor.UpperCenter;
            case TextAlignmentOptions.TopRight:
                return TextAnchor.UpperRight;
            case TextAlignmentOptions.Left:
                return TextAnchor.MiddleLeft;
            case TextAlignmentOptions.Center:
                return TextAnchor.MiddleCenter;
            case TextAlignmentOptions.Right:
                return TextAnchor.MiddleRight;
            case TextAlignmentOptions.BottomLeft:
                return TextAnchor.LowerLeft;
            case TextAlignmentOptions.Bottom:
                return TextAnchor.LowerCenter;
            case TextAlignmentOptions.BottomRight:
                return TextAnchor.LowerRight;
            case TextAlignmentOptions.BaselineLeft:
                return TextAnchor.MiddleLeft;
            case TextAlignmentOptions.Baseline:
                return TextAnchor.MiddleCenter;
            case TextAlignmentOptions.BaselineRight:
                return TextAnchor.MiddleRight;
            case TextAlignmentOptions.MidlineLeft:
                return TextAnchor.MiddleLeft;
            case TextAlignmentOptions.Midline:
                return TextAnchor.MiddleCenter;
            case TextAlignmentOptions.MidlineRight:
                return TextAnchor.MiddleRight;
            default:
                return TextAnchor.MiddleCenter;
        }
    }
    #endregion
}

