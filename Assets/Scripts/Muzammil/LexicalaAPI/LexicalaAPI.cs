using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class LexicalaAPI : MonoBehaviour
{
    public Text wordDisplay;
    //private string url = "https://lexicala1.p.rapidapi.com/languages";
    //private string url = "https://lexicala1.p.rapidapi.com/languages";
    private string url = "https://lexicala1.p.rapidapi.com/test";

    private void Start()
    {
        Invoke(nameof(apiCall), 3f);
    }
    void apiCall()
    {
        StartCoroutine(GetWordsFromAPI());
    }

    IEnumerator GetWordsFromAPI()
    {
        //using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
        //{
        //    webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        //    yield return webRequest.SendWebRequest();

        //    if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        //    {
        //        Debug.LogError(webRequest.error);
        //    }
        //    else
        //    {
        //        //ProcessResponse(webRequest.downloadHandler.text);
        //        Debug.LogError(webRequest.downloadHandler.text);
        //    }
        //}
        Debug.LogError("Starting : ");
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        //webRequest.SetRequestHeader("x-rapidapi-key", "f5111943dcmsh35729676d78776ep104265jsn2daecb8ee56c");
        webRequest.SetRequestHeader("x-rapidapi-key", "b30c19f494msh0870780cb1c7f4dp133ed9jsna1d151871e1b");
        webRequest.SetRequestHeader("x-rapidapi-host", "lexicala1.p.rapidapi.com");

        //webRequest.SetRequestHeader("x-rapidapi-host", "lexicala1.p.rapidapi.com");
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("error : " + webRequest.error.ToString());

        }
        else
        {
            Debug.LogError(webRequest.downloadHandler.text);

        }
    }

    void ProcessResponse(string jsonResponse)
    {
        JObject json = JObject.Parse(jsonResponse);
        List<string> words = new List<string>();

        foreach (var word in json["words"])
        {
            words.Add(word["word"].ToString());
        }

        // Display the first word as an example
        if (words.Count > 0)
        {
            wordDisplay.text = words[0];
        }

        // Implement your game logic with the words list
    }
}
