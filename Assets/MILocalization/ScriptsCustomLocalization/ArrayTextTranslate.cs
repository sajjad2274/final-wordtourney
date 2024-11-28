//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ArrayTextTranslate : MonoBehaviour
//{
//    TypingScript typingScript;
//    public string[] originalText;
//    Enums.Languages locallang;
//    #region Unity Event functions
//    private void OnEnable()
//    {
//        //if (locallang != Enums.currentLanguage)
//        actionAdding();
//        LocalizationManagerCustom.Instance.MethodsOnLanguageChange += actionAdding;
//    }

//    private void OnDisable()
//    {
//        if (LocalizationManagerCustom.Instance)
//            LocalizationManagerCustom.Instance.MethodsOnLanguageChange -= actionAdding;
//    }
//    #endregion
//    void actionAdding()
//    {
//        StartCoroutine(TranslateArrayFields());
//    }
//    IEnumerator TranslateArrayFields()
//    {
//        if (typingScript == null)
//        {
//            typingScript = GetComponent<TypingScript>();
//            if (typingScript != null)
//            {
//                originalText = new string[typingScript.sentences.Length];
//                for (int i = 0; i < typingScript.sentences.Length; i++)
//                    originalText[i] = typingScript.sentences[i];
//            }
//        }
//        if (typingScript != null)
//        {
//            for (int i = 0; i < typingScript.sentences.Length; i++)
//            {
//                string translatedText = LocalizationManagerCustom.Instance.GetTranslation(originalText[i]);
//                yield return new WaitForEndOfFrame();
//                if (!string.IsNullOrEmpty(translatedText))
//                    typingScript.sentences[i] = translatedText;
//            }
//            locallang = Enums.currentLanguage;
//        }
//    }
//}
