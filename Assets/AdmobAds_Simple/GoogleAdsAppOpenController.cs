using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

using UnityEngine;


    public class GoogleAdsAppOpenController : MonoBehaviour
    {
        private GoogleMobileAds.Api.AppOpenAd _appOpenAd;


    private void Awake()
    {
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }


    [SerializeField] private string _adIdAndroid;
        [SerializeField] private string _adIdIOS;

        private string _adUnitId;

        public void LoadAd()
        {
            
            Debug.Log("Creating appOpen view." + _adUnitId);
        
#if UNITY_ANDROID
            _adUnitId = _adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
        
            if (GooglesAdsController.Instance.testIds)
            {
#if UNITY_ANDROID
                _adUnitId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
            _adUnitId = "ca-app-pub-3940256099942544/5575463023";
#endif
            }

            
            if (_appOpenAd != null)
            {
                DestroyAd();
            }
            
            Debug.Log($"AppOpen: Request");

            
            GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() => {
                var adRequest = new GoogleMobileAds.Api.AdRequest();
                GoogleMobileAds.Api.AppOpenAd.Load(_adUnitId, adRequest, (GoogleMobileAds.Api.AppOpenAd ad, GoogleMobileAds.Api.LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.Log($"AppOpen: Error/Null");
                        Invoke(nameof(LoadAd),2f);
                        return;
                    }
                    _appOpenAd = ad;

                    RegisterEventHandlers(ad);
                    Debug.Log($"AppOpen: Loaded");
                  
                });
            
            });


        }


        private void DestroyAd()
        {
            if (_appOpenAd != null)
            {
                Debug.Log($"AppOpen: Destroying");

                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

        }
        
        public void ShowAd()
        {
            if (GooglesAdsController.NoAdsPurchased)
            {
                return;
            }


        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            Debug.Log("Showing app open ad.");
            _appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }
    }
        
        
        private void RegisterEventHandlers(GoogleMobileAds.Api.AppOpenAd ad)
        {
            ad.OnAdPaid += (GoogleMobileAds.Api.AdValue adValue) =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Debug.Log($"AppOpen: Paid {adValue.Value} {adValue.CurrencyCode}");
                });
          
            };
            ad.OnAdImpressionRecorded += () =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Debug.Log($"AppOpen: Recorded An Impression");

                });
            };
            ad.OnAdClicked += () =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Debug.Log($"AppOpen: Clicked");

                });
            };
            ad.OnAdFullScreenContentOpened += () =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Debug.Log($"AppOpen: Opened");
                
                });
          
            };
            ad.OnAdFullScreenContentClosed += () =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    GooglesAdsController.Instance?.EnableAdIsComing(false);
                    Invoke(nameof(LoadAd),2f);
                });
            };
            ad.OnAdFullScreenContentFailed += (GoogleMobileAds.Api.AdError error) =>
            {
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    GooglesAdsController.Instance?.EnableAdIsComing(false);
                    Debug.LogError($"AppOpen:  Failed - ErrorDetail: {error}");

                    Time.timeScale = 1;
                    
                    Invoke(nameof(LoadAd),2f);
                });
            };
        }
        
        
               
        public bool IsAppOpenAvailable()
        {
            if (_appOpenAd != null)
            {
                return _appOpenAd.CanShowAd();

            }
            else
            {
                return false;
            }
        }
        
    
        private void OnDestroy()
        {
        
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
          
        }

    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            if (IsAppOpenAvailable())
            {
                ShowAd();
            }
        }
    }

}
    


