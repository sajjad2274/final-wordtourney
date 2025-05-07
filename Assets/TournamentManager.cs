using System.Collections;
using UnityEngine;
using Firebase.Firestore;
using System.Linq;
using System;
using Unity.VisualScripting;
using Firebase.Extensions;
using System.Collections.Generic;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance;
    public string[] AllTournamentNames;
    public List<string> ActiveTournamentNames = new List<string>();


    private FirebaseFirestore db;
    public GameObject tournamentNofication;

    public static Action<bool> NotifyTournament; 

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);

        foreach (var tName in AllTournamentNames)
            PlayerPrefs.SetInt(tName + "_isTournamentSectionOpen", 0);


    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        StartCoroutine(CheckNewTournaments());
    }

    void ListAllDocuments(string collectionName)
    {
        return;

        db.Collection(collectionName).Listen(snapshot =>
        {
            GameHandler.Instance.tournamentLevels = new List<TournamentLevels>();

            // Convert IEnumerable<DocumentSnapshot> to List<DocumentSnapshot>
            var documentsList = snapshot.Documents.ToList();

            foreach (var document in documentsList) {
                Debug.Log("collectionName " + document.Id);

            }


        });
    }
    private void FetchTournaments()
    {

        foreach (var tName in AllTournamentNames)
        {
            db.Collection("Tournaments").Document(tName).Collection("Detail").Document("PrimaryDetail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {

                    DocumentSnapshot snapshot = task.Result;
                    var data = snapshot.ToDictionary();
                    var startTime = data["StartDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime();
                    var endTime = data["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime();


                    //Debug.LogError(endTime + "Tournament Available" + DateTime.Now);

                    var currTime = DateTime.Now;

                    if (currTime >= startTime && currTime < endTime)
                    {
                        Log("Tournament Available: "+ tName);
                        if (PlayerPrefs.GetInt(tName+ "_isTournamentSectionOpen",0)==0)
                        {
                            PlayerPrefs.SetInt(tName + "_isTournamentSectionOpen", 1);
                            if (!ActiveTournamentNames.Contains(tName))
                               ActiveTournamentNames.Add(tName);

                            NotifyTournament?.Invoke(ActiveTournamentNames.Count > 0);
                        }
                    }
                    else
                    {
                        PlayerPrefs.SetInt(tName + "_isTournamentSectionOpen", 0);

                        if (ActiveTournamentNames.Contains(tName))
                            ActiveTournamentNames.Remove(tName);

                        NotifyTournament?.Invoke(ActiveTournamentNames.Count > 0);

                    }

                    //tournamentNofication.SetActive(ActiveTournamentNames.Count > 0);
                    NotificationManager.Instance.ScheduleTournamnetNotification(tName, startTime, endTime);

                }
                else
                {

                }
            });
        }
    }

    // Coroutine to check for new tournaments every 5 minutes
    private IEnumerator CheckNewTournaments()
    {

        yield return new WaitForSeconds(2f); // 5 minutes
                                             //    ListAllDocuments("Tournaments");
        ListAllDocuments("Tournaments");
        FetchTournaments();

        while (true)
        {

            yield return new WaitForSeconds(5); // 5 minutes

            FetchTournaments();
            ListAllDocuments("Tournaments");



        }
    }


    public static void Log(string data)
    {
        string log =$"<color=#FFD900>{data}</color>";
        Debug.Log(log);
    }


}


