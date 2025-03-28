using Firebase.Messaging;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Notifications.Android;
using UnityEngine.Playables;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;



    public static bool ShowTournamentPanel = false;


    private void Awake()
    {
        Instance = this;
    }

   public void ScheduleTournamnetNotification(string tournamentName, DateTime dateTime)
    {

        var Id = tournamentName.ToLower() + "_channel";
        var newTime = dateTime.AddMinutes(-1);

        if (PlayerPrefs.GetString(Id, "") == "")
        {
           
            PlayerPrefs.SetString(Id, newTime.ToString());
            CreateNotificationChannel(Id);
            ScheduleNotification(Id,newTime);
            return;
        }
        if(PlayerPrefs.GetString(Id, "")!= newTime.ToString())
        {
            PlayerPrefs.SetString(Id, "");
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            PlayerPrefs.SetString(Id, newTime.ToString());
            CreateNotificationChannel(Id);
            ScheduleNotification(Id, newTime);
        }      
    }

    void CreateNotificationChannel(string id)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = id,
            Importance = Importance.High,
            Description = "Generic notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void ScheduleNotification(string id,DateTime fireTIme)
    {
        var notification = new AndroidNotification()
        {
            Title = "Tournament Reminder!",
            Text = "Don't forget! Your tournament starts soon.",
            FireTime = fireTIme
        };
        AndroidNotificationCenter.SendNotification(notification, id);
    }

    public void Init()
    {

        ShowTournamentPanel = false;
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
               
                Debug.Log("Sadiq --------------------> Notification Clicked!" + notificationIntent.Notification.Title+"     "+ notificationIntent.Id);
                PlayerPrefs.SetString(notificationIntent.Id.ToString(), "");
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

 



    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received Registration Token: " + token.Token);

    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Sadiq---------->Received a new message from: " + e.Message.From);


        if (e.Message.Data.ContainsKey("action") && e.Message.Data["action"] == "open_panel")
        {
            ShowTournamentPanel = true;
            UnityEngine.Debug.Log("Sadiq---------->OpenPanel");
        }
    }

}
