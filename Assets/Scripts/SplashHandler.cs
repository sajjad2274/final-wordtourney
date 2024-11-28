using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
//using TMPro;

public class SplashHandler : MonoBehaviour
{
    [SerializeField] GameObject loginSceneName;
 
    [SerializeField] Image loadingImage;
    [SerializeField] Text warningLoginText;

    AsyncOperation sceneLoader;

    bool sceneLoadStarted=false;

    private void Awake()
    {
        AttachFirebaseEvents();
      
    }

    private void Start()
    {
        loginSceneName.SetActive(true);

        StartCoroutine(LoadLoading());
        loadingImage.DOFillAmount(0.2f, 1.5f);
    }
    IEnumerator LoadLoading()
    {
        //yield return null;
        //SceneManager.LoadScene("Menu");
        //yield break;
        warningLoginText.text = "Checking Country";
        yield return new WaitForSeconds(1.5f);
        loadingImage.DOFillAmount(0.4f, 0.5f);
        yield return new WaitUntil(predicate: () => GameHandler.Instance.countryName!=string.Empty);
        string Countrii = GameHandler.Instance.countryName;
        string CountriiLang = GameHandler.Instance.countryLanguage;
        if(Countrii!= "None")
        {
            foreach (LanguageCountries l in GameHandler.Instance.languageCountries)
            {
                if(l.languageCountries.Contains(Countrii)||CountriiLang.ToLower()==l.languageName)
                {
                    GameHandler.Instance.languageSelected=l.languageNumber;
                    break;
                }
            }
        }
      
        warningLoginText.text = "Connecting to Server";
        Task task = FirebaseManager.Instance.LoadData();

        yield return new WaitUntil(predicate: () => task.IsCompleted);
        
        loadingImage.DOFillAmount(0.7f, 2f);
        warningLoginText.text = "Initializing Data";
        FirebaseManager.Instance.CheckAutoLogin();
        
        yield return new WaitUntil(predicate: () => sceneLoadStarted==true);
        warningLoginText.text = "Initializing Levels";
        Task task2 = FirebaseManager.Instance.LoadLevelsStart();

        yield return new WaitUntil(predicate: () => task2.IsCompleted);
        warningLoginText.text = "Loading...";
    
        loadingImage.DOFillAmount(1f, 6f);
        yield return new WaitForSeconds(6f);
       sceneLoader.allowSceneActivation = true;
    }
    private void OnDestroy()
    {
        DeAttachFirebaseEvents();
    }



    public void AttachFirebaseEvents()
    {
        FirebaseManager.onSuccessLogin += LoginSuccess;
        FirebaseManager.onSuccessAnonymous += LoginSuccess;
        FirebaseManager.onFailedLogin += LoginFailed;
        FirebaseManager.onFailedAnonymous += LoginFailed;



    }
    public void DeAttachFirebaseEvents()
    {
        FirebaseManager.onSuccessLogin -= LoginSuccess;
        FirebaseManager.onSuccessAnonymous -= LoginSuccess;
        FirebaseManager.onFailedLogin -= LoginFailed;
        FirebaseManager.onFailedAnonymous -= LoginFailed;
    }

   
    public void LoginSuccess(string msg)
    {

        warningLoginText.text = msg;

        sceneLoader = SceneManager.LoadSceneAsync("Menu");
        sceneLoader.allowSceneActivation = false;

        sceneLoadStarted = true;
      
    }
    public void LoginFailed(string msg)
    {
        warningLoginText.text = msg;

    }
}
