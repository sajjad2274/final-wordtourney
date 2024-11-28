using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TranslatedText))]
public class TranslateTextAndRemove : MonoBehaviour
{
    public string textValue;
    private Text uiText;
    TMP_Text tmpText;

    void OnValidate()
    {
        Debug.LogWarning("its working");
        if (GetComponent<TMP_Text>() != null)
            tmpText = GetComponent<TMP_Text>();
        else if (GetComponent<Text>() != null)
            uiText = GetComponent<Text>();

        // If the Text component exists, set its text to the string variable
        if (tmpText != null)
        {
            textValue = tmpText.text;
            GetComponent<TranslatedText>().keyString = textValue;
        }
        else if (uiText != null)
        {
            textValue = uiText.text;
            GetComponent<TranslatedText>().keyString = textValue;
        }
        else
        {
            Debug.LogWarning("No Text component found on this GameObject.");
        }
        RemoveThisScript();
    }
    void RemoveThisScript()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            DestroyImmediate(this, true);
        };
#endif
    }
}