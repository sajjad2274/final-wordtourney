using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NotificationController : MonoBehaviour
{

    public static NotificationController Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public List<NotificationBarScript> allNotificationBarScripts = new List<NotificationBarScript>();
    public NotificationBarScript notificationBarScriptPrefab;
    public float delayBetween = 1f;

    [ContextMenu("Add")]
    public void TestAdd()
    {
        AddNotification("Sadiq");
    }


    private Coroutine _showRoutine;

    public void AddNotification(string msg)
    {
        NotificationBarScript notificationBarScript = Instantiate(notificationBarScriptPrefab,transform);
        notificationBarScript.gameObject.SetActive(false);
        notificationBarScript.SetMsg(msg);
        allNotificationBarScripts.Add(notificationBarScript);
        
        if(_showRoutine==null)
            _showRoutine = StartCoroutine(ShowNotification());
        
    }

    private  IEnumerator ShowNotification()
    {
        while (allNotificationBarScripts.Count > 0)
        {
            NotificationBarScript notificationBar = allNotificationBarScripts[0];
            notificationBar.gameObject.SetActive(true);
            

            yield return new WaitForSeconds(delayBetween);
            if (notificationBar)
            {
                allNotificationBarScripts.Remove(notificationBar);
                Destroy(notificationBar.gameObject, 1f);
            }

        }
        StopCoroutine(_showRoutine);
        _showRoutine = null;
    }

    [ContextMenu("Clean")]
    public void CleanNotifications()
    {
        foreach (var notification in allNotificationBarScripts)
        {
            Destroy(notification.gameObject);
        }
        allNotificationBarScripts.Clear();
    }
}