using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class AnagramGenerator : MonoBehaviour
{
    private string apiUrl = "https://www.stands4.com/services/v2/ana.php";
    private string uid = "12715";          // Replace with your actual UID
    private string tokenid = "QpSxvvJXhzvwi0oC";  // Replace with your actual token ID
    private string term = "letter";        // Replace with the word you want to find anagrams for
    private string format = "xml";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetAnagrams());
    }

    IEnumerator GetAnagrams()
    {
        // Create the URL with query parameters
        string url = $"{apiUrl}?uid={uid}&tokenid={tokenid}&term={term}&format={format}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Get the XML response
                string xmlResponse = www.downloadHandler.text;

                // Print the raw XML to debug
                Debug.Log("Raw XML Response: " + xmlResponse);

                // Parse the XML and convert to JSON
                var jsonData = ConvertXmlToJson(xmlResponse);

                // Save the JSON data to a file
                SaveJsonToFile(jsonData, term);
            }
        }
    }

    string ConvertXmlToJson(string xmlData)
    {
        XDocument xmlDoc = XDocument.Parse(xmlData);

        // Extract words from XML
        var words = xmlDoc.Descendants("Anagram")
                           .Select(x => x.Value)
                           .ToArray();

        // Convert to LevelData format (for demo purposes, all words are put in level 1)
        var levelData = new LevelDataList
        {
            levels = new LevelData[]
            {
                new LevelData
                {
                    level = 1,
                    words = words
                }
            }
        };

        // Convert LevelDataList to JSON
        return JsonUtility.ToJson(levelData, true);
    }

    void SaveJsonToFile(string jsonData, string term)
    {
        // Define the file path
        string filePath = Path.Combine(Application.persistentDataPath, $"{term}_anagrams.json");

        // Save the JSON string to the file
        File.WriteAllText(filePath, jsonData);

        Debug.Log($"JSON saved successfully at: {filePath}");
    }

    [System.Serializable]
    public class LevelData
    {
        public int level;
        public string[] words;
    }

    [System.Serializable]
    public class LevelDataList
    {
        public LevelData[] levels;
    }
}
