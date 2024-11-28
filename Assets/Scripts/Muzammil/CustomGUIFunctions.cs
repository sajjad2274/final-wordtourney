#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomGUIFunctions : MonoBehaviour
{


    [MenuItem("Custom Functions/TMP to Text")]
    public static void TMPToText()
    {
        string textString;
        Color txtColor;
        bool isOutline;
        bool isBold;
        int outLineWidth = 0;
        float fontSize;
        bool isAutoSize;
        TextAnchor legacyAlignment;
        Font font = Resources.Load<Font>("Baloo-Regular2");
        Transform[] allChildren = GetAllTransformsRecursive(Selection.activeTransform);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.GetComponent<TMP_Text>())
            {
                Debug.Log("Obj name: " + child.gameObject.name);
                TMP_Text tmptxt = child.GetComponent<TMP_Text>();
                textString = tmptxt.text;
                txtColor = tmptxt.color;
                if (tmptxt.outlineWidth > 0)
                {
                    isOutline = true;
                    outLineWidth = (int)tmptxt.outlineWidth;
                }
                else
                {
                    isOutline = false;
                }
                isBold = tmptxt.isUsingBold;
                legacyAlignment = ConvertToLegacyAlignment(tmptxt.alignment);
                fontSize = tmptxt.fontSize;
                isAutoSize = tmptxt.enableAutoSizing;

                DestroyImmediate(tmptxt);

                child.gameObject.AddComponent<Text>();
                Text txt = child.GetComponent<Text>();
                txt.font = font;
                txt.text = textString;
                txt.color = txtColor;
                txt.fontSize = (int)fontSize;
                txt.resizeTextForBestFit = isAutoSize;
                txt.alignment = legacyAlignment;
                if (isOutline)
                {
                    txt.gameObject.AddComponent<Outline>();
                    txt.gameObject.GetComponent<Outline>().effectDistance = new Vector2(outLineWidth, outLineWidth);
                }

            }
        }
    }
    public static Transform[] GetAllTransformsRecursive(Transform root)
    {
        // Create a list to hold all the transforms
        System.Collections.Generic.List<Transform> allTransforms = new System.Collections.Generic.List<Transform>();

        // Use a stack to iterate over all children
        System.Collections.Generic.Stack<Transform> stack = new System.Collections.Generic.Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Transform current = stack.Pop();
            allTransforms.Add(current);

            // Push all children onto the stack
            foreach (Transform child in current)
            {
                stack.Push(child);
            }
        }

        return allTransforms.ToArray();
    }

    private static TextAnchor ConvertToLegacyAlignment(TextAlignmentOptions tmpAlignment)
    {
        switch (tmpAlignment)
        {
            case TextAlignmentOptions.Left: return TextAnchor.MiddleLeft;
            case TextAlignmentOptions.Center: return TextAnchor.MiddleCenter;
            case TextAlignmentOptions.Right: return TextAnchor.MiddleRight;
            case TextAlignmentOptions.TopLeft: return TextAnchor.UpperLeft;
            case TextAlignmentOptions.Top: return TextAnchor.UpperCenter;
            case TextAlignmentOptions.TopRight: return TextAnchor.UpperRight;
            case TextAlignmentOptions.BottomLeft: return TextAnchor.LowerLeft;
            case TextAlignmentOptions.Bottom: return TextAnchor.LowerCenter;
            case TextAlignmentOptions.BottomRight: return TextAnchor.LowerRight;
            default: return TextAnchor.MiddleCenter; // Default alignment
        }
    }

    // Validate the menu item defined by the function above.
    // The menu item will be disabled if this function returns false.
    [MenuItem("Custom Functions/TMP to Text", true)]
    static bool ValidateTMPToText()
    {
        // Return false if no transform is selected.
        return Selection.activeTransform != null;
    }




    [MenuItem("Custom Functions/Text Setting")]
    public static void TextSetting()
    {
        Transform[] allChildren = GetAllTransformsRecursive(Selection.activeTransform);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.GetComponent<Text>())
            {
                Text txt = child.GetComponent<Text>();

                txt.resizeTextForBestFit = true;
                txt.alignment = TextAnchor.MiddleCenter;

                if (!txt.gameObject.GetComponent<Outline>())
                    txt.gameObject.AddComponent<Outline>();
                txt.gameObject.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
                txt.gameObject.GetComponent<Outline>().effectColor = Color.black;
            }
        }
    }

    [MenuItem("Custom Functions/Text Setting", true)]
    static bool ValidateTextSetting()
    {
        // Return false if no transform is selected.
        return Selection.activeTransform != null;
    }




    [MenuItem("Custom Functions/All Texts Strings")]
    public static void TextStrings()
    {
        string allStrings = "";
        Transform[] allChildren = GetAllTransformsRecursive(Selection.activeTransform);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.GetComponent<Text>())
            {
                //allStrings += (child.GetComponent<Text>().text + "\n");
                if(!child.GetComponent<TranslatedText>())
                child.gameObject.AddComponent<TranslatedText>();
                child.GetComponent<TranslatedText>().keyString = child.GetComponent<Text>().text;
            }
        }
        Debug.LogError(allStrings);
    }

    [MenuItem("Custom Functions/All Texts Strings", true)]
    static bool ValidateTextStrings()
    {
        // Return false if no transform is selected.
        return Selection.activeTransform != null;
    }
}
#endif