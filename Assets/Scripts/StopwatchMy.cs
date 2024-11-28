using DTT.WordConnect;
using UnityEngine;

public class StopwatchMy : MonoBehaviour
{
    public float ElapsedTime;
    public double RemainTime;

    public bool IsRunning;

    public bool IsTimer;

    

 
   
    public void Restart()
    {
        IsRunning = false;
        ElapsedTime = 0f;
        RemainTime = 0d;
        IsRunning = true;
    }
    public void StartWatch()
    {
        IsRunning = true;
    }
    public void Stop()
    {
        IsRunning = false;
    }
    private void Update()
    {
        if (IsRunning)
        {
            if(!IsTimer)
            {
                ElapsedTime += Time.unscaledDeltaTime;

            }
            else
            {
                if(RemainTime > 0f) RemainTime -= Time.unscaledDeltaTime;
                else if(RemainTime <= 0f)
                {
                    
                    IsRunning = false;
                    if(WordConnectManager.Instance!=null)
                    {
                        WordConnectManager.Instance.ForceFinish();
                        MainMenuHandler.isFromTournament = true;
                        GamePlayHandler.Instance.tournamentEndedpanel.SetActive(true);
                    }
                }
                
            }
        }
    }
}
