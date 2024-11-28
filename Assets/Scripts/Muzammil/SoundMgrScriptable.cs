
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundMgrScriptable", order = 1)]
public class SoundMgrScriptable : ScriptableObject
{
    public GameObject AudioSourcePrefab;
    public AudioClip cashSound, gemsSound, keysSound, levelUpSound/*, giftOpenSound*/;
    public AudioClip firstPlaceSound, secondPlaceSound, thirdPlaceSound;

}