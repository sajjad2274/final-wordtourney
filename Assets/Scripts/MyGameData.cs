
using UnityEngine;
[CreateAssetMenu(fileName = "MyGameData", menuName = "Settings/My Game Data",order =0)]
public class MyGameData : ScriptableObject
{
    public Levels[] levels;
}
[System.Serializable]
public class WordContainer
{
    public string word;
}
[System.Serializable]
public class Levels
{
    public WordContainer[] words;
}