using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamnetNotifier : MonoBehaviour
{
    public GameObject notifyImage;
    public GameObject notifyImageMatched;


    private void Update()
    {
        if (NotificationManager.Instance != null)
        {
            NotifyTournament(NotificationManager.Instance.saveNotifierStatus);
        }
    }


    private void NotifyTournament(bool show)
    {
     
        notifyImage.SetActive(show);
    }


    private void NotifyTournament(string tName,bool show)
    {

        notifyImageMatched.SetActive(show);
    }
}
