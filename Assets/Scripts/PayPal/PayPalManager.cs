using System;
using System.Collections;

using System.Text;

using UnityEngine;
using UnityEngine.Networking;

using static Paypal.Model.Order;
using static Paypal.Payout.Order;
public enum PayPalMehtod
{
    cashin,
    cashout
}
public class PayPalManager : MonoBehaviour
{
    [SerializeField] GameProgressData progressData;
    [SerializeField] MainMenuHandler mainMenuHandler;
    [SerializeField]TextAsset[] jsonStrings;
    [SerializeField] PayPalPaymentCreation paymentDetail;
    [SerializeField] PayOutRoot payoutDetail;
    [SerializeField] CreatePayoutResponse payoutResponse;
    #region Credentials
    [SerializeField]
    string _clientID = "Ad2gQettf7zTXTK4W1iBadI5MR0Sno3HyPwgxqlfksBU9N02kh2Y7_bGxBth_a8wkqlrhyVRqR_30hiL";
    [SerializeField]
    string _secret = "";

    #endregion

    #region URLS
    string _authTokenURL = "https://api.sandbox.paypal.com/v1/oauth2/token";
    string _paymentURL = "https://api.sandbox.paypal.com/v1/payments/payment";
    string _payoutURL = "https://api-m.sandbox.paypal.com/v1/payments/payouts";
    string _executeURL;
    #endregion

    public int currentPayment;
    public string currentReciever;
    void Start()
    {
        
    }

    #region Payment
    public void StartPayment()
    {
        StartCoroutine(GetAccessTokenPayment());
    }
    IEnumerator GetAccessTokenPayment()
    {

        WWWForm form = new WWWForm();
        string base64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_clientID + ":" + _secret));
        form.AddField("grant_type", "client_credentials");
        using (var request = UnityWebRequest.Post(_authTokenURL, form))
        {
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Accept-Language", "en_US");
            request.SetRequestHeader("Authorization", "Basic " + base64);
            yield return request.SendWebRequest();

            if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {

                var data = JsonUtility.FromJson<PayPalAuthToken>(request.downloadHandler.text);
                Debug.Log(data.access_token);
                Debug.Log(data.token_type);
                Debug.Log("Request Sent!");
                StartCoroutine(GetPaymentID(data.access_token));
            }

        }
    }
    IEnumerator GetPaymentID(string accessToken)
    {
        PayPalPaymentCreation Payment = new PayPalPaymentCreation();
        //if (jsonStrings != null)
        //{
        //    Payment = JsonUtility.FromJson<PayPalPaymentCreation>(jsonStrings[0].ToString()); //Order can easily be setup ui buttons etc or gameplay elements.
        //    Payment.transactions[0].invoice_number += System.Guid.NewGuid().ToString();
        //    //Payment.transactions[0].amount.total = currentPayment.ToString();
        //    //Payment.transactions[0].amount.details.subtotal = currentPayment.ToString();
        //    //Payment.transactions[0].amount.details.tax = "0.00";
        //    //Payment.transactions[0].amount.details.shipping = "0.00";
        //    //Payment.transactions[0].amount.details.handling_fee = "0.00";
        //    //Payment.transactions[0].amount.details.insurance = "0.00";
        //    //Payment.transactions[0].amount.details.shipping_discount = "0.00";
        //    paymentDetail = Payment;
        //}
        Payment = paymentDetail;
        Payment.transactions[0].invoice_number += System.Guid.NewGuid().ToString();
        Payment.transactions[0].amount.total = currentPayment.ToString();
        Payment.transactions[0].amount.details.subtotal = currentPayment.ToString();
        Payment.transactions[0].amount.details.tax = "0.00";
        Payment.transactions[0].amount.details.shipping = "0.00";
        Payment.transactions[0].amount.details.handling_fee = "0.00";
        Payment.transactions[0].amount.details.insurance = "0.00";
        Payment.transactions[0].amount.details.shipping_discount = "0.00";
        Payment.transactions[0].item_list.items[0].price = currentPayment.ToString();
        Payment.transactions[0].item_list.items[0].tax = "0.00";

        var request = new UnityWebRequest(_paymentURL, "POST");
        string PaymentString = (JsonUtility.ToJson(Payment));
        byte[] bodyRaw = Encoding.UTF8.GetBytes(PaymentString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        yield return request.SendWebRequest();
        if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.downloadHandler.text);
            Debug.LogError(request.error);
        }
        else
        {
            var data = JsonUtility.FromJson<PayPalPaymentCreation>(request.downloadHandler.text);
            Payment = data;
            Debug.Log("request sent");
            Application.OpenURL(Payment.links[1].href);
            StartCoroutine(GetPayerID(Payment.links[0].href,accessToken));
        }
    }
    IEnumerator GetPayerID(string url,string accessToken)
    {
        bool orderApproved = false;
        while (orderApproved == false)
        {
            yield return new WaitForSeconds(5);
            using (var request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);
                yield return request.SendWebRequest();
                if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
                {
                    Debug.Log(request.downloadHandler.text);
                    Debug.LogError(request.error);

                }
                else
                {
                    var data = JsonUtility.FromJson<Paypal.Model.Verify>(request.downloadHandler.text);
                    if (data.payer.status == "VERIFIED")
                    {
                        Debug.Log("ORDER APPROVED STATUS VERIFIED");
                        orderApproved = true;
                        Debug.Log(data.links[0].href);
                        Debug.Log(data.links[1].href);
                        Debug.Log(data.links[2].href);
                        Debug.Log(data.payer.payer_info.payer_id);
                        _executeURL = data.links[1].href;
                        StartCoroutine(ExecutePayment(data, accessToken));
                    }
                }
            }
        }
    }
    IEnumerator ExecutePayment(Paypal.Model.Verify payer_ID,string accessToken)
    {
        string payerID = JsonUtility.ToJson(payer_ID.payer.payer_info);
        byte[] rawbody = Encoding.UTF8.GetBytes(payerID);
        using (var request = new UnityWebRequest(_executeURL, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(rawbody);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            yield return request.SendWebRequest();
            if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
            {
                mainMenuHandler.payPalAmountInputFieldResultDetail.text = request.downloadHandler.text;
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {

                Debug.Log("Request sent successfully");
                Debug.Log(request.downloadHandler.text);
                var data = JsonUtility.FromJson<Paypal.Model.ExecuteResponse>(request.downloadHandler.text);
               
                Debug.Log("Order Completed:" + data.state);
                if(mainMenuHandler.payPalMehtod==PayPalMehtod.cashin)
                {
                    progressData.tickets += (int)currentPayment;
                    mainMenuHandler.payPalAmountInputFieldResultDetail.text = "Order Completed:" + data.state;
                }
              else if (mainMenuHandler.payPalMehtod == PayPalMehtod.cashout)
                {
                    progressData.tickets -= (int)currentPayment;
                    mainMenuHandler.payPalAmountInputFieldResultDetail.text = "Order Completed:" + data.state;
                }
                mainMenuHandler.UpdateTxts();
                FirebaseManager.Instance.SaveProgressData();
            }

        }
    }
    #endregion
    #region Payout
    public void StartPayout()
    {
        //StartCoroutine(GetAccessTokenPayout());
        FirebaseManager.Instance.SendPayPalRequest(currentReciever, currentPayment.ToString(), currentPayment);
    }
    IEnumerator GetAccessTokenPayout()
    {

        WWWForm form = new WWWForm();
        string base64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_clientID + ":" + _secret));
        form.AddField("grant_type", "client_credentials");
        using (var request = UnityWebRequest.Post(_authTokenURL, form))
        {
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Accept-Language", "en_US");
            request.SetRequestHeader("Authorization", "Basic " + base64);
            yield return request.SendWebRequest();

            if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.downloadHandler.text);
                mainMenuHandler.payPalAmountInputFieldResultDetail.text = ("error");
                mainMenuHandler.payoutInProgress = false;
            }
            else
            {

                var data = JsonUtility.FromJson<PayPalAuthToken>(request.downloadHandler.text);
                Debug.Log(data.access_token);
                Debug.Log(data.token_type);
                Debug.Log("Request Sent!");
                StartCoroutine(GetPayoutID(data.access_token));
            }

        }
    }
    IEnumerator GetPayoutID(string accessToken)
    {
        //https://api.ipregistry.co/223.123.5.3?key=cdq6bh1rx7rv51id
        PayOutRoot payOutRoot = payoutDetail;
        payOutRoot.items[0].receiver = currentReciever;
        payOutRoot.items[0].amount.value = currentPayment.ToString();
        payOutRoot.sender_batch_header.sender_batch_id+= GameHandler.Instance.pID;
        payOutRoot.sender_batch_header.sender_batch_id+=  PlayerPrefs.GetInt("BatchId",0).ToString();
        PlayerPrefs.SetInt("BatchId", PlayerPrefs.GetInt("BatchId", 0) + 1);
        PlayerPrefs.Save();


        var request = new UnityWebRequest(_payoutURL, "POST");
        string PaymentString = (JsonUtility.ToJson(payOutRoot));
        byte[] bodyRaw = Encoding.UTF8.GetBytes(PaymentString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        yield return request.SendWebRequest();
        if (request.isNetworkError || !string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.downloadHandler.text);
            Debug.LogError(request.error);
            mainMenuHandler.payPalAmountInputFieldResultDetail.text = ("error");
            mainMenuHandler.payoutInProgress = false;
        }
        else
        {
            CreatePayoutResponse data = JsonUtility.FromJson<CreatePayoutResponse>(request.downloadHandler.text);
            payoutResponse = data;
            Debug.Log(request.downloadHandler.text);
            if (data!=null)mainMenuHandler.payPalAmountInputFieldResultDetail.text=("transaction done!\n waiting for approval\n"+data.batch_header.batch_status);
            progressData.tickets -= (int)currentPayment;
            FirebaseManager.Instance.SaveProgressData();
            FirebaseManager.Instance.UpdatePayPalData(payOutRoot.sender_batch_header.sender_batch_id, currentPayment.ToString());
            mainMenuHandler.GetPayPalHistory(payOutRoot.sender_batch_header.sender_batch_id, currentPayment.ToString(),false);
            //Application.OpenURL(data.Links[0].Href);
            // StartCoroutine(GetPayerID(data.Links[0].Href, accessToken));
            mainMenuHandler.payoutInProgress = false;
            mainMenuHandler.UpdateTxts();
        }
    }
    #endregion

    #region Paypal With FireBase

 
    #endregion
}
