using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class ScoreElement : MonoBehaviour
{

    public Text usernameText;
    public Text posText;
    public Text levelText;
    public Text cashText;
    public GameObject levelShowObj;
    public GameObject posShowObj;
    public Text timeText;
    public Sprite highlightBg;
    public Image medalImg;
    public Image cashImg;
    public Sprite keys;
    public Sprite gems;
    [Space]
    [Header("Medals 1st 3 positions")]
    public Sprite firstMedal;
    public Sprite secondMedal, thirdMedal;
    public void NewScoreElement(string _username, int _level, int _pos, int _cash)
    {
        Debug.LogError("here 1");
        if (_level == -1)
        {
            levelShowObj.SetActive(false);

        }
        usernameText.text = _username;
        if (_pos == 1)
        {
            medalImg.sprite = firstMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 2)
        {
            medalImg.sprite = secondMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 3)
        {
            medalImg.sprite = thirdMedal;
            posText.gameObject.SetActive(false);
        }
        posText.text = _pos.ToString();
        levelText.text = _level.ToString();
        cashText.text = _cash.ToString();
        if (highlightBg)
        {
            if (_username == GameHandler.Instance.username)
            {
                GetComponent<Image>().sprite = highlightBg;
            }
        }
        else
        {
            Debug.LogError("Highlight image is missing");
        }
    }
    public void NewHighestScoreElement(string _username, int cash, int _pos)
    {
        Debug.LogError("here 2");
        usernameText.text = _username;
        timeText.text = cash.ToString();
        if (_pos == 1)
        {
            medalImg.sprite = firstMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 2)
        {
            medalImg.sprite = secondMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 3)
        {
            medalImg.sprite = thirdMedal;
            posText.gameObject.SetActive(false);
        }
        posText.text = _pos.ToString();
        cashText.text=cash.ToString();
        if (highlightBg)
        {
            if (_username == GameHandler.Instance.username)
            {
                GetComponent<Image>().sprite = highlightBg;
            }
        }
        else
        {
            Debug.LogError("Highlight image is missing");
        }
    }
    public void NewScoreElement(string _username, int _level, int _pos)
    {
        Debug.LogError("here 3");
        if (_level == -1)
        {
            levelShowObj.SetActive(false);
        }
        usernameText.text = _username;
        if (_pos == 1)
        {
            medalImg.sprite = firstMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 2)
        {
            medalImg.sprite = secondMedal;
            posText.gameObject.SetActive(false);
        }
        else if (_pos == 3)
        {
            medalImg.sprite = thirdMedal;
            posText.gameObject.SetActive(false);
        }
        posText.text = _pos.ToString();
        levelText.text = _level.ToString();
        if (highlightBg)
        {
            if (_username == GameHandler.Instance.username)
            {
                GetComponent<Image>().sprite = highlightBg;
            }
        }
        else
        {
            Debug.LogError("Highlight image is missing");
        }

    }
    public void NewScoreElement(string _username, int _level, int _pos, float timeTaken, int cash)
    {
        if (_level == -1)
        {
            levelShowObj.SetActive(false);
            timeText.text = "";
            posShowObj.SetActive(false);
        }
        else
        {
            timeText.text = "TIME: " + timeTaken.ToString("F2") + "s";
        }
        usernameText.text = _username;
        switch (_pos)
        {
            case 1:
                medalImg.sprite = firstMedal;
                posText.gameObject.SetActive(false);
                //SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.firstPlaceSound);
                if (_username == GameHandler.Instance.username)
                    PlayerPrefs.SetInt("myPosition", 1);
                //StartCoroutine(playAudioClip(SoundManager.Instance.AllSounds.firstPlaceSound));
                break;
            case 2:
                medalImg.sprite = secondMedal;
                posText.gameObject.SetActive(false);
                cashImg.sprite = gems;
                //SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.secondPlaceSound);
                if (_username == GameHandler.Instance.username)
                    PlayerPrefs.SetInt("myPosition", 2);

                //StartCoroutine(playAudioClip(SoundManager.Instance.AllSounds.secondPlaceSound));
                break;
            case 3:
                medalImg.sprite = thirdMedal;
                posText.gameObject.SetActive(false);
                cashImg.sprite = keys;
                if (_username == GameHandler.Instance.username)
                    PlayerPrefs.SetInt("myPosition", 3);
                //StartCoroutine(playAudioClip(SoundManager.Instance.AllSounds.thirdPlaceSound));
                break;
        }
        //Debug.LogError("My position is:   " + PlayerPrefs.GetInt("myPosition"));
        posText.text = _pos.ToString();
        levelText.text = _level.ToString();
      //  cashText.text = MainMenuHandler.Instance.tCashss.ToString();
        cashText.text = cash.ToString();
        if (highlightBg)
        {
            if (_username == GameHandler.Instance.username)
            {
                GetComponent<Image>().sprite = highlightBg;
            }
        }
        else
        {
            Debug.LogError("Highlight image is missing");
        }
    }

    //IEnumerator playAudioClip(AudioClip clip)
    //{
    //    yield return new WaitForSeconds(totalDelay);
    //    SoundManager.Instance.PlayAudioClip(clip);

    //}

    float totalDelay;

    //public void PlayDoTweenAnimationWithDelay(float delay)
    //{
    //    totalDelay = transform.GetChild(0).gameObject.GetComponent<DOTweenAnimation>().delay + delay;
    //    //float dly = totalDelay;
    //    //transform.GetChild(0).gameObject.GetComponent<DOTweenAnimation>().delay = dly;
    //    //transform.GetChild(0).gameObject.GetComponent<DOTweenAnimation>().DOPlay();
    //}
}
