using System.Collections;
using UnityEngine;
using Firebase.Firestore;
using System.Linq;
using System;
using Unity.VisualScripting;
using Firebase.Extensions;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance;
    public string[] AllTournamentNames;


    private FirebaseFirestore db;
    public bool isTournamentSectionOpen = false;
    public GameObject tournamentNofication;

    public static Action<bool> NotifyTournament; 

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        StartCoroutine(CheckNewTournaments());
    }

    // Call this when entering the tournament section


    // Fetch latest tournaments immediately
    private void FetchTournaments()
    {

        //  foreach (var tName in AllTournamentNames)

            db.Collection("Tournaments").Document("Beginner").Collection("Detail").Document("PrimaryDetail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {

                    DocumentSnapshot snapshot = task.Result;
                    var data = snapshot.ToDictionary();
                    var startTime = data["StartDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime();
                    var endTime = data["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime();

                    //Debug.LogError(endTime + "Tournament Available" + DateTime.Now);

                    if (DateTime.Now < endTime)
                    {
                        Debug.LogError("Tournament Available");
                        tournamentNofication.SetActive(true);
                        NotifyTournament?.Invoke(true);
                        isTournamentSectionOpen = true;
                    }
                    else
                    {
                        NotifyTournament?.Invoke(false);
                    }
                }
                else
            {

            }
        });

    }

    // Coroutine to check for new tournaments every 5 minutes
    private IEnumerator CheckNewTournaments()
    {
        while (true)
        {

            yield return new WaitForSeconds(3); // 5 minutes

            if (!isTournamentSectionOpen)
            {
                FetchTournaments();
            }


        }
    }



}
