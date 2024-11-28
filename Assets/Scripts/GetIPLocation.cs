using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class GetIPLocation : MonoBehaviour
{
    private const string apiKey = "cdq6bh1rx7rv51id";
    private const string apiUrl = "https://api.ipregistry.co/";
    private void Start()
    {
        StartCoroutine(StartChatGptMethod());
    }



    IEnumerator StartChatGptMethod()
    {
        using (WWW www = new WWW("https://api.ipify.org"))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error getting public IP: " + www.error);
                GameHandler.Instance.countryName = "None";
            }
            else
            {
                // Print the public IP address to the console
              //  Debug.Log("Public IP Address: " + www.text);
                string ipAddress = www.text;

                string url = $"{apiUrl}{ipAddress}?key={apiKey}";

                using (WWW www2 = new WWW(url))
                {
                    yield return www2;

                    if (www2.error != null)
                    {
                        GameHandler.Instance.countryName = "None";
                       // Debug.LogError("Error: " + www2.error);
                    }
                    else
                    {
                        // Parse the JSON response to get the country name
                        string jsonResponse = www2.text;
                        var data =new GetIpResponse();
                        data = JsonUtility.FromJson<GetIpResponse>(jsonResponse);
                     //   Debug.Log("Your Country is " + data.location.country.name);
                        GameHandler.Instance.countryName = data.location.country.name;
                        GameHandler.Instance.countryLanguage = data.location.language.name;
                        GameHandler.Instance.progressData.userCountry = data.location.country.name;
                    }
                }
            }
        }
        
    }


}
#region Responce Classes
[System.Serializable]
public class Carrier
{
    public object name ;
    public object mcc ;
    public object mnc ;
}
[System.Serializable]
public class Company
{
    public string domain ;
    public string name ;
    public string type ;
}
[System.Serializable]
public class Connection
{
    public int asn ;
    public string domain ;
    public string organization ;
    public string route ;
    public string type ;
}
[System.Serializable]
public class Continent
{
    public string code ;
    public string name ;
}
[System.Serializable]
public class Country
{
    public int area ;
    public List<string> borders ;
    public string calling_code ;
    public string capital ;
    public string code ;
    public string name ;
    public int population ;
    public double population_density ;
    public Flag flag ;
    public List<Language> languages ;
    public string tld ;
}
[System.Serializable]
public class Currency
{
    public string code ;
    public string name ;
    public string name_native ;
    public string plural ;
    public string plural_native ;
    public string symbol ;
    public string symbol_native ;
    public Format format ;
}
[System.Serializable]
public class Flag
{
    public string emoji ;
    public string emoji_unicode ;
    public string emojitwo ;
    public string noto ;
    public string twemoji ;
    public string wikimedia ;
}
[System.Serializable]
public class Format
{
    public Negative negative ;
    public Positive positive ;
}
[System.Serializable]
public class Language
{
    public string code ;
    public string name ;
    public string native ;
}
[System.Serializable]
public class Language2
{
    public string code ;
    public string name ;
    public string native ;
}
[System.Serializable]
public class Location
{
    public Continent continent ;
    public Country country ;
    public Region region ;
    public string city ;
    public object postal ;
    public double latitude ;
    public double longitude ;
    public Language language ;
    public bool in_eu ;
}
[System.Serializable]
public class Negative
{
    public string prefix ;
    public string suffix ;
}
[System.Serializable]
public class Positive
{
    public string prefix ;
    public string suffix ;
}
[System.Serializable]
public class Region
{
    public string code ;
    public string name ;
}
[System.Serializable]
public class GetIpResponse
{
    public string ip ;
    public string type ;
    public object hostname ;
    public Carrier carrier ;
    public Company company ;
    public Connection connection ;
    public Currency currency ;
    public Location location ;
    public Security security ;
    public TimeZone time_zone ;
}
[System.Serializable]
public class Security
{
    public bool is_abuser ;
    public bool is_attacker ;
    public bool is_bogon ;
    public bool is_cloud_provider ;
    public bool is_proxy ;
    public bool is_relay ;
    public bool is_tor ;
    public bool is_tor_exit ;
    public bool is_vpn ;
    public bool is_anonymous ;
    public bool is_threat ;
}
[System.Serializable]
public class TimeZone
{
    public string id ;
    public string abbreviation ;
    public DateTime current_time ;
    public string name ;
    public int offset ;
    public bool in_daylight_saving ;
}
#endregion