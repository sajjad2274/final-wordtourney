using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using DTT.WordConnect;

using UnityEngine.SceneManagement;

public class AdmobAds : MonoBehaviour
{

    public  static AdmobAds instance;
    public GameProgressData gameData;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad (instance);
        }
        else {
            Destroy (gameObject);
        }
    }
    BannerView _bannerView;

    [HideInInspector] public RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;

    [HideInInspector] public bool isRewarded, giveReward;

    // IMPORTANT : *** CHANGE THESE VARIABLE VALUE TO YOUR ADMOB ACCOUNT ADUNIT ID ***
    [SerializeField]string RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField]string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField]string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

   readonly private string admob_sample_id = "ca-app-pub-3940256099942544~3347511713";

    [HideInInspector] public bool rewardedAdLoaded;




    void Start()
    {


        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            
        });

      
        if(rewardedAd==null||!rewardedAd.CanShowAd())RequestRewardedAd();
        //CreateBannerView();
        //LoadBannerAd();
        if (interstitialAd == null || !interstitialAd.CanShowAd()) LoadInterstitialAd();
    }



    public void LoadInterstitialAd()
    {
      
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

   
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

       
        InterstitialAd.Load(InterstitialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
            
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }
                RegisterReloadHandler(ad);
                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
            });
    }
    public void ShowInterstitialAd()
    {
        if (gameData.noAds) return;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            
        }
    }
    private void RegisterReloadHandler(InterstitialAd ad)
    {
      
        ad.OnAdFullScreenContentClosed += ()=>
    {
            Debug.Log("Interstitial Ad full screen content closed.");

       
        interstitialAd.Destroy();
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
           
            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }

    public void CreateBannerView()
    {
        if (gameData.noAds) return;
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Top);
    }
    public void LoadBannerAd()
    {
        if (gameData.noAds) return;
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    // Function for Requesting Rewarded Ad
    private void RequestRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        RewardedAd.Load(RewardedAdUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }
                RegisterReloadHandler(ad);
                //RegisterEventHandlers(ad);
                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());
                RegisterEventHandlers(ad);
                rewardedAd = ad;
            });
    

    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            RequestRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);


            // Reload the ad so that we can show another as soon as possible.
            RequestRewardedAd();
        };
    }
    // Function for Showing Rewarded Ad
    public void ShowRewardedAd ()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Scene scene = SceneManager.GetActiveScene();
                if(scene.name== "Menu")
                {
                    if(MainMenuHandler.Instance.isSpinWheelAd)
                    {
                        MainMenuHandler.Instance.isSpinWheelAd = false;
                        MainMenuHandler.Instance.SpinAnimatorWheel();
                    }
                }
                else if (scene.name == "Demo - Portrait")
                {
                    GamePlayHandler.Instance.RewardAds();
                }
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                reward.Amount,
                    reward.Amount));
                // TODO: Reward the user.
                //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

            });
        }
    }
    public void ShowRewardedAdButterFly(int tileNo, HintOption hintOpt)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                WordConnectManager.Instance.RevealLetterHint(tileNo, hintOpt);
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                reward.Amount,
                    reward.Amount));
                // TODO: Reward the user.
                //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

            });
        }
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            //if (GamePlayHandler.Instance.spinWheelAd)
            //{
            //    GamePlayHandler.Instance.spinWheelDemo[].Spin();
            //}
            //GamePlayHandler.Instance.spinWheelAd = false;
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            isRewarded = false;
            giveReward = false;
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }


   



}  
