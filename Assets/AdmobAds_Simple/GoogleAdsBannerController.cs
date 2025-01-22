using System;
using UnityEngine;


public class GoogleAdsBannerController : MonoBehaviour
{
        
    public GoogleMobileAds.Api.AdPosition position;
    private GoogleMobileAds.Api.BannerView _bannerView;
        
    [SerializeField] private string _adIdAndroid;
    [SerializeField] private string _adIdIOS;
    private string _adUnitId;

    private bool _bannerShown=false;
        
    public void LoadAd()
    {
        Debug.Log("Creating banner view." + _adUnitId);
        
#if UNITY_ANDROID
        _adUnitId = _adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
        
        if (GooglesAdsController.Instance.testIds)
        {
#if UNITY_ANDROID
        _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#endif
        }
        
        
        if (_bannerView != null)
        {
            DestroyAd();
        }
            
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _bannerView = new GoogleMobileAds.Api.BannerView(_adUnitId, GoogleMobileAds.Api.AdSize.Banner, position);
            ListenToAdEvents();
            Debug.Log("Banner view created." + _adUnitId);
            Debug.Log("GoogleAd---- banner---- Request--" + _adUnitId);
            var adRequest = new GoogleMobileAds.Api.AdRequest();
            Debug.Log("Loading banner ad." + _adUnitId);
            _bannerView?.LoadAd(adRequest);
        });
    }  
    
    
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            UnListenToAdEvents();
            Debug.Log("Destroying app banner.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
        
    public void ShowAd()
    {
        if (_bannerView != null)
        {
            if (!_bannerShown)
            {
                Debug.Log("Showing banner view.");
                _bannerShown = true;
                _bannerView.Show();
            }
        }
    }

    public void HideAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hiding banner view.");
            _bannerShown = false;
            _bannerView.Hide();
        }
    }
        
    public void ChangeBannerPosition(GoogleMobileAds.Api.AdPosition _pos)
    {
        _bannerView?.SetPosition(_pos);
    }
        

    private void ListenToAdEvents()
    {
        Debug.Log("ListenToAdEvents banner ad.");
        
        _bannerView.OnBannerAdLoaded += OnBannerAdLoadedCallBack;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
        _bannerView.OnAdPaid += OnAdPaid;
        _bannerView.OnAdImpressionRecorded += OnAdImpressionRecorded;
        _bannerView.OnAdClicked += OnAdClicked;
        _bannerView.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
        _bannerView.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
    }
                
    private void UnListenToAdEvents()
    {
        Debug.Log("Unlistened ToAdEvents banner ad.");
        
        _bannerView.OnBannerAdLoaded -= OnBannerAdLoadedCallBack;
        _bannerView.OnBannerAdLoadFailed -= OnBannerAdLoadFailed;
        _bannerView.OnAdPaid -= OnAdPaid;
        _bannerView.OnAdImpressionRecorded -= OnAdImpressionRecorded;
        _bannerView.OnAdClicked -= OnAdClicked;
        _bannerView.OnAdFullScreenContentOpened -= OnAdFullScreenContentOpened;
        _bannerView.OnAdFullScreenContentClosed -= OnAdFullScreenContentClosed;
    }
        
    private void OnBannerAdLoadedCallBack()
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Banner view loaded an ad with response : " + _bannerView.GetResponseInfo());
            _bannerShown = true;
            ShowAd();
        });
    }

    private void OnBannerAdLoadFailed(GoogleMobileAds.Api.LoadAdError error)
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _bannerShown = false;
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        });
    }

    private void OnAdPaid(GoogleMobileAds.Api.AdValue adValue)
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            
        });
    }

    private void OnAdImpressionRecorded()
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Banner view recorded an impression.");
        });
    }
        
    private void OnAdClicked()
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Banner view was clicked.");
        });
    }
        
    private void OnAdFullScreenContentOpened()
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Banner view full screen content opened.");
        });
    }
        
    private void OnAdFullScreenContentClosed()
    {
        GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Banner view full screen content closed.");
            _bannerShown = false;
        });
    }


}