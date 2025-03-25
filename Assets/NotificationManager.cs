using Firebase.Messaging;
using Firebase.Messaging;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;



    public static bool ShowTournamentPanel = false;


    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {

        ShowTournamentPanel = false;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        UnityEngine.Debug.Log("Sadiq---------->Messagae---Init");

       

    }


    private void OnApplicationPause(bool pause)
    {
        if (Application.isEditor)
            return;

        if (!pause)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            var action = intent.Call<string>("getStringExtra", "action");
            UnityEngine.Debug.Log("Sadiq---------->Messagae--" + action);
            if (action == "open_panel")
            {
                ShowTournamentPanel = true;
            }
        }
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
