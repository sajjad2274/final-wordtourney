using System.Collections;
using UnityEngine;
using Firebase.Firestore;
using System;
using Unity.VisualScripting;
using Firebase.Extensions;
using Unity.VisualScripting.Antlr3.Runtime;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance;
    public string[] AllTournamentNames;


    private FirebaseFirestore db;
    private Firebase.FirebaseApp app;
    public bool isTournamentSectionOpen = false;
    public GameObject tournamentNofication;

    public static Action<bool> NotifyTournament; 

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public static bool ShowTournamentPanel = false;
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        //{
        //    var dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        // Create and hold a reference to your FirebaseApp,
        //        // where app is a Firebase.FirebaseApp property of your application class.
        //        app = Firebase.FirebaseApp.DefaultInstance;

        //        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        //        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;



        //        Firebase.Messaging.FirebaseMessaging.MessageReceived += (sender, e) =>
        //        {
        //            if (e.Message.Data.ContainsKey("action") && e.Message.Data["action"] == "open_panel")
        //            {
        //                ShowTournamentPanel = true;
        //                UnityEngine.Debug.Log("Sadiq---------->OpenPanel");
        //            }
        //        };

        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.
        //    }
        //});





        StartCoroutine(CheckNewTournaments());


    }


    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received Registration Token: " + token.Token);

    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received a new message from: " + e.Message.From);
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

                    var currTime = DateTime.Now;

                    if (currTime >= startTime && currTime < endTime)
                    {
                        Debug.LogError("Tournament Available");
                        if (!isTournamentSectionOpen)
                        {
                            tournamentNofication.SetActive(true);
                            NotifyTournament?.Invoke(true);
                            isTournamentSectionOpen = true;
                        }
                     
                    }
                    else
                    {
                        NotifyTournament?.Invoke(false);
                        isTournamentSectionOpen = false;
                        tournamentNofication.SetActive(false);

                    }
                }
                else
            {

            }
        });

        FirebaseManager.Instance.LoadTournamentLevels();
        MainMenuHandler.Instance.StartFireStore();


    }

    // Coroutine to check for new tournaments every 5 minutes
    private IEnumerator CheckNewTournaments()
    {

        yield return new WaitForSeconds(2f); // 5 minutes


            FetchTournaments();

        while (true)
        {

            yield return new WaitForSeconds(5); // 5 minutes

                FetchTournaments();


        }
    }



}

