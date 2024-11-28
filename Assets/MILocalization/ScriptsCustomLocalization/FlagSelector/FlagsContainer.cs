using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Country Flags", menuName = "Scriptables/Country Flags", order = 1)]
public class FlagsContainer : ScriptableObject
{
    public FlagDetail[] flagsDetaila;
}
[Serializable]
public class FlagDetail
{
    public int LangEnumIndex;
    public Sprite flagImage;
    public string language;
    public string englishLanguage;
}