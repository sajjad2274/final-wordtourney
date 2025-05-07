
using UnityEngine;

public class NotificationBarScript : MonoBehaviour
{


    public UnityEngine.UI.Text notificationText;

    public void SetMsg(string msg)
    {
        notificationText.text = msg;
    }
}
