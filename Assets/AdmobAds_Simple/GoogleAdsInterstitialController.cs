using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Unity.Collections;

public class GoogleAdsInterstitialController : MonoBehaviour
{
    private InterstitialAd _interstitialAd;
        
    [SerializeField] private string _adIdAndroid;
    [SerializeField] private string _adIdIOS;
    
    private string _adUnitId;

    private Action _onComplete;
        
    public void LoadAd()
    {
        Debug.Log("GoogleAds----Inter--Request");

#if UNITY_ANDROID
        _adUnitId = _adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif

        if (GooglesAdsController.Instance.testIds)
        {
#if UNITY_ANDROID
            _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#endif
        }
        
        
  
        if (_interstitialAd != null)
        {
            DestroyAd();
        }
            
            
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {

            var adRequest = new AdRequest();
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Invoke(nameof(LoadAd),2f);
                    return;
                }
                
                Debug.Log("GoogleAds----Inter--Loaded");

                _interstitialAd = ad;
                RegisterEventHandlers(ad);
            });
        });

    }
        
        
    public void ShowAd(Action onComplete)
    {
        _onComplete = onComplete;
        if (IsInterstitialAvailable())
        {
            _interstitialAd?.Show();
        }
        else
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);
            _onComplete?.Invoke();
        }
    }

  
    private void DestroyAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
    }
        
    public bool IsInterstitialAvailable()
    {
        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }
        
    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {

                Debug.Log(String.Format("--------Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            });

        };
        
    
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {

                Debug.Log("------- Interstitial ad recorded an impression.");
            });

        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {

                Debug.Log("-------- Interstitial ad was clicked.");
            });

        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(true);
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {
                Debug.Log(" Interstitial ad full screen content opened.");
                  
            });

        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);

            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {
                Debug.Log("--------Interstitial ad full screen content closed.");
                _onComplete?.Invoke();
                Invoke(nameof(LoadAd),2f);
            });

        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);

            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.LogError("--------Interstitial ad failed to open full screen content with error : " + error);
                _onComplete?.Invoke();
                Invoke(nameof(LoadAd),2f);
            });
        };
    }
}