using System;
using UnityEngine;
using UnityEngine.Events;

public class GoogleAdsRewardedController : MonoBehaviour
{
    
    private GoogleMobileAds.Api.RewardedAd _rewardedAd;
        
    [SerializeField] private string _adIdAndroid;
    [SerializeField] private string _adIdIOS;

    private string _adUnitId;

    private bool _rewardGranted = false;


    private Action<bool> _rewardGiven;
   



    public void LoadAd()
    {
        Debug.Log(" GoogleAds----Rewarded--Request");

#if UNITY_ANDROID
        _adUnitId = _adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
        if (GooglesAdsController.Instance.testIds)
        {
#if UNITY_ANDROID
            _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
                _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#endif
        }
            

        if (_rewardedAd != null)
        {
            DestroyAd();
        }

        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            var adRequest = new GoogleMobileAds.Api.AdRequest();

            GoogleMobileAds.Api.RewardedAd.Load(_adUnitId, adRequest,
                (GoogleMobileAds.Api.RewardedAd ad, GoogleMobileAds.Api.LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Invoke(nameof(LoadAd),2f);
                        return;
                    }

                    _rewardedAd = ad;
                    RegisterEventHandlers(ad);
                    Debug.Log("-GoogleAds----Rewarded--loaded");

                });
        });

    }

    public void ShowAd(Action<bool> callBack)
    {
        _rewardGiven = callBack;
        _rewardGranted = false;
        if (IsRewardedAvailable())
        {
            _rewardedAd?.Show((GoogleMobileAds.Api.Reward reward) =>
            {
                _rewardGranted = true;
            });
        }else
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);
        }

    }

    private void DestroyAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
    }

    private void RegisterEventHandlers(GoogleMobileAds.Api.RewardedAd ad)
    {
        // var adNetworkAdapter = ad.MediationAdapterClassName();

        ad.OnAdPaid += (GoogleMobileAds.Api.AdValue adValue) =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            });
        };
        ad.OnAdImpressionRecorded += () =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            });
        };
        ad.OnAdClicked += () =>
        {
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log("Rewarded ad was clicked.");
            });

        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(true);

            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
               
            });

        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);
            GooglesAdsController.Instance.ResetTimerAd();

            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
                _rewardGiven?.Invoke(_rewardGranted);
                _rewardGranted = false;
                Invoke(nameof(LoadAd),2f);
            });

        };
        ad.OnAdFullScreenContentFailed += (GoogleMobileAds.Api.AdError error) =>
        {
            GooglesAdsController.Instance.EnableAdIsComing(false);

            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
                _rewardGiven?.Invoke(false);
                Invoke(nameof(LoadAd),2f);
            });

        };
    }

    public bool IsRewardedAvailable()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }
}