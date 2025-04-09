using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamnetNotifier : MonoBehaviour
{
    public GameObject notifyImage;
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

        notifyImage.SetActive(show);
    }
}
