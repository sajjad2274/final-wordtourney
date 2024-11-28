using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCanvasGroupAlpha : MonoBehaviour
{

    CanvasGroup canvasGroup;
    IEnumerator changeAlpha()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += (Time.deltaTime * 2);
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }
    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        StartCoroutine(changeAlpha());
    }

}
