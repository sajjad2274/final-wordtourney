using UnityEngine;
using AppsFlyerSDK;
using System.Collections.Generic;
using System;

public class AppsFlyerManager : MonoBehaviour, IAppsFlyerConversionData, IAppsFlyerUserInvite
{
    // Your AppsFlyer App ID
    [SerializeField]private string appsFlyerDevKey = "8Yw2NJyuN3mEDrUhiyWSsE";

    

    // Define your reward parameters
    private int referralRewardCoins = 100;

    void Start()
    {
        AppsFlyer.setIsDebug(true);
        AppsFlyer.OnRequestResponse += appsflyeronrequestresponse;
        AppsFlyer.OnInAppResponse += (sender, args) =>
        {
            var af_args = args as AppsFlyerRequestEventArgs;
            AppsFlyer.AFLog("AppsFlyerOnRequestResponse", " status code " + af_args.statusCode);
            Debug.Log("AppsFlyerOnRequestResponse" + " status code " + af_args.statusCode);
        };
        AppsFlyer.initSDK(appsFlyerDevKey,"",this);
      AppsFlyer.setCustomerUserId(FirebaseManager.Instance.User.UserId);
        AppsFlyer.OnDeepLinkReceived += OnDeepLink;
        AppsFlyer.startSDK();
        AppsFlyer.setAppInviteOneLinkID("eT4y");
        generateappsflyerlink();


    }
    public void generateappsflyerlink()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("channel", "invite");
        parameters.Add("campaign", "invitecampaign");
        parameters.Add("referrername", FirebaseManager.Instance.User.UserId);


        // other params
        //parameters.add("referrername", "some_referrername");
        //parameters.add("referrerimageurl", "some_referrerimageurl");
        //parameters.add("customerid", "some_customerid");
        //parameters.add("basedeeplink", "some_basedeeplink");
        //parameters.add("branddomain", "some_branddomain");


        AppsFlyer.generateUserInviteLink(parameters, this);
    }
   
    public void onConversionDataSuccess(string conversiondata)
    {
        AppsFlyer.AFLog("didreceiveconversiondata", conversiondata);
        Debug.Log("didreceiveconversiondata" + conversiondata);
        Dictionary<string, object> data = AppsFlyer.CallbackStringToDictionary(conversiondata);
        if (data.ContainsKey("af_status") && (string)data["af_status"] == "Non-organic")
        {
            // This is a non-organic install (referral)
            // Extract relevant parameters from the conversion data
            string mediaSource = data.ContainsKey("referrername") ? (string)data["referrername"] : "";
           
            if(mediaSource!="")
            {
                FirebaseManager.Instance.RewardReferer(mediaSource);
            }

            
        }
     
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didreceiveconversiondatawitherror", error);
        Debug.Log("didreceiveconversiondatawitherror" + error);
    }

    public void onAppOpenAttribution(string attributiondata)
    {
        AppsFlyer.AFLog("onappopenattribution", attributiondata);
        Debug.Log("onappopenattribution" + attributiondata);
        Dictionary<string, object> attributiondatadictionary = AppsFlyer.CallbackStringToDictionary(attributiondata);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        Debug.Log("onappopenattributionfailure" + error);
    }
    public void onInviteLinkGenerated(string link)
    {
        Debug.Log("appsflyer link: " + link);
        GameHandler.Instance.refereralLink = link;
    }

    public void onInviteLinkGeneratedFailure(string error)
    {
        Debug.Log("appsflyer link fail: " + error);
    }

    public void onOpenStoreLinkGenerated(string link)
    {
        Debug.Log("onopenstorelinkgenerated: " + link);
    }



    void OnDeepLink(object sender, EventArgs args)
    {
        var deepLinkEventArgs = args as DeepLinkEventsArgs;

        switch (deepLinkEventArgs.status)
        {
            case DeepLinkStatus.FOUND:

                if (deepLinkEventArgs.isDeferred())
                {
                    AppsFlyer.AFLog("OnDeepLink", "This is a deferred deep link");
                    Debug.Log("OnDeepLink" + "This is a deferred deep link");
                }
                else
                {
                    AppsFlyer.AFLog("OnDeepLink", "This is a direct deep link");
                    Debug.Log("OnDeepLink" + "This is a direct deep link");
                }

                // deepLinkParamsDictionary contains all the deep link parameters as keys
                Dictionary<string, object> deepLinkParamsDictionary = null;
#if UNITY_IOS && !UNITY_EDITOR
                  if (deepLinkEventArgs.deepLink.ContainsKey("click_event") && deepLinkEventArgs.deepLink["click_event"] != null)
                  {
                      deepLinkParamsDictionary = deepLinkEventArgs.deepLink["click_event"] as Dictionary<string, object>;
                  }
#elif UNITY_ANDROID && !UNITY_EDITOR
                      deepLinkParamsDictionary = deepLinkEventArgs.deepLink;
#endif

                break;
            case DeepLinkStatus.NOT_FOUND:
                AppsFlyer.AFLog("OnDeepLink", "Deep link not found");
                Debug.Log("OnDeepLink" + "Deep link not found");
                break;
            default:
                AppsFlyer.AFLog("OnDeepLink", "Deep link error");
                Debug.Log("OnDeepLink" + "Deep link error");
                break;
        }
    }
    void appsflyeronrequestresponse(object sender, EventArgs e)
    {
        var args = e as AppsFlyerRequestEventArgs;
        AppsFlyer.AFLog("appsflyeronrequestresponse", " status code " + args.statusCode);
        Debug.Log("appsflyeronrequestresponse" + " status code " + args.statusCode);
    }
    //.....................
    //// Check for deep link on app launch
    //private void CheckForDeepLink()
    //{
    //    string deepLink ="";
    //    if (!string.IsNullOrEmpty(deepLink))
    //    {
    //        Debug.Log("Deep link received: " + deepLink);

    //        // Parse and handle the deep link parameters as needed
    //        HandleDeepLink(deepLink);
    //    }
    //}

    //// Handle referral information from the conversion data
    //private void HandleReferral(string mediaSource, string campaign, string referrerUid)
    //{
    //    // Implement your logic to reward the user for the referral
    //    // You can use mediaSource, campaign, and referrerUid parameters to identify the referral source
    //    Debug.Log("Referral from Media Source: " + mediaSource + ", Campaign: " + campaign + ", Referrer UID: " + referrerUid);

    //    // Check if the referrer UID is valid and not empty
    //    if (!string.IsNullOrEmpty(referrerUid))
    //    {
    //        // Reward the referrer user (previous user) with coins
    //        RewardReferrer(referrerUid);
    //    }
    //}

    //// Reward the referrer user with coins
    //private void RewardReferrer(string referrerUid)
    //{
    //    // Implement your reward logic here, such as updating the user's balance in your game
    //    // You may need a user management system to keep track of user balances and rewards
    //    Debug.Log("Rewarding referrer user with " + referralRewardCoins + " coins");

    //    // For simplicity, you can use PlayerPrefs to store the user's balance
    //    int currentBalance = PlayerPrefs.GetInt("UserBalance_" + referrerUid, 0);
    //    currentBalance += referralRewardCoins;
    //    PlayerPrefs.SetInt("UserBalance_" + referrerUid, currentBalance);

    //    // You can also trigger an event to update UI or notify the user about the reward
    //}

    //// Handle deep link parameters
    //private void HandleDeepLink(string deepLink)
    //{
    //    // Parse and handle the deep link parameters as needed
    //    // Example: Extract specific parameters from the deep link URL
    //    // You might want to use a URL parsing library or custom logic based on your deep link format
    //    Debug.Log("Handling deep link: " + deepLink);
    //}

    //void IAppsFlyerConversionData.onConversionDataSuccess(string conversionData)
    //{
    //    //Debug.Log("AppsFlyer conversion data: " + conversionData);

    //    //// Parse the conversion data as needed
    //    //Dictionary<string, object> data = AppsFlyer.CallbackStringToDictionary(conversionData);

    //    //if (data.ContainsKey("af_status") && (string)data["af_status"] == "Non-organic")
    //    //{
    //    //    // This is a non-organic install (referral)
    //    //    // Extract relevant parameters from the conversion data
    //    //    string mediaSource = data.ContainsKey("media_source") ? (string)data["media_source"] : "";
    //    //    string campaign = data.ContainsKey("campaign") ? (string)data["campaign"] : "";
    //    //    string referrerUid = data.ContainsKey("referrer_customer_id") ? (string)data["referrer_customer_id"] : "";

    //    //    // Handle the referral information (media source, campaign, referrer UID) as needed in your app
    //    //    HandleReferral(mediaSource, campaign, referrerUid);
    //    //}
    //}

    //void IAppsFlyerConversionData.onConversionDataFail(string error)
    //{
    //    Debug.LogError("AppsFlyer conversion data failed: " + error);
    //}

    //void IAppsFlyerConversionData.onAppOpenAttribution(string attributionData)
    //{
    //    Debug.Log("AppsFlyer App OPen: " + attributionData);
    //}

    //void IAppsFlyerConversionData.onAppOpenAttributionFailure(string error)
    //{
    //    Debug.Log("AppsFlyer App OPen fail: " + error);
    //}




}

