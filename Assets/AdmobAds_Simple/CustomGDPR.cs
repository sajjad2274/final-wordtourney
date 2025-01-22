using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class CustomGDPR : MonoBehaviour
{
    [SerializeField]     private DebugGeography _debugGeography=DebugGeography.EEA;
    private ConsentForm _consentForm;
    private Action _onGDPRComplete;
    private Coroutine _coroutine;
    public void ShowGDPR(Action onGDPRComplete)
    {
        _onGDPRComplete = onGDPRComplete;
        _coroutine??=StartCoroutine(ShowGDPRDelay());
    }

    private IEnumerator ShowGDPRDelay()
    {
        yield return new WaitForSeconds(0.5f);
        var debugSettings = new ConsentDebugSettings
        {
            DebugGeography = _debugGeography,
            TestDeviceHashedIds = new List<string>
            {
                "965E4A26737DF85475A353251709C315"
            }
        };
        var request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error);
            _onGDPRComplete?.Invoke();
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            Debug.Log("IsConsentFormAvailable");

            LoadConsentForm();
        }
        else
        {
            Debug.Log("IsNotConsentFormAvailable");
            _onGDPRComplete?.Invoke();
        }
    }

    private void LoadConsentForm()
    {
        Debug.Log("LoadConsentFormStart");
        ConsentForm.Load(OnLoadConsentForm);
    }

    private void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        Debug.Log("OnLoadConsentForm");

        if (error != null)
        {
            Debug.LogError(error);
            _onGDPRComplete?.Invoke();
            return;
        }
        _consentForm = consentForm;
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            Debug.Log("Show");
            _consentForm.Show(OnShowForm);
        }
        else
        {
                
            Debug.Log("ShowElse");
            _onGDPRComplete?.Invoke();
        }
    }
        
    private void OnShowForm(FormError error)
    {
        Debug.Log("OnShowForm");

        if (error != null)
        {
            Debug.LogError(error);
            _onGDPRComplete?.Invoke();
            return;
        }
        LoadConsentForm();
            

    }

}