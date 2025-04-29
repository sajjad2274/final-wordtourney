using Firebase.Messaging;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;
using System;
using Unity.Notifications.Android;
using Firebase.Firestore;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Android;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public static bool ShowTournamentPanel = false;

    private float ScheduleTimeOffset = -5;



    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

    }

    private void Start()
    {

       


        AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;
    }

    public void ScheduleTournamnetNotification(string tournamentName, DateTime startTime, DateTime endTime)
    {
        TournamentManager.Log($"{tournamentName}-> Start: {startTime}  -  End: {endTime}");


        var Id = tournamentName.ToLower() + "_channel";

        var currTime = DateTime.Now;

        var newTime = startTime.AddMinutes(ScheduleTimeOffset);




        if (PlayerPrefs.GetString(Id, "") == "")
        {

            if (currTime > newTime)
            {
                PlayerPrefs.SetString(Id, "");
                PlayerPrefs.SetString("SaveNotificationId_" + Id, "");
                TournamentManager.Log($"{tournamentName}--------------------> Time Over!");
                return;

            }


            TournamentManager.Log($"{tournamentName}--------------------> Notification Schedule!    " + newTime);
            var notId = ScheduleNotification(Id, newTime);
            PlayerPrefs.SetString(Id, newTime.ToString());
            PlayerPrefs.SetString("SaveNotificationId_" + Id, notId.ToString());
            return;
        }

        if (PlayerPrefs.GetString(Id, "") != newTime.ToString())
        {

            if (currTime > newTime)
            {
                var notId1 = PlayerPrefs.GetString("SaveNotificationId_" + Id, "");

                if (notId1 != "")              
                    AndroidNotificationCenter.CancelNotification(int.Parse(notId1));
                
                PlayerPrefs.SetString(Id, "");
                PlayerPrefs.SetString("SaveNotificationId_" + Id, "");
                TournamentManager.Log($"{tournamentName} --------------------> Time Over2!");
                return;

            }

            TournamentManager.Log($"{tournamentName} --------------------> Notification Again!" + newTime);
            PlayerPrefs.SetString(Id, newTime.ToString());

            var notId = PlayerPrefs.GetString("SaveNotificationId_" + Id, "");

            if (notId != "")
            {
                AndroidNotificationCenter.CancelNotification(int.Parse(notId));
            }
            var newNotId = ScheduleNotification(Id, newTime);
            PlayerPrefs.SetString("SaveNotificationId_" + Id, newNotId.ToString());
        }
    }



    private int ScheduleNotification(string id, DateTime fireTIme)
    {

        var channel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = id,
            Importance = Importance.High,
            Description = "Generic notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        var notification = new AndroidNotification()
        {
            Title = "Tournament Reminder!",
            Text = "Don't forget! Your tournament starts soon.",
            FireTime = fireTIme
        };
        return AndroidNotificationCenter.SendNotification(notification, id);
    }

    public void Init()
    {

      //  ShowTournamentPanel = false;
        //Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        //Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        //UnityEngine.Debug.Log("Sadiq---------->Messagae---Init");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (Application.isEditor)
            return;


        if (hasFocus)
        {
            var notificationIntent = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntent != null)
            {

                Debug.LogError(notificationIntent.Channel + "    Sadiq --------------------> Notification Clicked!" + notificationIntent.Notification.Title + "     " + notificationIntent.Id);

                PlayerPrefs.SetString(notificationIntent.Channel.ToString(), "");

                AndroidNotificationCenter.CancelNotification(notificationIntent.Id);
                // Open your panel or take any action here

                ShowTournamentPanel = true;
            }
        }
    }


    private void OnApplicationPause(bool pause)
    {
        if (Application.isEditor)
            return;



        //if (!pause)
        //{
        //    var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //    var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        //    var action = intent.Call<string>("getStringExtra", "action");
        //    UnityEngine.Debug.Log("Sadiq---------->Messagae--" + action);
        //    if (action == "open_panel")
        //    {
        //        ShowTournamentPanel = true;
        //    }
        //}
    }

    private void OnNotificationReceived(AndroidNotificationIntentData data)
    {
        if (data.Notification.ShowInForeground)
        {
            Debug.LogError("Notification clicked while app was running: " + data.Notification.Title);
           // MainMenuHandler.Instance?.LoadTournamentPanel();
        }

    }


    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        AndroidNotificationCenter.OnNotificationReceived -= OnNotificationReceived;
    }


    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received Registration Token: " + token.Token);

    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received a new message from: " + e.Message.From);


        if (e.Message.Data.ContainsKey("action") && e.Message.Data["action"] == "open_panel")
        {
         //   ShowTournamentPanel = true;
            UnityEngine.Debug.Log("Sadiq---------->OpenPanel");
        }
    }

}


