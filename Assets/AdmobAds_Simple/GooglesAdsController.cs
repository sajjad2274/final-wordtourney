using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class GooglesAdsController : MonoBehaviour
{
    public static GooglesAdsController Instance;
    public static Action<bool> OnAdsInitialized;
    
    public static bool NoAdsPurchased
    {
        get=> PlayerPrefs.GetInt("MFS_NoAds", 0) == 1;
        set=> PlayerPrefs.SetInt("MFS_NoAds", value ? 1 : 0);
    }

    
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }
    public bool testIds;
    
    //public GameObject adIsComing;


    private AppTrackingTransparencyManager _trackingTransparencyManager;
    private CustomGDPR _customGdpr;
    private GoogleAdsAppOpenController _googleAdsAppOpenController;
    private GoogleAdsBannerController _googleAdsBannerController;
    private GoogleAdsInterstitialController _googleAdsInterstitialController;
    private GoogleAdsRewardedController _rewardedController;
    private InterstitialTimerAdController _interstitialTimerAdController;
    
    private void Start()
    {
#if UNITY_EDITOR
        //adIsComing.GetComponent<Canvas>().sortingOrder = 0;
#endif
        
        _trackingTransparencyManager = GetComponent<AppTrackingTransparencyManager>();
        _customGdpr = GetComponent<CustomGDPR>();
        if (_trackingTransparencyManager==null)
            _trackingTransparencyManager = gameObject.AddComponent<AppTrackingTransparencyManager>();
        
        if (_customGdpr==null)
            _customGdpr = gameObject.AddComponent<CustomGDPR>();
        
        _googleAdsAppOpenController = GetComponent<GoogleAdsAppOpenController>();
        _googleAdsBannerController = GetComponent<GoogleAdsBannerController>();
        _googleAdsInterstitialController = GetComponent<GoogleAdsInterstitialController>();
        _rewardedController = GetComponent<GoogleAdsRewardedController>();
        _interstitialTimerAdController = GetComponent<InterstitialTimerAdController>();
        
        _trackingTransparencyManager?.ShowATT(() =>
        {
            _customGdpr?.ShowGDPR(() =>
            {
                Initialize();
            });
        });
    }
    
    public void Initialize()
    {
            
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            OnInitializationComplete();
        });
            
    }

    private void OnInitializationComplete()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            OnAdsInitialized?.Invoke(true);
            _googleAdsAppOpenController?.LoadAd();
            _googleAdsBannerController?.LoadAd();
            _googleAdsInterstitialController?.LoadAd();
            _rewardedController?.LoadAd();
        });

    }

    public void ShowBanner()
    {
        _googleAdsBannerController.ShowAd(); 
    }
    
    public void HideBanner()
    {
        if (NoAdsPurchased)
        {
            return;
        }
        _googleAdsBannerController.HideAd(); 
    }

    public void ShowInterstitialAd(Action onComplete)
    {
        if (NoAdsPurchased)
        {
            onComplete?.Invoke();
            return;
        }
        _googleAdsInterstitialController?.ShowAd(onComplete);
    }

    public void ShowRewardedAd(Action<bool> rewardGiven)
    {
     
        _rewardedController?.ShowAd(rewardGiven);
    }
    
    public  void StartTimerAd()
    {
        if (NoAdsPurchased)
        {
            return;
        }
        _interstitialTimerAdController?.StartTimerAd();
    }
        
    public void StopTimerAd()
    {
        if (NoAdsPurchased)
        {
            return;
        }
        _interstitialTimerAdController?.StopTimerAd();

    }
    
    public void ResetTimerAd()
    {
        if (NoAdsPurchased)
        {
            return;
        }
        _interstitialTimerAdController?.StopTimerAd();
    }
    
    
    public void EnableAdIsComing(bool value)
    {
        //adIsComing?.SetActive(value);
    }
    
    public void NoAdsPurchasedSuccessfully()
    {
        NoAdsPurchased = true;
        HideBanner();
    }
    
}