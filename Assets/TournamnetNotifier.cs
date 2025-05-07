using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamnetNotifier : MonoBehaviour
{
    public GameObject notifyImage;
    public GameObject notifyImageMatched;
    private void OnEnable()
    {
        TournamentManager.NotifyTournament += NotifyTournament;
    }
    private void OnDisable()
    {
        TournamentManager.NotifyTournament -= NotifyTournament;
    }

    private void NotifyTournament(bool show)
    {
        TournamentManager.Log("TournamnetNotifier Shown "+show);
        if (show)
        {
            NotificationController.Instance.AddNotification("Tournament Started!");
        }
        notifyImage.SetActive(show);
    }


    private void NotifyTournament(string tName,bool show)
    {

        notifyImageMatched.SetActive(show);
    }
}
