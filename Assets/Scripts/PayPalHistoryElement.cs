using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PayPalHistoryElement : MonoBehaviour
{
    public Text transactionId;
    public Text Cash;
    public Text status;
    private readonly string clientId = "AZ7rTGTgzGKjGJoTZkLF-qEmkAkYkmD0n4gShak8BSNM9XTCvu7KRQvDfSYvY3jQpIqYB-CoWDKJN2Q9";
    private readonly string secret = "ELTF8TQ2GwcQfSy2tmbd2hkRq808mDQLVFmHDseCOthudW9Bzf0YqwLPo1ZKemYWaIxVzsRgKH0t_DIK";
    private readonly string payoutsUrl = "https://api-m.paypal.com/v1/payments/payouts/"; // Use https://api.sandbox.paypal.com for sandbox

    public void NewScoreElement(string _transationId, string _cash,bool isDone)
    {
        transactionId.text = _transationId;
        Cash.text = _cash;
   
        if(isDone)
        {
            status.text = "Paid";
        }
        else
        {
            status.text = "Pending";
        }
    }
    //private void OnEnable()
    //{
    //    //StartCoroutine(GetPayPalAccessToken(transactionId.text));
    //}

    IEnumerator GetPayPalAccessToken(string batchId)
    {
        string endpoint = "https://api-m.paypal.com/v1/oauth2/token"; // Use https://api.sandbox.paypal.com for sandbox
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");

        UnityWebRequest www = UnityWebRequest.Post(endpoint, form);
        www.SetRequestHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + secret)));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseBody = www.downloadHandler.text;
            PayPalTokenResponse tokenResponse = JsonUtility.FromJson<PayPalTokenResponse>(responseBody);
          
            string accessToken = tokenResponse.access_token;

            if (!string.IsNullOrEmpty(accessToken))
            {
                UnityWebRequest payoutRequest = UnityWebRequest.Get(payoutsUrl + batchId);
                payoutRequest.SetRequestHeader("Content-Type", "application/json");
                payoutRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

                yield return payoutRequest.SendWebRequest();

                if (payoutRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Payout Status: " + payoutRequest.downloadHandler.text);
                    status.text = payoutRequest.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("Error Checking Payout Status: " + payoutRequest.error);
                }
            }
            else
            {
                Debug.LogError("Failed to get access token.");
            }
        }
        else
        {
            Debug.LogError("Token request failed: " + www.error);
            yield return null;
        }
    }

    [Serializable]
    private class PayPalTokenResponse
    {
        public string access_token;
        public string token_type;
        public string app_id;
        public int expires_in;
        public string nonce;
    }
}
