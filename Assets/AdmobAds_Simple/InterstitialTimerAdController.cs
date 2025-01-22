using System;
using UnityEngine;


public class InterstitialTimerAdController : MonoBehaviour
{
        
    
    [SerializeField] private float _delay = 30.0f;
    [SerializeField]   private float _timer = 0.0f;
        
    private  bool _startTimer = false;
    private  bool _halt = false;
    private void Awake()
    {
        ResetTimerAd();
    }

        
    private void LateUpdate()
    {
        
        if (!_startTimer)
            return; 
            
    
        if (_halt)
            return;

            

        if (_timer <= 0)
        {
            _halt = true;
            ShowInterstitial();
            return;
        }
        _timer -= Time.deltaTime;
    }
        
   
    public  void StartTimerAd()
    {
        _startTimer = true;
    }
        
    public void StopTimerAd()
    {
        _startTimer = false;
    }

     
    public  void ResetTimerAd()
    {
        _timer = _delay;
    }

     


    private void ShowInterstitial()
    {
        GooglesAdsController.Instance.ShowInterstitialAd(() =>
        {
            Time.timeScale = 1;
            ResetTimerAd();
            _halt = false;
        });
    }
}