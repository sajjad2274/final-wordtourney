using DTT.WordConnect;
using DTT.WordConnect.Demo;
using Gley.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GamePlayHandler : MonoBehaviour
{
    public Color[] interactableColors;
    public static GamePlayHandler Instance;
    public GameProgressData progressData;
    public ResultsUI resultsUI;
    public GameObject timerObj;
    public GameObject levelObj;
    public GameObject freeModeObj;


    public Text[] cashTxt;
    public Text[] keysTxt;
    public Text[] gemsTxt;
    public Text LevelTxt;

    [Header("Powers")]
    public Text fireCrackerPowerTxt;
    public Text fireCrackerPowerTxt2;
    public Text hammerPowerTxt;
    public Text bulbPowerTxt;

    public Text fireCrackerPowerPriceTxt;
    public Text hammerPowerPriceTxt;
    public Text bulbPowerPriceTxt;

    public GameObject fireCrackerPowerPriceObj;
    public GameObject hammerPowerPriceObj;
    public GameObject bulbPowerPriceObj;

    public GameObject fireCrackerPowerCountObj;
    public GameObject hammerPowerCountObj;
    public GameObject bulbPowerCountObj;

    public int fireCrackerPowerPrice;
    public int hammerPowerPrice;
    public int bulbPowerPrice;

    public bool hammerPowerPriceUsed;
    public bool hammerUsed;

    public GameObject[] hammerCancelBtn;

    public GameObject rocketUsedPanel;


    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject soundOffBtn, soundOnBtn;
    [SerializeField] GameObject musicOffBtn, musicOnBtn;


    [Header("Toturial")]
    [SerializeField] GameObject toturialCanvas;

    [Header("Particles")]
    public GameObject[] particles;
    public GameObject particlesSpawnPos;
    public float particlesSpawnLife;
    [HideInInspector] public float particlesTime = 0;

    [Header("Butterfly")]
    public bool isButterFly;

    [Space]
    [Header("Sound Theme")]
    public AudioSource soundBgSimple;
    public AudioSource soundBgTournamentSection;
    public AudioSource soundBgGameplayFreeMode;
    public AudioSource soundBgGameplayTournamentMode;
    public AudioSource soundBtnOpen;
    public AudioSource soundBtnClickFreeMode;
    public AudioSource soundBtnClose;
    public AudioSource soundCashCollect;
    public AudioSource soundHammerUsed;
    public AudioSource soundLampUsed;
    public AudioSource soundNotHaveMoreHelpsAndGemsPrompt;
    public AudioSource soundNotHaveMoreHelpsAndGemsUse;
    public AudioSource soundRocketUsed;
    public AudioSource soundLevelStartFreeMode;
    public AudioSource soundWinFreeMode;
    public AudioSource soundNormalKeySoundNotForAnimation;
    public AudioSource soundMinutesBeforeTournamentStart;
    public AudioSource soundNotificationNormal;
    public AudioSource soundButterFlyClick;
    public AudioSource soundAvatar;
    public AudioSource soundClickWhenYouBuy;
    [Space]
    public AudioSource soundGemCollect;
    public AudioSource soundNotificationTournament;
    public AudioSource soundSpin;
    public AudioSource soundSpinOpened;
    public AudioSource soundTie;
    public AudioSource soundSecondPlace;
    public AudioSource soundThreePlace;
    public AudioSource soundWinnerTournament;
    public AudioSource soundAlertRestriction;
    public AudioSource soundEnterTournament;
    public AudioSource soundTournamentLast30Second;
    public AudioSource soundJoinTournament;
    public AudioSource soundToturialTournament;
    public AudioSource soundUnlock;
    public AudioSource soundAmazingLongWord;
    public AudioSource soundCorrectGoodWord;
    public AudioSource soundPerfectWordInterMedium;
    public AudioSource soundIncorrect;
    public AudioSource[] soundLetters;
    public AudioSource soundChangePositionCircle;
    public AudioSource soundFindNewWord;

    public int currentLetterSound;

    [Header("ads")]
    public bool doubleValueAd;
    public bool skipLevelAd;

    public GameObject doubleRewardAdBtn;

    [Header("Win Panel")]
    public Text cashTxtWinPanel;

    [Header("Review Panel")]
    public string urlReview;
    public GameObject rewiewPanel;

    [Header("Panels")]
    [SerializeField] float panelOpenCloseTime;
    Coroutine panelopenRoutine;
    Coroutine panelCloseRoutine;

    [Space]
    [Header("LeaderBoard")]

    public GameObject leaderBoardOpenerBtn;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    public GameObject noDataLeaderBoardObj;



    [Space]
    [Header("GamePlay")]
    public Button watchAdBtn;
    public Text watchAdBtnTxt;
    public int watchAdBtnTime;
    Coroutine watchAdBtnRoutine;

    [Space]
    [Header("LevelContinue")]
    public WordsFound wordsFound;

    [Space]
    [Header("Free Mode btns")]
    public GameObject[] freeModeBtns;


    [Space]
    [Header("Language Changer")]
    public TextWithLanguage[] allWords;

    [Space]
    [Header("Tournament Rewards")]
    public GameObject tournamentRewardPage;
    public Text tournamentRewardTxt;
    public Text tournamentRewardNameTxt;
    public Sprite[] tournamentRewardImages;
    public Sprite[] tournamentRewardImages2;
    public Image tournamentRewardImage;
    public Image tournamentRewardImage2;
    public List<TournamentRewards> tournamentRewardPrizes;

    [Space]
    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Text detailPanelTxt;
    public GameObject tournamentEndedpanel;

    private void Awake()
    {
        if (GameHandler.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        Instance = this;
        tournamentRewardPrizes = new List<TournamentRewards>();
        particlesTime = 0;
        if (GameHandler.Instance.isTournament)
        {
            timerObj.SetActive(true);
            freeModeObj.SetActive(false);
            leaderBoardOpenerBtn.SetActive(false);
            foreach (GameObject g in freeModeBtns)
            {
                //g.SetActive(false);
                g.GetComponent<Button>().interactable = false;
            }
            PlaySoundBgGameplayTournamentMode();
        }
        else
        {
            foreach (GameObject g in freeModeBtns)
            {
                g.SetActive(true);
            }
            timerObj.SetActive(false);
            freeModeObj.SetActive(true);
            leaderBoardOpenerBtn.SetActive(true);
            LevelTxt.text = (progressData.levelCompleted + 1).ToString();
            PlaySoundBgGameplayFreeMode();
            PlaySoundLevelStartFreeMode();
        }
        CheckPowers();
        FirebaseManager.onGetLeaderBoard += GetLeaderBoardValue;
        ChangeTextsLanguage();
        if(GameHandler.Instance.isTournament)
        {
            watchAdBtn.gameObject.SetActive(false);
        }
    }

    void TutorialChoose()
    {
        if (GameHandler.Instance.isTournament)
        {

        }
    }
    private void Start()
    {
        currentLetterSound = 0;
        if (GameHandler.Instance.sound == true)
        {
            SoundOn();
        }
        else
        {
            SoundOf();
        }

        if (GameHandler.Instance.music == true)
        {
            MusicOn();
        }
        else
        {
            MusicOf();
        }
        if (!GameHandler.Instance.isTournament)
        {
            if (PlayerPrefs.GetInt("toturial", 0) == 0)
            {
                toturialCanvas.transform.GetChild(0).gameObject.SetActive(false);
                toturialCanvas.transform.GetChild(3).gameObject.SetActive(true);
                Invoke("StartToturial", 1f);
            }
        }
        else
        {
            if (!PlayerPrefs.HasKey("TournamentTutorial"))
            {
                PlayerPrefs.SetInt("TournamentTutorial", 1);
                Invoke("StartToturial", 1f);
            }
        }
        ActivateSoundTheme(GameHandler.Instance.soundThemeUsed);
        foreach (Transform child in scoreboardContent.transform)
        {
            Destroy(child.gameObject);
        }

        LoadLeaderBoard();
        UpdateTxts();
    }
    public void ShowRewardTournament(string rType, string rewVal, string tName)
    {
        if (rType == "-1")
        {
            tournamentRewardImage.gameObject.SetActive(false);
            tournamentRewardTxt.text = "Ended";
            tournamentRewardNameTxt.text = tName;
            tournamentRewardPage.SetActive(true);
            Debug.Log("t reaward show -1");
            return;
        }

        switch (rType)
        {
            case "keys":
                tournamentRewardImage.sprite = tournamentRewardImages[0];
                tournamentRewardImage2.sprite = tournamentRewardImages2[0];
                break;
            case "gems":
                tournamentRewardImage.sprite = tournamentRewardImages[1];
                tournamentRewardImage2.sprite = tournamentRewardImages2[1];
                break;
            case "cash":
                tournamentRewardImage.sprite = tournamentRewardImages[2];
                tournamentRewardImage2.sprite = tournamentRewardImages2[2];
                break;
        }
        tournamentRewardImage2.SetNativeSize();
        tournamentRewardTxt.text = rewVal;
        tournamentRewardNameTxt.text = tName;
        tournamentRewardPage.SetActive(true);
        Debug.Log("t reaward show ");

    }
    public void ShowRewardTournament()
    {

        if (tournamentRewardPrizes.Count > 0)
        {
            switch (tournamentRewardPrizes[0].pType)
            {
                case "keys":
                    tournamentRewardImage.sprite = tournamentRewardImages[0];
                    break;
                case "gems":
                    tournamentRewardImage.sprite = tournamentRewardImages[1];
                    break;
                case "cash":
                    tournamentRewardImage.sprite = tournamentRewardImages[2];
                    break;
            }
            tournamentRewardTxt.text = tournamentRewardPrizes[0].pValue.ToString();
            tournamentRewardNameTxt.text = tournamentRewardPrizes[0].tournamentName;
            tournamentRewardPage.SetActive(true);
            tournamentRewardPrizes.RemoveAt(0);
        }
    }
    public void PlayLetterSound()
    {
        if (soundLetters.Length > 0)
        {
            soundLetters[currentLetterSound].Play();
            currentLetterSound++;
            if (currentLetterSound >= soundLetters.Length)
            {
                currentLetterSound = 0;
            }
        }

    }
    public void ShowRewardTournamentBtn()
    {

        if (tournamentRewardPrizes.Count > 0)
        {
            ShowRewardTournament();
        }
        else
        {
            GoHome();
        }
    }
    public bool CheckTournamentEnded()
    {
        return GameHandler.Instance.tournamentCurrentPlayerData.endDate.ToLocalTime() <= DateTime.Now;
    }
    public void ChangeTextsLanguage()
    {
        foreach (TextWithLanguage t in allWords)
        {
            t.txt.text = API.GetText(t.wordId);
        }
    }
    public void OpenReviewPanel()
    {
        OpenPanel(rewiewPanel);
    }
    public void OnOffHammerUsedBtn(bool val)
    {
        foreach (GameObject g in hammerCancelBtn)
        {
            g.SetActive(val);
        }
    }
    public void InitWordsFound()
    {
        wordsFound = new WordsFound();
        if (GameHandler.Instance.isTournament)
        {
            if (GameHandler.Instance.tournamentCurrentPlayerData.wordsFound != "" && GameHandler.Instance.tournamentCurrentPlayerData.wordsFound != string.Empty)
            {
                wordsFound = JsonUtility.FromJson<WordsFound>(GameHandler.Instance.tournamentCurrentPlayerData.wordsFound);
            }
        }
        else
        {
            if (progressData.wordsFound != "" && progressData.wordsFound != string.Empty)
            {
                wordsFound = JsonUtility.FromJson<WordsFound>(progressData.wordsFound);
            }
        }

    }
    public void AddWordFound(string w)
    {
        if (!wordsFound.Words.Contains(w))
        {
            wordsFound.Words.Add(w);
            if (GameHandler.Instance.isTournament)
            {
                GameHandler.Instance.tournamentCurrentPlayerData.wordsFound = JsonUtility.ToJson(wordsFound);
                FirebaseManager.Instance.UpdateTournamentProgress(0, 0);
            }
            else
            {
                progressData.wordsFound = JsonUtility.ToJson(wordsFound);
                FirebaseManager.Instance.SaveProgressData();
                //Debug.LogError("Json is: " + progressData.wordsFound);
            }

        }
    }
    public void ClearWordFound()
    {

        PlayerPrefs.DeleteKey("WordsFound");
        PlayerPrefs.Save();
    }
    public void StopWatchAdBtnTime()
    {
        if (watchAdBtnRoutine != null) StopCoroutine(watchAdBtnRoutine);
        watchAdBtn.interactable = false;
        watchAdBtnTxt.transform.parent.gameObject.SetActive(false);
        watchAdBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = interactableColors[0];
        watchAdBtn.transform.GetChild(1).gameObject.GetComponent<Image>().color = interactableColors[0];
        //watchAdBtn.gameObject.SetActive(false);
    }
    public void StartWatchAdBtnTime()
    {
        if (watchAdBtnRoutine != null) StopCoroutine(watchAdBtnRoutine);
        watchAdBtnRoutine = StartCoroutine(WatchAdBtnTimeRoutine());
    }
    IEnumerator WatchAdBtnTimeRoutine()
    {
        while (true)
        {

            watchAdBtn.gameObject.SetActive(true);
            watchAdBtn.interactable = true;
            watchAdBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = interactableColors[1];
            watchAdBtn.transform.GetChild(1).gameObject.GetComponent<Image>().color = interactableColors[1];

            watchAdBtnTxt.transform.parent.gameObject.SetActive(true);
            yield return null;
            int wt = watchAdBtnTime;
            while (wt > 0)
            {

                watchAdBtnTxt.text = wt.ToString();
                yield return new WaitForSeconds(1f);
                wt -= 1;

            }
            watchAdBtn.interactable = false;
            watchAdBtn.transform.GetChild(0).gameObject.GetComponent<Image>().color = interactableColors[0];
            watchAdBtn.transform.GetChild(1).gameObject.GetComponent<Image>().color = interactableColors[0];

            watchAdBtnTxt.transform.parent.gameObject.SetActive(false);
            //watchAdBtn.gameObject.SetActive(false);
            yield return new WaitForSeconds(15f);

        }


    }

    private void OnDestroy()
    {
        FirebaseManager.onGetLeaderBoard -= GetLeaderBoardValue;
    }
    public void LoadLeaderBoard()
    {
        //Destroy any existing scoreboard elements
        for (int i = scoreboardContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreboardContent.transform.GetChild(i).gameObject);
        }

        FirebaseManager.Instance.LoadLeaderBoard();

    }
    public void GetLeaderBoardValue(string username, int level, int pos, int cash)
    {
        //Instantiate new scoreboard elements
        if (noDataLeaderBoardObj.activeSelf) noDataLeaderBoardObj.SetActive(false);
        GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
        scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, level, pos, cash);
    }
    public void StartToturial()
    {
        toturialCanvas.SetActive(true);
    }
    public void ToturialFinished()
    {
        PlayerPrefs.SetInt("toturial", 1);
        PlayerPrefs.Save();
    }
    public void WatchSkipLevelAd()
    {
        skipLevelAd = true;
        AdmobAds.instance.ShowRewardedAd();

    }
    public void WatchDoubleRewardAd()
    {
        doubleValueAd = true;
        doubleRewardAdBtn.SetActive(false);
        AdmobAds.instance.ShowRewardedAd();

    }
    public void RewardAds()
    {
        if (skipLevelAd)
        {
            progressData.gems += 5;
            UpdateTxts();
            detailPanelTxt.text = $"GOT 5 GEMS AS REWARD";
            detailPanel.SetActive(true);
            //WordConnectManager.Instance.ForceNextlevel();
        }
        else if (doubleValueAd)
        {
            int csh = Int32.Parse(cashTxtWinPanel.text);
            progressData.gems += csh;
            csh *= 2;
            cashTxtWinPanel.text = csh.ToString();

        }
        skipLevelAd = false;
        doubleValueAd = false;
    }
    public void ActivateSoundTheme(int soundVal)
    {
        //  foreach (GameObject g in soundThemes) g.SetActive(false);
        //  soundThemes[soundVal].SetActive(true);
        GameHandler.Instance.soundThemeUsed = soundVal;
    }
    public void SpawnParticle(float sTime)
    {

        int num = -1;
        if (sTime - particlesTime < 15)
        {
            num = 0;
            soundCorrectGoodWord.Play();
        }
        else if (sTime - particlesTime < 30)
        {
            num = 1;
            soundPerfectWordInterMedium.Play();
        }
        else if (sTime - particlesTime < 45)
        {
            num = 2;
            soundAmazingLongWord.Play();
        }
        else if (sTime - particlesTime < 60)
        {
            num = 3;
            soundAmazingLongWord.Play();
        }
        particlesTime = sTime;
        if (num != -1) Destroy(Instantiate(particles[num], particlesSpawnPos.transform), particlesSpawnLife);
    }

    public void UpdateTxts()
    {
        foreach (Text txt in cashTxt)
        {
            txt.text = progressData.tickets.ToString();
        }
        foreach (Text txt in keysTxt)
        {
            txt.text = progressData.keys.ToString();
        }
        foreach (Text txt in gemsTxt)
        {
            txt.text = progressData.gems.ToString();
        }
    }

    public void UseHammer()
    {
        if (GameHandler.Instance.isTournament)
        {

            if (!hammerUsed)
            {
                if (GameHandler.Instance.tournamentCurrentPlayerData.hammers > 0)
                {
                    GameHandler.Instance.tournamentCurrentPlayerData.hammers--;

                    hammerUsed = true;
                    hammerPowerTxt.text = GameHandler.Instance.tournamentCurrentPlayerData.hammers.ToString();
                    OnOffHammerUsedBtn(true);
                    CheckPowers();
                    FirebaseManager.Instance.UpdateTournamentProgress(0, 0);
                    PlaySoundHammerUsed();
                }
                else
                {
                    PlaySoundNotHaveMoreHelpsAndGemsPrompt();
                }

            }
        }
        else
        {
            if (!hammerUsed)
            {
                if (progressData.hammer > 0)
                {
                    progressData.hammer--;
                    hammerUsed = true;
                    hammerPowerTxt.text = progressData.hammer.ToString();
                    OnOffHammerUsedBtn(true);
                    CheckPowers();
                    PlaySoundHammerUsed();
                }
                ///
                else if (progressData.gems >= hammerPowerPrice)
                {
                    progressData.gems -= hammerPowerPrice;
                    hammerPowerPriceUsed = true;
                    hammerUsed = true;
                    hammerPowerTxt.text = progressData.hammer.ToString();
                    OnOffHammerUsedBtn(true);
                    UpdateTxts();
                    //PlaySoundHammerUsed();
                    PlaySoundNotHaveMoreHelpsAndGemsUse();
                }
                else
                {
                    PlaySoundNotHaveMoreHelpsAndGemsPrompt();
                }
            }
        }
    }
    public void CheckPowers()
    {
        if (GameHandler.Instance.isTournament)
        {

            hammerPowerTxt.text = GameHandler.Instance.tournamentCurrentPlayerData.hammers.ToString();
            bulbPowerTxt.text = GameHandler.Instance.tournamentCurrentPlayerData.lamps.ToString();
        }
        else
        {
            hammerPowerTxt.text = progressData.hammer.ToString();
            bulbPowerTxt.text = progressData.bulb.ToString();
        }

        fireCrackerPowerTxt.text = progressData.fireCracker.ToString();
        fireCrackerPowerTxt2.text = progressData.fireCracker.ToString();

        fireCrackerPowerPriceTxt.text = fireCrackerPowerPrice.ToString();
        hammerPowerPriceTxt.text = hammerPowerPrice.ToString();
        bulbPowerPriceTxt.text = bulbPowerPrice.ToString();
        if (GameHandler.Instance.isTournament)
        {

            //bulbPowerPriceObj.SetActive(false);
            //hammerPowerPriceObj.SetActive(false);


        }
        else
        {
            if (progressData.bulb > 0)
            {
                bulbPowerPriceObj.SetActive(false);
                bulbPowerCountObj.SetActive(true);
            }
            else
            {
                bulbPowerPriceObj.SetActive(true);
                bulbPowerCountObj.SetActive(false);
            }
            if (progressData.hammer > 0)
            {
                hammerPowerPriceObj.SetActive(false);
                hammerPowerCountObj.SetActive(true);
            }
            else
            {
                hammerPowerPriceObj.SetActive(true);
                hammerPowerCountObj.SetActive(false);
            }
            if (progressData.fireCracker > 0)
            {
                fireCrackerPowerPriceObj.SetActive(false);
                fireCrackerPowerCountObj.SetActive(true);
            }
            else
            {
                fireCrackerPowerPriceObj.SetActive(true);
                fireCrackerPowerCountObj.SetActive(false);
            }
            bulbPowerTxt.text = progressData.bulb.ToString();
            hammerPowerTxt.text = progressData.hammer.ToString();
        }


    }
    public void CancelHammer()
    {
        if (GameHandler.Instance.isTournament)
        {
            if (hammerUsed)
            {

                GameHandler.Instance.tournamentCurrentPlayerData.hammers++;
                CheckPowers();


                hammerUsed = false;
                hammerPowerTxt.text = GameHandler.Instance.tournamentCurrentPlayerData.hammers.ToString();
                OnOffHammerUsedBtn(false);
                FirebaseManager.Instance.UpdateTournamentProgress(0, 0);

            }
        }
        else
        {
            if (hammerUsed)
            {
                if (hammerPowerPriceUsed)
                {
                    progressData.gems += hammerPowerPrice;
                    UpdateTxts();
                }
                else
                {
                    progressData.hammer++;
                    CheckPowers();
                }

                hammerUsed = false;
                hammerPowerTxt.text = progressData.hammer.ToString();
                OnOffHammerUsedBtn(false);
            }
        }

    }
    public void GotButterFly()
    {
        progressData.commonButterfly++;
        //int rand = UnityEngine.Random.Range(0, 3);
        //switch(rand)
        //{
        //    case 0:
        //        progressData.commonButterfly++;
        //        break;
        //    case 1:
        //        progressData.goldenButterfly++;
        //        break;
        //    case 2:
        //        progressData.legendaryButterfly++;
        //        break;
        //}
        FirebaseManager.Instance.SaveProgressData();
    }
    public void UseFireCracker()
    {
        //SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.name);
        if (!GameHandler.Instance.fireCrackerUsed)
        {
            if (progressData.fireCracker > 0)
            {
                progressData.fireCracker--;
                GameHandler.Instance.fireCrackerUsed = true;
                CheckPowers();
                fireCrackerPowerTxt.text = progressData.fireCracker.ToString();
                fireCrackerPowerTxt2.text = progressData.fireCracker.ToString();

                OpenPanel(rocketUsedPanel);
                FirebaseManager.Instance.SaveProgressData();
                PlaySoundRocketUsed();
            }
            else if (progressData.gems >= fireCrackerPowerPrice)
            {
                progressData.gems -= fireCrackerPowerPrice;
                GameHandler.Instance.fireCrackerUsed = true;
                GameHandler.Instance.fireCrackerPowerPriceUsed = true;
                fireCrackerPowerTxt.text = progressData.fireCracker.ToString();
                fireCrackerPowerTxt2.text = progressData.fireCracker.ToString();
                OpenPanel(rocketUsedPanel);
                UpdateTxts();
                FirebaseManager.Instance.SaveProgressData();
                PlaySoundRocketUsed();
                // suggestion needed
            }
            else
            {
                PlaySoundNotHaveMoreHelpsAndGemsPrompt();
            }
        }
    }
    public void UseBulb()
    {
        if (GameHandler.Instance.isTournament)
        {
            if (GameHandler.Instance.tournamentCurrentPlayerData.lamps > 0)
            {
                GameHandler.Instance.tournamentCurrentPlayerData.lamps--;
                WordConnectManager.Instance.RevealLetterHint();
                bulbPowerTxt.text = GameHandler.Instance.tournamentCurrentPlayerData.lamps.ToString();
                CheckPowers();
                FirebaseManager.Instance.SaveProgressData();
                PlaySoundLampUsed();

            }
            //else if (progressData.gems >= bulbPowerPrice)
            //{
            //    progressData.gems -= bulbPowerPrice;

            //    WordConnectManager.Instance.RevealLetterHint();
            //    UpdateTxts();
            //    FirebaseManager.Instance.SaveProgressData();
            //    //PlaySoundLampUsed();
            //    PlaySoundNotHaveMoreHelpsAndGemsUse();
            //}
            else
            {
                PlaySoundNotHaveMoreHelpsAndGemsPrompt();
            }
        }
        else
        {
            if (progressData.bulb > 0)
            {
                progressData.bulb--;
                WordConnectManager.Instance.RevealLetterHint();
                bulbPowerTxt.text = progressData.bulb.ToString();
                CheckPowers();
                FirebaseManager.Instance.SaveProgressData();
                PlaySoundLampUsed();
            }
            else if (progressData.gems >= bulbPowerPrice )
            {
                progressData.gems -= bulbPowerPrice;

                WordConnectManager.Instance.RevealLetterHint();
                UpdateTxts();
                FirebaseManager.Instance.SaveProgressData();
                //PlaySoundLampUsed();
                PlaySoundNotHaveMoreHelpsAndGemsUse();
            }
            else
            {
                PlaySoundNotHaveMoreHelpsAndGemsPrompt();
            }
        }
    }
    #region Sounds


    public void PlaySoundBgSimple()
    {
        soundBgSimple.Play();
        soundBgSimple.loop = true;
    }
    public void PlaySoundBgTournamentSection()
    {
        soundBgTournamentSection.Play();
        soundBgTournamentSection.loop = true;

    }
    public void PlaySoundBgGameplayFreeMode()
    {
        soundBgGameplayFreeMode.Play();
        soundBgGameplayFreeMode.loop = true;
    }
    public void PlaySoundBgGameplayTournamentMode()
    {
        soundBgGameplayTournamentMode.Play();
        soundBgGameplayTournamentMode.loop = true;
    }
    public void StopSoundBgSimple()
    {
        soundBgSimple.Stop();
        soundBgSimple.loop = true;
    }
    public void StopSoundBgTournamentSection()
    {
        soundBgTournamentSection.Stop();
        soundBgTournamentSection.loop = true;

    }
    public void StopSoundBgGameplayFreeMode()
    {
        soundBgGameplayFreeMode.Stop();
        soundBgGameplayFreeMode.loop = true;
    }
    public void StopSoundBgGameplayTournamentMode()
    {
        soundBgGameplayTournamentMode.Stop();
        soundBgGameplayTournamentMode.loop = true;
    }
    public void PlaySoundBtnOpen()
    {
        if (GameHandler.Instance.isTournament)
        {
            soundBtnClickFreeMode.Play();
        }
        else
        {
            soundBtnOpen.Play();
        }

    }
    public void PlaySoundBtnClickFreeMode()
    {
        soundBtnClickFreeMode.Play();
    }
    public void PlaySoundBtnClose()
    {
        soundBtnClose.Play();
    }
    public void PlaySoundCashCollect()
    {
        soundCashCollect.Play();
    }
    public void PlaySoundHammerUsed()
    {
        soundHammerUsed.Play();
    }
    public void PlaySoundLampUsed()
    {
        soundLampUsed.Play();
    }
    public void PlaySoundNotHaveMoreHelpsAndGemsPrompt()
    {
        soundNotHaveMoreHelpsAndGemsPrompt.Play();
    }
    public void PlaySoundNotHaveMoreHelpsAndGemsUse()
    {
        soundNotHaveMoreHelpsAndGemsUse.Play();
    }
    public void PlaySoundRocketUsed()
    {
        soundRocketUsed.Play();
    }
    public void PlaySoundLevelStartFreeMode()
    {
        soundLevelStartFreeMode.Play();
    }
    public void PlaySoundWinFreeMode()
    {
        soundWinFreeMode.Play();
    }
    public void PlaySoundNormalKeySoundNotForAnimation()
    {
        soundNormalKeySoundNotForAnimation.Play();
    }
    public void PlaySoundMinutesBeforeTournamentStart()
    {
        soundMinutesBeforeTournamentStart.Play();
    }
    public void PlaySoundNotificationNormal()
    {
        soundNotificationNormal.Play();
    }
    public void PlaySoundButterFlyClick()
    {
        soundButterFlyClick.Play();
    }
    public void PlaySsoundAvatar()
    {
        soundAvatar.Play();
    }
    public void PlaySoundClickWhenYouBuy()
    {
        soundClickWhenYouBuy.Play();
    }
    #endregion
    #region Settings
    public void SaveDataButton()
    {


        FirebaseManager.Instance.SaveProgressData();


    }
    public void SoundOn()
    {
        GameHandler.Instance.sound = true;
        audioMixer.SetFloat("SFX", -3);
        audioMixer.SetFloat("UI", -3);
        soundOffBtn.SetActive(false);
        soundOnBtn.SetActive(true);
    }
    public void SoundOf()
    {
        GameHandler.Instance.sound = false;
        audioMixer.SetFloat("SFX", -80);
        audioMixer.SetFloat("UI", -80);
        soundOnBtn.SetActive(false);
        soundOffBtn.SetActive(true);
    }
    public void MusicOn()
    {
        GameHandler.Instance.music = true;
        audioMixer.SetFloat("BGM", -3);
        musicOffBtn.SetActive(false);
        musicOnBtn.SetActive(true);
    }
    public void MusicOf()
    {
        GameHandler.Instance.music = false;
        audioMixer.SetFloat("BGM", -80);
        musicOnBtn.SetActive(false);
        musicOffBtn.SetActive(true);
    }

    #endregion
    public void OpenStore()
    {
        GameHandler.Instance.openStorePanel = true;
        GoHome();
    }
    public void OpenCash()
    {
        GameHandler.Instance.openCashPanel = true;
        GoHome();
    }
    public void OpenReviewUrl()
    {
        //Application.OpenURL(urlReview);
        PlayerPrefs.SetInt("ratedone", 1);
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
        rewiewPanel.SetActive(false);

    }
    public void NoAdsIap()
    {
        progressData.noAds = true;
        FirebaseManager.Instance.SaveProgressData();
    }
    public void RocketsPlusIap()
    {
        progressData.fireCracker += 10;
        FirebaseManager.Instance.SaveProgressData();
    }
    public void GoHome()
    {
        if (GameHandler.Instance.isTournament)
        {
            PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, (float)WordConnectManager.Instance.GameTimer.ElapsedTime);
            PlayerPrefs.SetString(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "LastExitTime", DateTime.Now.ToString());
            PlayerPrefs.Save();
            FirebaseManager.Instance.UpdateTournamentProgress(0, 0);
        }

        CancelHammer();
        SceneManager.LoadScene("Menu");
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (GameHandler.Instance.isTournament)
            {
                PlayerPrefs.SetFloat(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName, (float)WordConnectManager.Instance.GameTimer.ElapsedTime);
                PlayerPrefs.SetString(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName + "LastExitTime", DateTime.Now.ToString());
                PlayerPrefs.Save();
                FirebaseManager.Instance.UpdateTournamentProgress(0, 0);
            }
        }

    }
    public void OpenPanel(GameObject pn)
    {
        if (panelopenRoutine == null)
        {
            panelopenRoutine = StartCoroutine(OpenPanel(pn, true));
        }
    }
    public void ClosePanel(GameObject pn)
    {
        if (panelCloseRoutine == null)
        {
            panelCloseRoutine = StartCoroutine(ClosePanel(pn, false));
        }
    }
    IEnumerator OpenPanel(GameObject pn, bool val)
    {
        yield return new WaitForSeconds(panelOpenCloseTime);
        pn.SetActive(false);
        if (pn.GetComponent<Canvas>())
        {
            pn.GetComponent<Canvas>().enabled = val;
            if (!pn.activeSelf)
            {
                //Debug
                pn.SetActive(true);
            }
        }
        else
            pn.SetActive(val);
        panelopenRoutine = null;
    }
    IEnumerator ClosePanel(GameObject pn, bool val)
    {
        yield return new WaitForSeconds(panelOpenCloseTime);
        if (pn.GetComponent<Animator>())
        {

            pn.GetComponent<Animator>().SetTrigger("Close");
            yield return new WaitForSeconds(0.5f);
        }
        if (pn.GetComponent<Canvas>())
        {
            pn.GetComponent<Canvas>().enabled = val;
            pn.SetActive(false);
        }
        else
            pn.SetActive(val);
        panelCloseRoutine = null;
    }

    public void showAnimatior(Animator anim)
    {
        anim.enabled = true;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float delay = stateInfo.normalizedTime;
        Debug.LogError("Delay time: " + delay);
        StartCoroutine(disableAniamation(anim, 4));
    }

    IEnumerator disableAniamation(Animator anim, float delay)
    {

        yield return new WaitForSeconds(delay);
        anim.enabled = false;
        anim.Rebind();
        anim.transform.parent.gameObject.SetActive(false);

    }
}
[System.Serializable]
public class WordsFound
{
    public WordsFound()
    {
        Words = new List<string>();
    }
    public List<string> Words;
}