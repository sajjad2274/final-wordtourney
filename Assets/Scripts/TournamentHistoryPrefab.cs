using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TournamentHistoryPrefab : MonoBehaviour
{
    public Text completionDetail;
    public Text tournamentName;
    public Text timeTaken;
    public Text tournamentPlace;
    public Image badgeImg;
    public Text timeRemainingTxt;
    public Text prizeTxt;
    public Text entryFeeTxt;
    public Text timeTxt;
    public DateTime eDate;

    public Transform parentT;
    public void LoadData(string _completionDetail, string _tournamentName, string _tournamentPlace, string _timeTaken, int badgeIndex, int prize, DateTime endDate,int entryFee,Transform p)
    {
        parentT = p;
        completionDetail.text = _completionDetail;
        tournamentName.text = _tournamentName;
        timeTaken.text = _timeTaken;
        tournamentPlace.text = _tournamentPlace;
        badgeImg.sprite = MainMenuHandler.Instance.tournamentTypes[badgeIndex].tournamentBadgeSprite;
        entryFeeTxt.text = entryFee.ToString();
        prizeTxt.text = prize.ToString();
        eDate = endDate;
        this.gameObject.SetActive(true);
        if (completionDetail.text == "In Progress"&&gameObject.activeInHierarchy) StartCoroutine(WaiterRoutine(eDate));
    }
    void OnEnable()
    {
        if (completionDetail.text == "In Progress") StartCoroutine(WaiterRoutine(eDate));
        else if (completionDetail.text == "Completed")
        {
            timeTxt.text = "Ended";
            int referenceIndex = parentT.GetSiblingIndex();
            int newSiblingIndex = referenceIndex + 1;
            this.transform.SetSiblingIndex(newSiblingIndex);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator WaiterRoutine(DateTime startTime)
    {
        yield return null;

        TimeSpan timeDifference = startTime - DateTime.Now;
        double totalSeconds = timeDifference.TotalSeconds;

        timeTxt.text = "Time Left :" + FormatTimeInMinutes(totalSeconds);

        while (totalSeconds > 0)
        {
            yield return new WaitForSeconds(60f); // Wait for 1 minute intervals
            totalSeconds -= 60; // Decrease by 60 seconds (1 minute)
            timeTxt.text = "Time Left :" + FormatTimeInMinutes(totalSeconds);
          
        }
        completionDetail.text = "Completed";
        int referenceIndex = parentT.GetSiblingIndex();
        int newSiblingIndex = referenceIndex + 1;
        this.transform.SetSiblingIndex(newSiblingIndex);
        timeTxt.text = "";
    }

    // Helper method to format time in "minutes"
    private string FormatTimeInMinutes(double totalSeconds)
    {
        int minutes = Mathf.CeilToInt((float)totalSeconds / 60);
        return minutes.ToString() + " minutes";
    }
    //public static string FormatTime(double totalSeconds)
    //{
    //    // Round the total seconds to the nearest integer
    //    int roundedSeconds = Mathf.RoundToInt((float)totalSeconds);

    //    int hours = roundedSeconds / 3600;
    //    int minutes = (roundedSeconds % 3600) / 60;
    //    int seconds = roundedSeconds % 60;

    //    // Format the time as HH:MM:SS
    //    return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    //}
}
