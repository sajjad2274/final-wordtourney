using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;


public class RewardedAdController : MonoBehaviour
{
    //  public GameObject AdLoadedStatus;

#if UNITY_ANDROID
    private const string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        private const string _adUnitId = "unused";
#endif
    public static RewardedAdController instance;
    private RewardedAd _rewardedAd;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
        });
        LoadAd();
    }
    public void LoadAd()
    {
        if (_rewardedAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading rewarded ad.");
        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }

            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;
            RegisterEventHandlers(ad);
                // AdLoadedStatus?.SetActive(true);
            });
    }

    public void ShowRewardedAd()
    {
        
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            _rewardedAd.Show((Reward reward) =>
            {
                    // Get the active scene
                    Scene scene = SceneManager.GetActiveScene();

                    // Logic based on the current scene
                    if (scene.name == "Menu")
                {
                    if (MainMenuHandler.Instance != null && MainMenuHandler.Instance.isSpinWheelAd)
                    {
                        MainMenuHandler.Instance.isSpinWheelAd = false;
                        MainMenuHandler.Instance.SpinAnimatorWheel();
                    }
                }
                else if (scene.name == "Demo - Portrait")
                {
                    if (GamePlayHandler.Instance != null)
                    {
                        GamePlayHandler.Instance.RewardAds();
                    }
                }

                Debug.Log(string.Format("Rewarded ad granted a reward: {0} {1}",
                    reward.Amount,
                    reward.Type));
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
        }

        // AdLoadedStatus?.SetActive(false);
    }

    public void DestroyAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        //  AdLoadedStatus?.SetActive(false);
    }

    public void LogResponseInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
                // Reload the ad after it is closed
                LoadAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
        };
    }
}

