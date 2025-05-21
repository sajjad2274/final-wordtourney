using System.Collections.Generic;
using System;
using Unity.Notifications.Android;
using System.Linq;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public static bool ShowTournamentPanel = false;
    public bool saveNotifierStatus = false;

    public List<NotificationContent> NotificationContents=new List<NotificationContent> ();

    private float ScheduleTimeOffset = -1;



    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

    }

    private void Start()
    {

        AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;
    }




    private void Update()
    {
        if (NotificationContents == null || NotificationContents.Count == 0 || NotificationContents.Count < 5)
            return;

        DateTime currTime = DateTime.Now;
        bool anyActive = false;
        var activeTCount = 0;

        for (int i = 0; i < NotificationContents.Count; i++)
        {
            var notification = NotificationContents[i];

            notification.shouldSchedule = currTime < notification.NotificationScheduleTime;
            notification.active = currTime >= notification.StartTime && currTime < notification.EndTime;

            if (notification.active)
            {
                anyActive = true;

                var show = false;
                if (currTime >= notification.StartTime)
                {
                    TimeSpan diff = currTime - notification.StartTime;
                    if (diff.Minutes < 1)
                    {
                        show = true;
                    }

                    if (show && !notification.GetAppUiNotificationStatus() || !notification.GetAppUiNotificationStatus())
                    {
                        notification.SetAppUiNotificationStatus(true);
                        activeTCount++;
                    }
                }
            }
            else
            {
                notification.SetAppUiNotificationStatus(false);
            }
        }

        if(activeTCount > 0)
        {
            NotificationController.Instance.AddNotification(activeTCount > 1 ? "Tournaments Started!": "Tournament Started!");
            activeTCount = 0;
        }

        // Only update if status has changed
        if (anyActive != saveNotifierStatus)
        {
            saveNotifierStatus = anyActive;
        }
    }


    public void ScheduleTournamnetNotification(string tournamentName, DateTime startTime, DateTime endTime)
    {


        var Id = tournamentName.ToLower() + "_channel";
        var newTime = startTime.AddMinutes(ScheduleTimeOffset);


        NotificationContent match = NotificationContents
            .FirstOrDefault(n => n.TournamentName.Equals(tournamentName, StringComparison.OrdinalIgnoreCase));

        if (match != null)
        {
            match.TournamentName = tournamentName;
            match.NotificationId = Id;
         
            match.startTimeString=startTime.ToString();
            match.endTimeString=endTime.ToString();
            match.notificationScheduleString=newTime.ToString();
        }
        else
        {
            var notificationContent = new NotificationContent();
            notificationContent.TournamentName = tournamentName;
            notificationContent.NotificationId = Id;
            notificationContent.startTimeString = startTime.ToString();
            notificationContent.endTimeString = endTime.ToString();
            notificationContent.notificationScheduleString = newTime.ToString();
            NotificationContents.Add(notificationContent);
        }






        return;
        var currTime = DateTime.Now;

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
                ShowTournamentPanel = true;
                CancelAllNotifications();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (Application.isEditor)
            return;

        ScheduleNotifications();
    }

    private void ScheduleNotifications()
    {
        foreach (var notification in NotificationContents)
        {

            if (notification.shouldSchedule)
            {
                Debug.LogError($"Scheduled {notification.TournamentName} ");

                ScheduleNotification(notification.NotificationId, notification.NotificationScheduleTime);
                notification.shouldSchedule = false;
            }
        }
    }


    private void OnApplicationPause(bool pause)
    {
        //if (Application.isEditor)
        //    return;


        ScheduleNotifications();


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




    private void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
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

[System.Serializable]
public class NotificationContent
{
    public string TournamentName;
    public string NotificationId;
    public string startTimeString;
    public string endTimeString;
    public string notificationScheduleString;

    public bool shouldSchedule;
    public bool active;
    public bool IsAppUiNotificationShown;


    public DateTime StartTime => DateTime.Parse(startTimeString);
    public DateTime EndTime => DateTime.Parse(endTimeString);
    public DateTime NotificationScheduleTime => DateTime.Parse(notificationScheduleString);


    public bool GetAppUiNotificationStatus()
    {
        IsAppUiNotificationShown = PlayerPrefs.GetInt($"{TournamentName}-InAppUiNotificationShown", 0) == 1;
        return IsAppUiNotificationShown;
    }

    public void SetAppUiNotificationStatus(bool value)
    {
        IsAppUiNotificationShown = value;
        PlayerPrefs.SetInt($"{TournamentName}-InAppUiNotificationShown", value ? 1 : 0);

    }

}


