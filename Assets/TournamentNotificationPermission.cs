
using UnityEngine;
using UnityEngine.Android;
public class TournamentNotificationPermission : MonoBehaviour
{

    public static TournamentNotificationPermission Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Canvas canvas;


    public void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
 if (AndroidVersion() >= 33)
        if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") == false)
        {
            ShowPanel(true);
        }
#endif

    }


    public void OnYes()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       if (AndroidVersion() >= 33)
        if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") == false)
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            ShowPanel(false);

        }
#endif
    }


    public void OnNo() {

        ShowPanel(false);
    }


    public void ShowPanel(bool show )
    {
        if (AndroidVersion() >= 33)
            canvas.enabled = show;
        else
            canvas.enabled = false;
    }

    private int AndroidVersion()
    {
        using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }

}
