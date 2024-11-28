using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentWaitingPanel : MonoBehaviour
{
    public Text tournamentTitleTxt;
    public Text tournamentTimeTxt;
    public Text tournamentProceedBtnTxt;

    public Coroutine coroutineTimer;
    TournamentDetailContainer tournamentDatail;
    DateTime timeTournament;
    public void Init(DateTime time, string tName, TournamentDetailContainer td)
    {
        tournamentDatail = td;
        timeTournament = time;
        if (coroutineTimer != null)
        {
            StopCoroutine(coroutineTimer);
        }
        tournamentTimeTxt.text = "Wait";
        tournamentTitleTxt.text = tName;
       
        MainMenuHandler.Instance.OpenPanel(this.gameObject);
    }
    private void OnEnable()
    {
        if (timeTournament > DateTime.Now)
        {
            coroutineTimer = StartCoroutine(WaiterRoutine(timeTournament));
            tournamentProceedBtnTxt.text = "Back";
        }
        else
        {
            tournamentProceedBtnTxt.text = "Play";
        }
    }
    private void OnDisable()
    {
        if (coroutineTimer != null)
        {
            StopCoroutine(coroutineTimer);
        }
    }
    public void Proceed()
    {
        if(timeTournament<=DateTime.Now)
        {
            tournamentDatail.OnHandleClick();
        }
        else
        {
            if (coroutineTimer != null)
            {
                StopCoroutine(coroutineTimer);
            }
        }
        MainMenuHandler.Instance.ClosePanel(this.gameObject);
    }
    IEnumerator WaiterRoutine(DateTime time)
    {
        yield return null;
        TimeSpan timeDifference = time - DateTime.Now;

        // Get the total difference in seconds
        double totalSeconds = timeDifference.TotalSeconds;
        tournamentTimeTxt.text = FormatTime(totalSeconds);
        while (time > DateTime.Now)
        {
            yield return new WaitForSeconds(1f);
            totalSeconds--;
            tournamentTimeTxt.text = "Tournament Starts In "+FormatTime(totalSeconds);

        }
        tournamentTimeTxt.text = "Started";
        tournamentProceedBtnTxt.text = "Play";
    }
    public static string FormatTime(double totalSeconds)
    {
        // Round the total seconds to the nearest integer
        int roundedSeconds = Mathf.RoundToInt((float)totalSeconds);

        int hours = roundedSeconds / 3600;
        int minutes = (roundedSeconds % 3600) / 60;
        int seconds = roundedSeconds % 60;

        // Format the time as HH:MM:SS
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
    public static string FormatTime(float totalSeconds)
    {
        // Round the total seconds to the nearest integer
        int roundedSeconds = Mathf.RoundToInt(totalSeconds);

        int hours = roundedSeconds / 3600;
        int minutes = (roundedSeconds % 3600) / 60;
        int seconds = roundedSeconds % 60;

        // Format the time as HH:MM:SS
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
