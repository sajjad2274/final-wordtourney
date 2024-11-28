using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAchievements : MonoBehaviour
{
    [SerializeField] GameProgressData progressData;
    public MedalAchievement[] achievements;
    public Transform achievementListContainer;
    public GameObject achievementPrefab;

    public Sprite musicClubbadgeSprite;

    void Start()
    {
        CheckForAchievements();
    }



    void CheckForAchievements()
    {
        int achNo = 1;
        if (PlayerPrefs.HasKey("musicClubBadge"))
        {
            GameObject newAchievement = Instantiate(achievementPrefab, achievementListContainer);
            newAchievement.transform.GetChild(0).transform.Find("MedalImage").GetComponent<Image>().sprite = musicClubbadgeSprite;
            newAchievement.transform.GetChild(0).transform.Find("AchievementName").GetComponent<TextMeshProUGUI>().text = "MUSIC CLUB";
            newAchievement.transform.GetChild(0).transform.Find("LevelNo").GetComponent<TextMeshProUGUI>().text = "WOW MUSIC CLUB";
            newAchievement.transform.GetChild(0).transform.Find("AchievementNo").GetComponent<TextMeshProUGUI>().text = achNo.ToString();
            achNo++;
        }
        foreach (var achievement in achievements)
        {
            if (progressData.level >= achievement.levelThreshold)
            {
                InstantiateAchievement(achievement, achNo);
            }
            achNo++;
        }

    }

    void InstantiateAchievement(MedalAchievement achievement, int achNo)
    {
        GameObject newAchievement = Instantiate(achievementPrefab, achievementListContainer);
        newAchievement.transform.GetChild(0).transform.Find("MedalImage").GetComponent<Image>().sprite = achievement.medalImage;
        newAchievement.transform.GetChild(0).transform.Find("AchievementName").GetComponent<TextMeshProUGUI>().text = achievement.name;
        newAchievement.transform.GetChild(0).transform.Find("LevelNo").GetComponent<TextMeshProUGUI>().text = "Level " + achievement.levelThreshold.ToString();
        newAchievement.transform.GetChild(0).transform.Find("AchievementNo").GetComponent<TextMeshProUGUI>().text = achNo.ToString();

    }
}
