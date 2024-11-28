using UnityEngine;


[CreateAssetMenu(fileName ="ProgressData",menuName ="Settings/ProgressData",order =1)]
public class GameProgressData : ScriptableObject
{
    public int levelCompleted;

  public int tickets;
    public int keys;
    public int gems;


    public bool noAds;

    //user level
    public int level;
    public int currentLevelPoints;
    public int requiredLevelPoints;

    //reward gift box
    public int currentRewardGiftPoints;
    public int requiredRewardGiftPoints;

  
    //butterflies
    public int commonButterfly;
    public int goldenButterfly;
    public int legendaryButterfly;

    //powerups
    public int hammer;
    public int bulb;
    public int fireCracker;

    //tournament
    public int tournamentUnlockLevel;

    //Player Profile Pic
    public string playerProfilePic;
    //public string name;

    public string userCountry;
    public string userEmail;
    public string userGender;

    public string spinWheelLastDate;
    public int spinWheelCount;
    public bool  freeSpinWheel;


    public bool invite1RewardGot;
    public bool invite2RewardGot;
    public bool invite3RewardGot;
    public int invites;
    public int paypalWithDrawAmount;


    public string wordsFound;

    public int wonTournament;
    public int failedTournament;
    public int playedTournament;
   

}
