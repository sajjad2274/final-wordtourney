using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopGamePlay : MonoBehaviour
{
    public GameProgressData progressData;

    [Space]
    [Header("store")]
    public Image storeAvatarPic;
    public Button storefreeGemsBtn;
    DateTime storefreeGemsLastDateTime;
    public TextMeshProUGUI PurchaseSuccessDetail;
    public GameObject PurchaseSuccessPanel;

    [Space]
    [Header("Sounds")]
    public AudioSource soundClickWhenYouBuy;

    public TMP_Text keysTxt, gemsTxt, cashTxt;

    private void Start()
    {
        UpdateTxts();
    }
    public void Store_Packs(string pack)
    {
        switch (pack)
        {
            case "master":
                if (progressData.gems >= 10000)
                {
                    progressData.gems -= 10000;
                    progressData.fireCracker += 250;
                    progressData.hammer += 40;
                    progressData.bulb += 40;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 250, 40, 40);
                    PurchaseSuccessPanel.SetActive(true);
                  GamePlayHandler.Instance.  OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "encyclopedia":
                if (progressData.gems >= 15000)
                {
                    progressData.gems -= 15000;
                    progressData.fireCracker += 100;
                    progressData.hammer += 50;
                    progressData.bulb += 300;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 100, 50, 300);
                    PurchaseSuccessPanel.SetActive(true);
                    GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "workbook":
                if (progressData.gems >= 2500)
                {
                    progressData.gems -= 2500;
                    progressData.fireCracker += 20;
                    progressData.hammer += 10;
                    progressData.bulb += 5;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 20, 10, 5);
                    PurchaseSuccessPanel.SetActive(true);
                    GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "key1":
                if (progressData.gems >= 1000)
                {
                    progressData.gems -= 1000;
                    progressData.keys += 1;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 1);
                    PurchaseSuccessPanel.SetActive(true);
                    GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "key2":
                if (progressData.gems >= 8000)
                {
                    progressData.gems -= 1000;
                    progressData.keys += 10;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 10);
                    PurchaseSuccessPanel.SetActive(true);
                    GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "key3":
                if (progressData.gems >= 40000)
                {
                    progressData.gems -= 40000;
                    progressData.keys += 50;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 50);
                    PurchaseSuccessPanel.SetActive(true);
                    GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "noAd":
                progressData.noAds = true;
                PurchaseSuccessDetail.text = "Ads are off now";
                PurchaseSuccessPanel.SetActive(true);
                GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                break;
            case "wow":
                progressData.noAds = true;
                progressData.gems += 1000;
                progressData.fireCracker += 10;
                PurchaseSuccessDetail.text = string.Format("Gems: {0}\nRockets: {1}\n no ads forever", 1000, 10);
                PurchaseSuccessPanel.SetActive(true);
                GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
                break;
            default:
                break;
        }
        UpdateTxts();
        FirebaseManager.Instance.SaveProgressData();
    }

    public void GetHammer(int val)
    {
        progressData.hammer += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Hammers\n" + val;
        PurchaseSuccessPanel.SetActive(true);
        GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
    }
    public void GetBulb(int val)
    {
        progressData.bulb += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Bulbs\n" + val;
        PurchaseSuccessPanel.SetActive(true);
        GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
    }
    public void GetGems(int val)
    {
        progressData.gems += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        //UpdateTxts();
        PurchaseSuccessDetail.text = "Gems\n" + val;
        PurchaseSuccessPanel.SetActive(true);
        UpdateTxts();
        GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
    }
    public void GetfireCracker(int val)
    {
        progressData.fireCracker += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Rockets\n" + val;
        PurchaseSuccessPanel.SetActive(true);
        UpdateTxts();
        GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
    }
    public void GetFreeGems()
    {
        storefreeGemsLastDateTime = DateTime.Now;
        PlayerPrefs.SetString("storefreeGemsLastDateTime", storefreeGemsLastDateTime.ToString());
        PlayerPrefs.Save();
        storefreeGemsBtn.interactable = false;
        progressData.gems += 50;
        UpdateTxts();
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Free Gems\n 50";
        PurchaseSuccessPanel.SetActive(true);
        GamePlayHandler.Instance.OpenPanel(PurchaseSuccessPanel);
    }
    public void PlaySoundClickWhenYouBuy()
    {
        soundClickWhenYouBuy.Play();
    }

    private void UpdateTxts()
    {
        GamePlayHandler.Instance.UpdateTxts();
        gemsTxt.text = progressData.gems.ToString();
        cashTxt.text = progressData.tickets.ToString();
        keysTxt.text = progressData.keys.ToString();
    }
}
