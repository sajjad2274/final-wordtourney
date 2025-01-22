using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using UnityEngine.UI;
using System;
//using TMPro;
using UnityEngine.Audio;
using static NativeShare;
using Unity.VisualScripting;
using UnityEngine.Video;
using DG.Tweening;
using Gley.Localization;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using System.Text.RegularExpressions;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;





public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler Instance;


    public GameProgressData progressData;

    public VideoPlayer vp;
    [SerializeField] string videoFileName;
    public GameObject LoadingBg;
    [Space]
    [Header("Texts")]
    public Text[] cashTxt;
    public Text[] keysTxt;
    public Text[] levelTxt;
    public Text[] gemsTxt;


    public Text commonButterflyTxt;
    public Text goldenButterflyTxt;
    public Text legendaryButterflyTxt;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject soundOffBtn, soundOnBtn;
    [SerializeField] GameObject musicOffBtn, musicOnBtn;

    [Space]
    [Header("LeaderBoard")]
    public GameObject scoreElement;
    public Transform scoreboardContent;
  public GameObject noDataLeaderBoardObj;

    [Space]
    [Header("LeaderBoard Highest")]
    public GameObject scoreHighestElement;
    public Transform scoreboardHighestContent;
    public GameObject noDataHighestLeaderBoardObj;

    [Space]
    [Header("UserData")]
    public InputField usernameField;
    public Text uId;
    public Text playerLevelProgress;
    public Image levelFillBar;
    public Image playerProfilePic;
    public Text tournamentWonTxt;
    public Text tournamentLostTxt;
    public Text tournamentPlayedTxt;
    public InputField emailInp;
    public InputField countryInp;
    public List<string> genders;
    public Dropdown genderDropDown;


    [Space]
    [Header("Language Changer")]
    public SupportedLanguages currentLanguage;
    public List<SupportedLanguages> allLanguages;
    public Dropdown lcDropown;
    public TextWithLanguage[] allWords;

    [Space]
    [Header("Spin Wheel")]
    [SerializeField] private Button[] uiSpinButton;
    //Animator Wheel
    [SerializeField] string[] animatorWheelTriggers;
    [SerializeField] int animatorWheelResultNo;
    [SerializeField] int[] animatorWheelChances;
    [SerializeField] Animator animatorWheel;
    [SerializeField] int animatorWheelFees;
    [SerializeField] bool isSpinning;
    [SerializeField] SpinReward[] spinRewards;
    [SerializeField] ParticleSystem spinWheelParticles;
    DateTime spinWheelLastDateTime;

    [SerializeField] GameObject spinBtnAdObj;
    [SerializeField] GameObject spinRewardPanel;
    [SerializeField] Text spinRewardObtainedTxt;
    [SerializeField] Text[] spinWheelCountTxt;
    [SerializeField] Text[] spinWheelRewardDetailTxt;



    [Space]
    [Header("Sound Theme")]
    //public GameObject[] soundThemes;
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

    [Space]
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject tournamentPanel;
    public GameObject spinPanel;
    public GameObject mainLoginPanel;
    public GameObject leaderBoardPanel;
    public GameObject tournamentListPanel;
    public GameObject storePanel;
    public GameObject payPalPanel;
    [SerializeField] float panelOpenCloseTime;
    Coroutine panelopenRoutine;
    Coroutine panelCloseRoutine;

    [Space]
    [Header("Login")]
    public GameObject mainLoginBtn;
    public GameObject mainLoginSelectionUI;
    public Text warningLogInText;

    [Space]
    [Header("Tournaments")]
    public GameObject[] tournamentLock;
    public Text[] tournamentBtnsTxt;
    public GameObject tournamentsLockedPanel;
    public GameObject tournamentSignInPanel;
    public GameObject lowPlayers;
    public GameObject countryNotAllowedPanel;

    [SerializeField] GameObject tournamentPrefab;
    List<ListenerRegistration> ld;
    public string pID;
    public Dictionary<string, TournamentDetailContainer> tournamentDetailContainers;
    bool tournamentDataLoaded = false;

    [Space]
    [Header("Tournament Rewards")]
    [SerializeField] private GameObject tournamentRewardPage;
    public GameObject[] TournamentPlace;
    [SerializeField] private Text tournamentRewardKeyplacetext;
    [SerializeField] private Text tournamentRewardTxt;
    [SerializeField] private Text tournamentRewardNameTxt;
    [SerializeField] private Sprite[] tournamentRewardImages;
    [SerializeField] private Image tournamentRewardImage;
    [SerializeField] private Sprite[] tournamentRewardImages2;
    [SerializeField] private Image tournamentRewardImage2;
    public List<TournamentRewards> tournamentRewardPrizes;
    int tournamentCounted = 0;
    int tournamentCreated = 0;
    [SerializeField] private GameObject tournamentRewardKeysPage;
    [SerializeField] private GameObject tournamentRewardGemsPage;

    [SerializeField] private Text tournamentRewardGemsTxt;
    [SerializeField] private Text tournamentRewardNameGemsTxt;

    [SerializeField] private Text tournamentRewardKeysTxt;
    [SerializeField] private Text tournamentRewardNameKeysTxt;


    [Space]
    [Header("Tournament Types")]
    [SerializeField] Sprite tournamentTypeUnlockedSprite;
    public List<TournamentTypes> tournamentTypes;
    [SerializeField] GameObject tournamentTypesPanel;

    [Space]
    [Header("Tournament History")]
    public Transform trnmntHstryCntnr;
    public Transform inProgressTrnmntHstryCntnr;
    public Transform completedTrnmntHstryCntnr;
    public GameObject TournamentHistoryPrefab;

    [Space]
    [Header("Toturial")]
    [SerializeField] GameObject toturialCanvas;

    [Space]
    [Header("ad")]
    public bool isSpinWheelAd;

    [Space]
    [Header("store")]
    public Image storeAvatarPic;
    public Button storefreeGemsBtn;
    DateTime storefreeGemsLastDateTime;
    public Text PurchaseSuccessDetail;
    public GameObject PurchaseSuccessPanel;

    [Space]
    [Header("paypal")]
    public PayPalManager payPalManager;
    public InputField payPalAmountInputField;
    public InputField payPalEmailInputField;
    public Text payPalAmountInputFieldResultDetail;
    public PayPalMehtod payPalMehtod;
    public GameObject payPalPaymentPanel;
    public GameObject paypalHistoryElement;
    public Transform paypalHistoryContent;
    [HideInInspector] public bool payoutInProgress;

    [Space]
    [Header("Main Panel")]
    public Text[] stageTxt;

    [Space]
    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Text detailPanelTxt;


    [Space]
    [Header("Invite")]
    public Button[] inviteClaimBtns;
    public Button sendInviteBtns;
    public int[] InviteReward;
    public Text inviteCount;

    [Space]
    [Header("Tournament Waiting")]
    public TournamentWaitingPanel tournamentWaitingPanel;

    [Space]
    [Header("Floating animations objects")]
    public Sprite diamondImg;
    public Sprite keyImg;
    public Sprite cashImg;
    public GameObject imgPrefab;
    public GameObject initialPos;
    public GameObject targetPositionGem;
    public GameObject targetPositionKey;
    public GameObject targetPositionCash;

    public static bool isFromTournament;
    private void Awake()
    {
        if (GameHandler.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        Instance = this;
        // VideoPlayerCompleted(vp);
        tournamentRewardPrizes = new List<TournamentRewards>();
        payoutInProgress = false;
        //if (vp.isPrepared && vp.isPlaying)
        //{
        //    LoadingBg.GetComponent<Image>().DOFade(0, 2f);
        //    Debug.Log("PlayVid");
        //}
        //else
        //{
        //    vp.prepareCompleted += VideoPlayerCompleted;
        //}
        //if (vp)
        //{
        //    //string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        //    //vp.url = videoPath;
        //    vp.clip = Resources.Load<VideoClip>(videoFileName);
        //    vp.isLooping = true;
        //    vp.Play();

        //}
        if (!GameHandler.Instance.isLoggedIn)
        {
            AttachFirebaseEvents();
            mainLoginBtn.SetActive(true);
        }

        FirebaseManager.onGetPaypalHistory += GetPayPalHistory;
        FirebaseManager.onGetTournamentHistory += OnGetTournamentHistory;
        if (PlayerPrefs.GetInt("SignedIn", 0) == 1)
        {

            mainLoginBtn.SetActive(false);

        }
        inviteCount.text = progressData.invites.ToString();
        tournamentDetailContainers = new Dictionary<string, TournamentDetailContainer>();
        ld = new List<ListenerRegistration>();
        PlaySoundBgSimple();
        SetGameQuality();
    }
    void SetGameQuality()
    {
        int ram = SystemInfo.systemMemorySize;
        if (ram < 3500)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = 90;
        if (ram < 3500) // Less than 4GB of RAM
        {
            QualitySettings.SetQualityLevel(0, true); // Set to Low
            Debug.Log("Setting quality to Low.");
        }
        else if (ram < 5000) // 4GB to 8GB of RAM
        {
            QualitySettings.SetQualityLevel(2, true); // Set to Medium
            Debug.Log("Setting quality to Medium.");
        }
        else // More than 8GB of RAM
        {
            QualitySettings.SetQualityLevel(4, true); // Set to High
            Debug.Log("Setting quality to High.");
        }
    }
    private void Start()
    {
        if (!mainPanel.activeInHierarchy)
            mainPanel.SetActive(true);
        StartCoroutine(CheckLeaderBoardTournament());
        GameHandler.Instance.isTournament = false;
        pID = GameHandler.Instance.pID;
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
        StartFireStore();
        updateUsernameField();

        UpdateTxts();
        ActivateSoundTheme(GameHandler.Instance.soundThemeUsed);

        //if (PlayerPrefs.GetInt("toturialMenu", 0) == 0)
        //{
        //    toturialCanvas.SetActive(true);
        //}
        //else
        //{
        //    CheckSpinWheelTime();
        //}

        StartCoroutine(opentutorial());

        if (DateTime.TryParse(PlayerPrefs.GetString("storefreeGemsLastDateTime", "0"), out storefreeGemsLastDateTime))
        {
            if (DateTime.Now.Subtract(storefreeGemsLastDateTime).TotalHours > 24d)
            {
                storefreeGemsBtn.interactable = true;

            }
            else
            {
                storefreeGemsBtn.interactable = false;

            }
        }
        else
        {

            storefreeGemsBtn.interactable = true;

        }
        if (FirebaseManager.Instance.avatarPicTexture != null)
        {
            storeAvatarPic.sprite = FirebaseManager.Instance.avatarPicTexture;
        }

        FirebaseManager.Instance.LoadPayPalHistory();

        CheckInvites();
        // System by default  
        //currentLanguage = GameHandler.Instance.currentLanguage;
        //lcDropown.value = allLanguages.IndexOf(currentLanguage);

        ChangeTextsLanguage();
        UpdateTournamentStats();
        SetTournamentLocks();
        LoadTournamentTypes();

        countryInp.text = progressData.userCountry;
        genderDropDown.value = genders.IndexOf(progressData.userGender);
        emailInp.text = progressData.userEmail;
        FirebaseManager.Instance.SaveProgressData();
        if (GameHandler.Instance.openStorePanel)
        {
            GameHandler.Instance.openStorePanel = false;
            ClosePanel(mainPanel);
            OpenPanel(storePanel);
        }
        else if (GameHandler.Instance.openCashPanel)
        {
            GameHandler.Instance.openCashPanel = false;
            ClosePanel(mainPanel);
            OpenPanel(payPalPanel);
        }
        Invoke(nameof(RewardMethodCall), 3);
        if (isFromTournament)
        {
            isFromTournament = false;
            OpenTournamentPanel(mainPanel);
        }
        if (PlayerPrefs.GetString("spinwheeldate") == GetDate())
            uiSpinButton[3].gameObject.SetActive(true);
        else
            uiSpinButton[3].gameObject.SetActive(false);

        //  StartCoroutine(LoadHighestLeaderBoardAfterCashGet());
        // StartCoroutine(boolTrue());
        //FirebaseManager.Instance.DeleteDocument("Beginner");
        LoadHighestLeaderBoard();
    }
    //bool isDataGot;
    //IEnumerator LoadHighestLeaderBoardAfterCashGet()
    //{
    //    yield return new WaitUntil(() => isDataGot);
    //    LoadHighestLeaderBoard();
    //}
    //IEnumerator boolTrue()
    //{
    //    yield return new WaitForSeconds(5);
    //    isDataGot = true;
    //}
    IEnumerator opentutorial()
    {
        yield return new WaitForSeconds(1.5f);
        if (PlayerPrefs.GetInt("toturialMenu", 0) == 0)
        {
            toturialCanvas.SetActive(true);
        }
        else
        {
            CheckSpinWheelTime();
        }
    }
    void RewardMethodCall()
    {
        if (!PlayerPrefs.HasKey("endTimeTournament") || !PlayerPrefs.HasKey("tournament_cash"))
            return;
        string endDatee = PlayerPrefs.GetString("endTimeTournament");
        DateTime endTim = DateTime.Parse(endDatee);
        DateTime endTim2 = endTim.AddHours(25);
        //DateTime endTim2 = endTim.AddMinutes(1);

        if (endTim2 < DateTime.Now)
            GiveRewardWhenNoRecord();
    }
    #region Tournament Types
    public void LoadTournamentTypes()
    {
        foreach (TournamentTypes t in tournamentTypes)
        {
            if (t.tournamentUnlockLevel <= progressData.level)
            {
                t.tournamentLock.SetActive(false);
                t.tournamentImg.sprite = tournamentTypeUnlockedSprite;
            }
        }
    }
    public void OpenTournamentTypePanel(int tNo)
    {
        if (!tournamentTypes[tNo].tournamentLock.activeSelf)
        {
            ClosePanel(tournamentTypesPanel);
            OpenPanel(tournamentTypes[tNo].ContainerPanel);
        }
        else
        {
            detailPanelTxt.text = $"Will Unlock At Level {tournamentTypes[tNo].tournamentUnlockLevel}";
            OpenPanel(detailPanel);
        }
    }
    #endregion
    public void OnChangeEmail(string val)
    {
        progressData.userEmail = val;
    }
    public void OnChangeGender(int g)
    {
        progressData.userGender = genders[g];
    }

    public void SetTournamentLocks()
    {
        if (progressData.level >= progressData.tournamentUnlockLevel)
        {
            foreach (GameObject t in tournamentLock)
            {
                t.SetActive(false);
            }
            foreach (Text t in tournamentBtnsTxt)
            {
                t.text = "tournament";
            }
        }
        else
        {
            foreach (GameObject t in tournamentLock)
            {
                t.SetActive(true);
            }
            foreach (Text t in tournamentBtnsTxt)
            {
                t.text = "reach level " + progressData.tournamentUnlockLevel + " to unlock tournaments";
            }
        }
    }
    public void UpdateTournamentStats()
    {
        tournamentWonTxt.text = progressData.wonTournament.ToString();
        int lostStat = progressData.failedTournament;
        tournamentLostTxt.text = lostStat.ToString();
        tournamentPlayedTxt.text = progressData.playedTournament.ToString();
    }
    public void VideoPlayerCompleted(VideoPlayer r)
    {
        StartCoroutine(CheckVp(r));

    }
    IEnumerator CheckVp(VideoPlayer r)
    {
        //yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return null;
            if (r.isPlaying && r.isPrepared && r.frame > 1)
            {
                LoadingBg.GetComponent<Image>().DOFade(0, 2f);
                break;
            }
        }
    }
    IEnumerator CheckLeaderBoardTournament()
    {

        while (true)
        {
            yield return null;
            if (tournamentDataLoaded)
            {
                // LoadLeaderBoard(0);
                break;
            }
        }
    }
    public void ShowDetail(string d)
    {
        detailPanelTxt.text = d;
        OpenPanel(detailPanel);
    }
    public void CheckInvites()
    {
        inviteClaimBtns[0].interactable = !progressData.invite1RewardGot && progressData.invites >= 1;
        inviteClaimBtns[1].interactable = !progressData.invite2RewardGot && progressData.invites >= 3;
        inviteClaimBtns[2].interactable = !progressData.invite3RewardGot && progressData.invites >= 5;

    }
    public void CheckSpinWheelTime()
    {
        if (DateTime.TryParse(progressData.spinWheelLastDate, out spinWheelLastDateTime))
        {
            if (DateTime.Now.Subtract(spinWheelLastDateTime).TotalHours > 24d)
            {
                spinWheelLastDateTime = DateTime.Now;
                progressData.spinWheelLastDate = DateTime.Now.ToString();
                progressData.spinWheelCount += 1;

                if (!GameHandler.Instance.spinWheelOpened)
                {
                    OpenPanel(spinPanel);
                    ClosePanel(mainPanel);
                    GameHandler.Instance.spinWheelOpened = true;
                }
                progressData.freeSpinWheel = true;
            }

        }
        else
        {
            if (!GameHandler.Instance.spinWheelOpened)
            {
                OpenPanel(spinPanel);
                ClosePanel(mainPanel);
                GameHandler.Instance.spinWheelOpened = true;
            }

            spinWheelLastDateTime = DateTime.Now;
            progressData.spinWheelLastDate = DateTime.Now.ToString();
            progressData.spinWheelCount += 1;
            progressData.freeSpinWheel = true;
        }
        if (progressData.freeSpinWheel)
        {

            spinBtnAdObj.SetActive(false);
        }
        else
        {

            if (progressData.spinWheelCount <= 0)
            {

                spinBtnAdObj.SetActive(false);
            }
            else
            {

                spinBtnAdObj.SetActive(true);
            }
        }
        foreach (var v in spinWheelCountTxt) v.text = progressData.spinWheelCount.ToString();

    }
    public void ToturialFinished()
    {
        PlayerPrefs.SetInt("toturialMenu", 1);
        PlayerPrefs.Save();
        CheckSpinWheelTime();
        ShowRewardTournament();
    }
    public void ActivateSoundTheme(int soundVal)
    {
        // foreach (GameObject g in soundThemes) g.SetActive(false);
        // soundThemes[soundVal].SetActive(true);
        GameHandler.Instance.soundThemeUsed = soundVal;
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
        soundBtnOpen.Play();
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
    public void Spin()
    {

        for (int i = 0; i < uiSpinButton.Length; i++)
        {
            uiSpinButton[i].interactable = false;
        }



        //pickerWheel.OnSpinEnd(wheelPiece =>
        //{


        //    switch (wheelPiece.Label)
        //    {
        //        case "cash":
        //            progressData.gems += (int)wheelPiece.Amount;
        //            break;
        //        case "keys":
        //            progressData.keys += (int)wheelPiece.Amount;
        //            break;

        //        default:
        //            break;
        //    }
        //    UpdateTxts();
        //    FirebaseManager.Instance.SaveProgressData();

        //    Debug.Log(
        //       @" <b>Index:</b> " + wheelPiece.Index + "           <b>Label:</b> " + wheelPiece.Label
        //      + "\n <b>Amount:</b> " + wheelPiece.Amount + "      <b>Chance:</b> " + wheelPiece.Chance + "%"
        //   );

        //    for (int i = 0; i < uiSpinButton.Length; i++)
        //    {
        //        uiSpinButton[i].interactable = true;
        //    }

        //});

        //pickerWheel.Spin();
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
        foreach (Text txt in levelTxt)
        {
            txt.text = progressData.level.ToString();
        }
        foreach (Text txt in gemsTxt)
        {
            txt.text = progressData.gems.ToString();
        }
        foreach (Text txt in stageTxt)
        {
            txt.text = (progressData.levelCompleted + 1).ToString();
        }
        commonButterflyTxt.text = progressData.commonButterfly.ToString();
        goldenButterflyTxt.text = progressData.goldenButterfly.ToString();
        legendaryButterflyTxt.text = progressData.legendaryButterfly.ToString();
        playerLevelProgress.text = progressData.currentLevelPoints + "/" + progressData.requiredLevelPoints;
        levelFillBar.fillAmount = (float)progressData.currentLevelPoints / (float)progressData.requiredLevelPoints;
    }
    //public int tCashss, tGemss, tKeyss;
    public void LoadLeaderBoard(string tName)
    {
        //Destroy any existing scoreboard elements
        for (int i = scoreboardContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreboardContent.transform.GetChild(i).gameObject);
        }

        if (tournamentDetailContainers.ContainsKey(tName))
        {
            if (tournamentDetailContainers[tName].leaderBoards.Count > 0)
            {
                noDataLeaderBoardObj.SetActive(false);
            }
            else
            {
                noDataLeaderBoardObj.SetActive(true);
            }
            foreach (TLeaderBoard tt in tournamentDetailContainers[tName].leaderBoards)
            {
            
                GetLeaderBoardValue(tt.username, tt.level, tt.iRange, tt.timeTaken,tt.cash);
            }
        }
        else
        {
            noDataLeaderBoardObj.SetActive(true);
        }


    }
    public void LoadNewLeaderBoard(string tName)
    {
        //Destroy any existing scoreboard elements
        for (int i = scoreboardContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreboardContent.transform.GetChild(i).gameObject);
        }

        if (tournamentDetailContainers.ContainsKey(tName))
        {
            if (tournamentDetailContainers[tName].leaderBoards.Count > 0)
            {
                noDataLeaderBoardObj.SetActive(false);
            }
            else
            {
                noDataLeaderBoardObj.SetActive(true);
            }
            foreach (TLeaderBoard tt in tournamentDetailContainers[tName].leaderBoards)
            {
             
                GetLeaderBoardValue(tt.username, tt.level, tt.iRange, tt.timeTaken, tt.cash);
            }
        }
        else
        {
            noDataLeaderBoardObj.SetActive(true);
        }
        // ClosePanel(tournamentListPanel);
        OpenPanel(leaderBoardPanel);

    }
    public void GetLeaderBoardValue(string username, int level, int pos, int cash)
    {
        //Instantiate new scoreboard elements

        if (noDataLeaderBoardObj.activeSelf) noDataLeaderBoardObj.SetActive(false);
        GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
        scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, level, pos, cash);


    }
    public void GetLeaderBoardValue(string username, int level, int pos, float timeTaken, int cash)
    {
        //Instantiate new scoreboard elements
        if (noDataLeaderBoardObj.activeSelf) noDataLeaderBoardObj.SetActive(false);
        GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
        //scoreboardElement.GetComponent<ScoreElement>().PlayDoTweenAnimationWithDelay(pos * 0.5f);

        DOTweenAnimation tween = scoreboardElement.transform.GetChild(0).gameObject.GetComponent<DOTweenAnimation>();

        float totalDelay = 0.5f + (0.5f * pos);  // Calculate delay for each item
        Debug.LogError("Delay: " + totalDelay);
        tween.delay = totalDelay;  // Set delay

        scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, level, pos, timeTaken, cash);
        scoreboardElement.SetActive(true);
        tween.DOPlay();
    }
    public void updateUsernameField()
    {
        usernameField.text = GameHandler.Instance.username;
        uId.text = "ID:   " + GameHandler.Instance.pID;
        if (progressData.playerProfilePic != String.Empty)
        {
            playerProfilePic.sprite = FirebaseManager.Instance.ConvertTextureStringToSprite(progressData.playerProfilePic);
        }
    }
    public void SaveDataButton(GameObject thisPanel = null)
    {


        if (PlayerPrefs.GetInt("SignedIn", 0) == 0 && thisPanel != null)
        {
            ClosePanel(thisPanel);
            OpenPanel(mainLoginPanel);
        }
        if (!string.IsNullOrEmpty(emailInp.text))
        {
            bool isValid = IsValidEmail(emailInp.text);
            if (isValid)
            {
                progressData.userEmail = emailInp.text;
            }
        }
        FirebaseManager.Instance.UpdateUserName(usernameField.text);

        FirebaseManager.Instance.SaveProgressData();

    }
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        // Regular expression for validating email addresses
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        return Regex.IsMatch(email, emailPattern);
    }

    public void SaveNameOnFireBase()
    {
        if (string.IsNullOrEmpty(usernameField.text))
            return;
        FirebaseManager.Instance.UpdateUserName(usernameField.text);
        if (!string.IsNullOrEmpty(usernameField.text))
            GameHandler.Instance.username = usernameField.text;
        FirebaseManager.Instance.SaveProgressData();
    }
    private void OnDestroy()
    {
        FirebaseManager.onGetHighestLeaderBoard -= GetHighestLeaderBoardValue;
        FirebaseManager.onGetPaypalHistory -= GetPayPalHistory;
        FirebaseManager.onGetTournamentHistory -= OnGetTournamentHistory;
        foreach (ListenerRegistration l in ld)
        {
            l.Stop();
        }
        if (!GameHandler.Instance.isLoggedIn)
        {
            DeAttachFirebaseEvents();

        }
    }

    public void Play()
    {
        Invoke("LoadPlayScene", panelOpenCloseTime);
    }
    public void LoadPlayScene()
    {
        SceneManager.LoadScene("Demo - Portrait");
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
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void goToEmail(int index)
    {
        string email = "wordtourneyrewards@gmail.com";
        string subject = "";
        switch (index)
        {
            case 0:
                subject = "Account Management";
                break;
            case 1:
                subject = "Game Play Related";
                break;
            case 2:
                subject = "Report and Restriction";
                break;
            case 3:
                subject = "Purchase management";
                break;
            case 4:

                break;
            default:
                break;
        }
        string body = "ID:" + GameHandler.Instance.pID;
        subject = MyEscapeURL(subject);
        string mailto = $"mailto:{email}?subject={subject}&body={body}";
        Application.OpenURL(mailto);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void ShareScore(int shareScore)
    {
        int shareReward = shareScore;
        new NativeShare()
          //.AddFile(path)
          .SetSubject("Join My Game")
          .SetText(GameHandler.Instance.refereralLink).SetCallback(ShareResultCallback)
          .Share();
        //StartCoroutine(sendScore());
    }
    void ShareResultCallback(ShareResult result, string shareTarget)
    {
        if (result == ShareResult.Shared)
        {
            //progressData.keys += (int)shareReward;
            //SaveDataButton();
            //UpdateTxts();
        }
        else if (result == ShareResult.Unknown)
        {

        }
    }
    public void GetInviteReward(int btnNo)
    {
        progressData.keys += (int)InviteReward[btnNo];
        switch (btnNo)
        {
            case 0:
                progressData.invite1RewardGot = true;
                break;
            case 1:
                progressData.invite2RewardGot = true;
                break;
            case 2:
                progressData.invite3RewardGot = true;
                break;
            default:
                break;
        }

        SaveDataButton();
        UpdateTxts();
    }
    public void IAP_Packs(int pack)
    {
        switch (pack)
        {
            case 0:
                progressData.fireCracker += 20;
                progressData.hammer += 5;
                progressData.bulb += 5;
                break;
            case 1:
                progressData.fireCracker += 220;
                progressData.hammer += 45;
                progressData.bulb += 45;
                break;
            default:
                break;
        }
        FirebaseManager.Instance.SaveProgressData();
    }
    public void Cash_Packs(int pack)
    {
        switch (pack)
        {
            case 0:
                if (progressData.gems >= 15000)
                {
                    progressData.gems -= 15000;
                    progressData.fireCracker += 20;
                    progressData.hammer += 5;
                    progressData.bulb += 5;
                }

                break;
            case 1:
                if (progressData.gems >= 25000)
                {
                    progressData.gems -= 25000;
                    progressData.fireCracker += 220;
                    progressData.hammer += 45;
                    progressData.bulb += 45;
                }

                break;
            default:
                break;
        }
        UpdateTxts();
        FirebaseManager.Instance.SaveProgressData();
    }
    public void Exit()
    {
        Application.Quit();
    }
    bool isSoundPlay;
    int aa = 0;
    public void OnChangedLanguage(int val)
    {
        currentLanguage = allLanguages[val];
        //GameHandler.Instance.currentLanguage = currentLanguage;
        API.SetCurrentLanguage(currentLanguage);
        PlayerPrefs.SetInt("clang", val);
        PlayerPrefs.Save();
        ChangeTextsLanguage();
        if (isSoundPlay)
            PlaySoundBtnOpen();
        else
            isSoundPlay = true;
    }
    public void ChangeTextsLanguage()
    {
        foreach (TextWithLanguage t in allWords)
        {
            t.txt.text = Gley.Localization.API.GetText(t.wordId);
        }
    }
    public void IapCashPurchase(int cashVal)
    {
        progressData.tickets += (int)cashVal;
    }

    public void OpenTournamentPanel(GameObject CurrentPanel)
    {
        //if (PlayerPrefs.GetInt("SignedIn", 0) == 0)
        //{
        //    OpenPanel(tournamentSignInPanel);
        //}
        //else
        if (progressData.level >= progressData.tournamentUnlockLevel)
        {
            StopSoundBgSimple();
            PlaySoundBgTournamentSection();
            OpenPanel(tournamentPanel);
            ClosePanel(CurrentPanel);
        }
        else
        {
            OpenPanel(tournamentsLockedPanel);
        }
    }
    public void CloseTournamentPanel()
    {
        PlaySoundBgSimple();
        StopSoundBgTournamentSection();
    }
    public void OpenPanel(GameObject pn)
    {

        if (panelopenRoutine == null)
        {
            panelopenRoutine = StartCoroutine(OpenPanel(pn, true));
            if (isWowClub)
            {
                PlayerPrefs.SetString("musicClubBadge", "1");
            }
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
        if (pn.GetComponent<Canvas>())
        {
            pn.GetComponent<Canvas>().enabled = val;
            pn.SetActive(false);
            if (!pn.activeSelf) pn.SetActive(true);

            //DOTweenAnimation[] dta = pn.GetComponentsInChildren<DOTweenAnimation>();
            //foreach (DOTweenAnimation anim in dta)
            //{
            //    anim.DORestart();
            //}
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
        }
        else
            pn.SetActive(val);
        panelCloseRoutine = null;
    }
    public void SpinAnimatorWheelAd()
    {
        if (!isSpinning && progressData.spinWheelCount >= 0)
        {
            PlayerPrefs.SetString("spinwheeldate", GetDate());
            if (progressData.freeSpinWheel)
            {
                SpinAnimatorWheel();
            }
            else
            {
                isSpinWheelAd = true;
                GooglesAdsController.Instance?.ShowRewardedAd(rewardGiven =>
                {
                    if (rewardGiven)
                    {
                        SpinAnimatorWheel();
                    }
                });

                // RewardedAdController.instance.ShowRewardedAd();
            }

        }

    }
    string GetDate()
    {
        DateTime currentDate = DateTime.Now;
        return currentDate.ToString("yyyy-MM-dd");
    }
    public void SpinAnimatorWheelCash()
    {
        if (animatorWheelFees <= progressData.gems && !isSpinning)
        {

            progressData.gems -= (int)animatorWheelFees;
            SpinAnimatorWheel();
        }

    }
    public void SpinAnimatorWheel()
    {
        soundSpin.Stop();
        soundSpin.Play();
        isSpinning = true;
        animatorWheelResultNo = 0;
        int num = UnityEngine.Random.Range(0, 100);
        for (int i = 0; i < animatorWheelChances.Length; i++)
        {
            if (num < animatorWheelChances[i])
            {
                animatorWheelResultNo = i;
                break;
            }
        }
        animatorWheel.SetTrigger(animatorWheelTriggers[animatorWheelResultNo]);
        for (int i = 0; i < uiSpinButton.Length; i++)
        {
            uiSpinButton[i].interactable = false;
        }

        if (progressData.freeSpinWheel) progressData.freeSpinWheel = false;

        spinBtnAdObj.SetActive(true);

        progressData.spinWheelCount -= 1;

        if (progressData.spinWheelCount <= 0)
        {
            progressData.spinWheelCount = 0;
            spinBtnAdObj.SetActive(false);
        }
        else
        {

            spinBtnAdObj.SetActive(true);
        }


        foreach (var v in spinWheelCountTxt) v.text = progressData.spinWheelCount.ToString();



    }
    public void SpinFinishedAnimatorWheel()
    {
        foreach (var v in spinWheelRewardDetailTxt)
            v.gameObject.SetActive(false);
        spinWheelParticles.Play();
        soundSpinOpened.Play();
        int rewardAmound = 0;
        switch (spinRewards[animatorWheelResultNo].rewardType)
        {
            case "cash":
                progressData.gems += spinRewards[animatorWheelResultNo].rewardValue;
                spinRewardObtainedTxt.text = "YOU HAVE OBTAINED\n" + spinRewards[animatorWheelResultNo].rewardValue + " GEMS";
                rewardAmound = spinRewards[animatorWheelResultNo].rewardValue;
                SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.cashSound);
                StartCoroutine(ShowAnimationObjects(cashImg, rewardAmound, targetPositionCash, progressData.gems));
                spinWheelRewardDetailTxt[0].gameObject.SetActive(true);
                break;
            case "key":
                progressData.keys += spinRewards[animatorWheelResultNo].rewardValue;
                spinRewardObtainedTxt.text = "YOU HAVE OBTAINED\n" + spinRewards[animatorWheelResultNo].rewardValue + " KEYS";
                rewardAmound = spinRewards[animatorWheelResultNo].rewardValue;

                SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.keysSound);
                StartCoroutine(ShowAnimationObjects(keyImg, rewardAmound, targetPositionKey, progressData.keys));
                spinWheelRewardDetailTxt[1].gameObject.SetActive(true);
                break;
            case "gem":
                progressData.gems += spinRewards[animatorWheelResultNo].rewardValue;
                spinRewardObtainedTxt.text = "YOU HAVE OBTAINED\n" + spinRewards[animatorWheelResultNo].rewardValue + " GEMS";
                rewardAmound = spinRewards[animatorWheelResultNo].rewardValue;

                SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.gemsSound);

                StartCoroutine(ShowAnimationObjects(diamondImg, rewardAmound, targetPositionGem, progressData.gems));
                spinWheelRewardDetailTxt[2].gameObject.SetActive(true);
                break;
            default:
                break;
        }
        Debug.LogError("YOU HAVE OBTAINED\n" + spinRewards[animatorWheelResultNo].rewardValue + "  " + spinRewards[animatorWheelResultNo].rewardType);
        FirebaseManager.Instance.SaveProgressData();

        isSpinning = false;
        for (int i = 0; i < uiSpinButton.Length; i++)
        {
            uiSpinButton[i].interactable = true;
            uiSpinButton[3].gameObject.SetActive(true);
        }
        //UpdateTxts();
    }

    void ShowSpinRewardPanel(string totalAmount)
    {
        spinRewardPanel.SetActive(true);
        OpenPanel(spinRewardPanel);
        UpdateTxts();
        Debug.LogError("cchecking value: " + totalAmount);
    }

    IEnumerator ShowAnimationObjects(Sprite imgSprite, int countObjects, GameObject targetPos, int totalAmount)
    {
        int prevAmount = int.Parse(targetPos.transform.parent.transform.GetChild(2).gameObject.GetComponent<Text>().text);
        int diff = totalAmount - prevAmount;
        int incrementer = 0;
        if (diff < 15)
        {
            countObjects = diff;
            incrementer = 1;
        }
        else
        {
            countObjects = 15;
            incrementer = diff / 15;
        }
        for (int i = 0; i < countObjects; i++)
        {
            GameObject animatedPrefab = Instantiate(imgPrefab);
            animatedPrefab.SetActive(true);

            SetPosAndRect(animatedPrefab, imgPrefab.GetComponent<RectTransform>(), imgPrefab.transform.parent);
            animatedPrefab.GetComponent<Image>().sprite = imgSprite;
            prevAmount += incrementer;
            if (prevAmount > totalAmount)
                prevAmount = totalAmount;
            if (i >= countObjects - 1)
                prevAmount = totalAmount;

            animatedPrefab.GetComponent<AnimateObject>().SendToTarget(targetPos, prevAmount.ToString());

            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(1f);
        ShowSpinRewardPanel(totalAmount.ToString());
    }


    public void SetPosAndRect(GameObject InstantiatedObj, RectTransform ALReadyObjPos, Transform Parentobj)
    {
        InstantiatedObj.transform.parent = Parentobj;
        RectTransform MyPlayerRectTransform = InstantiatedObj.GetComponent<RectTransform>();
        MyPlayerRectTransform.localScale = ALReadyObjPos.localScale;
        MyPlayerRectTransform.localPosition = ALReadyObjPos.localPosition;
        MyPlayerRectTransform.anchorMin = ALReadyObjPos.anchorMin;
        MyPlayerRectTransform.anchorMax = ALReadyObjPos.anchorMax;
        MyPlayerRectTransform.anchoredPosition = ALReadyObjPos.anchoredPosition;
        MyPlayerRectTransform.sizeDelta = ALReadyObjPos.sizeDelta;
        MyPlayerRectTransform.localRotation = ALReadyObjPos.localRotation;

    }
    #region Login
    public void AttachFirebaseEvents()
    {
        FirebaseManager.onSuccessLogin += LoginSuccess;
        FirebaseManager.onFailedLogin += LoginFailed;
    }
    public void DeAttachFirebaseEvents()
    {
        FirebaseManager.onSuccessLogin -= LoginSuccess;
        FirebaseManager.onFailedLogin -= LoginFailed;


    }




    public void LoginSuccess(string msg)
    {

        warningLogInText.text = msg;
        GameHandler.Instance.isLoggedIn = true;
        DeAttachFirebaseEvents();
        mainLoginBtn.SetActive(false);
        //updateUsernameField();
        mainLoginSelectionUI.SetActive(false);
        mainPanel.SetActive(true);
        UpdateTxts();


    }
    public void LoginFailed(string msg)
    {
        warningLogInText.text = msg;

    }


    public void GoogleSignIn()
    {
        FirebaseManager.Instance.SignInWithGoogle();
    }
    public void FacebookSignIn()
    {
        //FirebaseManager.Instance.Facebook_LogIn();
    }
    #endregion
    #region Store
    public void GetFreeGems()
    {
        storefreeGemsLastDateTime = DateTime.Now;
        PlayerPrefs.SetString("storefreeGemsLastDateTime", storefreeGemsLastDateTime.ToString());
        PlayerPrefs.Save();
        storefreeGemsBtn.interactable = false;
        progressData.gems += 50;
        UpdateTxts();
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Free Gems\n 50";
        PurchaseSuccessPanel.SetActive(true);
    }
    public void Store_Packs(string pack)
    {
        isWowClub = false;
        switch (pack)
        {
            case "master":
                if (progressData.gems >= 10000)
                {
                    progressData.gems -= 10000;
                    progressData.fireCracker += 250;
                    progressData.hammer += 40;
                    progressData.bulb += 40;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 250, 40, 40);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "encyclopedia":
                if (progressData.gems >= 15000)
                {
                    progressData.gems -= 15000;
                    progressData.fireCracker += 100;
                    progressData.hammer += 50;
                    progressData.bulb += 300;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 100, 50, 300);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "workbook":
                if (progressData.gems >= 2500)
                {
                    progressData.gems -= 2500;
                    progressData.fireCracker += 20;
                    progressData.hammer += 10;
                    progressData.bulb += 5;
                    PurchaseSuccessDetail.text = string.Format("Rockets: {0}\nHemmers: {1}\nBulbs: {2}", 20, 10, 5);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }

                break;
            case "key1":
                if (progressData.gems >= 1000)
                {
                    progressData.gems -= 1000;
                    progressData.keys += 1;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 1);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "key2":
                if (progressData.gems >= 8000)
                {
                    progressData.gems -= 1000;
                    progressData.keys += 10;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 10);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "key3":
                if (progressData.gems >= 40000)
                {
                    progressData.gems -= 40000;
                    progressData.keys += 50;
                    PurchaseSuccessDetail.text = string.Format("Keys: {0}", 50);
                    PurchaseSuccessPanel.SetActive(true);
                    OpenPanel(PurchaseSuccessPanel);
                }
                break;
            case "noAd":
                progressData.noAds = true;
                PurchaseSuccessDetail.text = "Ads are off now";
                PurchaseSuccessPanel.SetActive(true);
                OpenPanel(PurchaseSuccessPanel);
                break;
            case "wow":
                progressData.noAds = true;
                progressData.gems += 1000;
                progressData.fireCracker += 10;
                PurchaseSuccessDetail.text = string.Format("Gems: {0}\nRockets: {1}\n no ads forever", 1000, 10);
                PurchaseSuccessPanel.SetActive(true);
                isWowClub = true;
                OpenPanel(PurchaseSuccessPanel);

                break;
            default:
                break;
        }
        UpdateTxts();
        FirebaseManager.Instance.SaveProgressData();
    }
    bool isWowClub;
    public void GetHammer(int val)
    {
        progressData.hammer += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Hammers\n" + val;
        PurchaseSuccessPanel.SetActive(true);
    }
    public void GetBulb(int val)
    {
        progressData.bulb += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Bulbs\n" + val;
        PurchaseSuccessPanel.SetActive(true);
    }
    public void GetGems(int val)
    {
        progressData.gems += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        UpdateTxts();
        PurchaseSuccessDetail.text = "Gems\n" + val;
        PurchaseSuccessPanel.SetActive(true);
    }
    public void GetfireCracker(int val)
    {
        progressData.fireCracker += (int)val;
        FirebaseManager.Instance.SaveProgressData();
        PurchaseSuccessDetail.text = "Rockets\n" + val;
        PurchaseSuccessPanel.SetActive(true);
    }
    public void BuyAvatar()
    {
        if (progressData.gems > 10000)
        {
            progressData.gems -= 10000;
            progressData.playerProfilePic = FirebaseManager.Instance.ConvertTextureToString(FirebaseManager.Instance.avatarPicTexture.texture);
            updateUsernameField();
            UpdateTxts();
            FirebaseManager.Instance.SaveProgressData();
            PurchaseSuccessDetail.text = "Avatar Got";
            PurchaseSuccessPanel.SetActive(true);
        }
    }

    #endregion
    #region PayPal
    public void CashInPayPal()
    {
        payPalMehtod = PayPalMehtod.cashin;
        OpenPanel(payPalPaymentPanel);
    }
    public void CashOutPayPal()
    {
        payPalMehtod = PayPalMehtod.cashout;
        OpenPanel(payPalPaymentPanel);
    }
    public void ProceedPayPalPayment()
    {

        if (payPalMehtod == PayPalMehtod.cashout)
        {
            if (progressData.tickets <= 0)
            {
                payPalAmountInputFieldResultDetail.text = "Empty Wallet";
            }
            else if (payPalEmailInputField.text.Length <= 0)
            {
                payPalAmountInputFieldResultDetail.text = "Enter Email";
            }
            else if (payoutInProgress)
            {

                payPalAmountInputFieldResultDetail.text = "Old Request In Progress";


            }
            else if (progressData.tickets < progressData.paypalWithDrawAmount)
            {

                payPalAmountInputFieldResultDetail.text = "Not Enough Cash\nMinimum Withdrawal: " + progressData.paypalWithDrawAmount;


            }
            else
            {
                payoutInProgress = true;
                payPalManager.currentPayment = progressData.tickets;
                payPalManager.currentReciever = payPalEmailInputField.text;
                payPalManager.StartPayout();

            }
        }

    }
    public void ClearPayPalResult()
    {
        payPalAmountInputFieldResultDetail.text = "";
        payPalAmountInputField.text = "";
        payPalEmailInputField.text = "";
    }
    public void GetPayPalHistory(string tID, string Cash, bool done)
    {
        //Instantiate new scoreboard elements


        GameObject scoreboardElement = Instantiate(paypalHistoryElement, paypalHistoryContent);
        scoreboardElement.GetComponent<PayPalHistoryElement>().NewScoreElement(tID, Cash, done);


    }
    #endregion

    #region Tournament History
    public void OnGetTournamentHistory(string tournamentID, TournamentHistory tData)
    {
        if (tData.position == "")
        {

            GameObject th = Instantiate(TournamentHistoryPrefab, trnmntHstryCntnr);
            th.GetComponent<TournamentHistoryPrefab>().LoadData("In Progress", tournamentID, tData.position, tData.timeTaken, tData.badgeInd,tData.prize,tData.endDate,tData.entryFee, completedTrnmntHstryCntnr);
            int referenceIndex = inProgressTrnmntHstryCntnr.GetSiblingIndex();
            int newSiblingIndex = referenceIndex + 1;
            th.transform.SetSiblingIndex(newSiblingIndex);

        }
        else
        {
            GameObject th = Instantiate(TournamentHistoryPrefab, trnmntHstryCntnr);
            th.GetComponent<TournamentHistoryPrefab>().LoadData("Completed", tournamentID, tData.position, tData.timeTaken, tData.badgeInd, tData.prize, tData.endDate, tData.entryFee, completedTrnmntHstryCntnr);
            int referenceIndex = completedTrnmntHstryCntnr.GetSiblingIndex();
            int newSiblingIndex = referenceIndex + 1;
            th.transform.SetSiblingIndex(newSiblingIndex);
        }
    }
    #endregion
    public void StartFireStore()
    {
        //tournamentDetailContainers = new Dictionary<string, TournamentDetailContainer>();
        //for (int i = tournamentContainerParent.childCount - 1; i >= 0; i--)
        //{
        //    Destroy(tournamentContainerParent.GetChild(0).gameObject);
        //}
        //tournamentDetailContainers.Clear();
        foreach (ListenerRegistration l in ld)
        {
            l.Stop();
        }
        ld.Clear();
        FirebaseManager.Instance.dbf.Collection("Tournaments").GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
        {
            tournamentCreated = snapshot.Result.Documents.Count();
            Debug.LogError("-------Tournament created: " + snapshot.Result.Documents);
            int tournamentNo = 0;
            foreach (var item in snapshot.Result.Documents)
            {
                TournamentDetailContainer t = new TournamentDetailContainer();
                if (tournamentDetailContainers.ContainsKey(item.Id))
                {
                    t = tournamentDetailContainers[item.Id];
                }
                else
                {

                    t = Instantiate(tournamentPrefab, tournamentTypes[0].tournamentContainerParent).GetComponent<TournamentDetailContainer>();
                    tournamentDetailContainers.Add(item.Id, t);
                }

                int tNoNew = tournamentNo;
                Debug.LogError("-------Tournament created: " + tNoNew);
                item.Reference.Collection("Detail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {

                        QuerySnapshot snapshot = task.Result;
                        TournamentPlayerData tp = new TournamentPlayerData();
                        DocumentSnapshot PlayersListener = null;
                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            Dictionary<string, object> data = document.ToDictionary();

                            if (document.Id == "Country")
                            {
                                t.CheckCountries(data);
                            }
                            else if (document.Id == "Players")
                            {
                                PlayersListener = document;

                                if (data.ContainsKey(FirebaseManager.Instance.User.UserId))
                                {


                                    tp = new TournamentPlayerData(JsonConvert.DeserializeObject<TournamentPlayerData>(data[FirebaseManager.Instance.User.UserId].ToString()));

                                }

                                t.CheckPlayers(data);
                            }
                            else if (document.Id == "PrimaryDetail")
                            {
                                t.tournamentPrizeDistributionCount = data["TournamentPrizeDistributionCount"].ConvertTo<int>();

                                TournamentTypes tType = tournamentTypes.Find(t => t.typeName == data["Type"].ConvertTo<string>());
                                if (tType != null)
                                {
                                    t.transform.parent = tType.tournamentContainerParent;
                                    t.bgImg.sprite = tType.tournamentPrefabSprite;
                                }

                                t.SaveDatail(data["EntryFee"].ConvertTo<int>(),
                                    data["Prize"].ConvertTo<int>(),
                                    item.Id,
                                    data["IsStarted"].ConvertTo<bool>(),
                                    data["EndDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime(),
                                    data["PlayersReq"].ConvertTo<int>(),
                                    tNoNew, data["StartDate"].ConvertTo<Timestamp>().ToDateTime().ToLocalTime());


                            }


                        }
                        if (PlayersListener != null)
                        {

                            ListenerRegistration lld = PlayersListener.Reference.Listen(task =>
                            {
                                Dictionary<string, object> data22 = task.ToDictionary();

                                if (data22.ContainsKey(FirebaseManager.Instance.User.UserId))
                                {


                                    tp = new TournamentPlayerData(JsonConvert.DeserializeObject<TournamentPlayerData>(data22[FirebaseManager.Instance.User.UserId].ToString()));

                                }
                                t.leaderBoards.Clear();
                                t.CheckPlayers(data22, true);
                                StartLoadScoreboardDataTournament(t, (tp.playerID != "" ? tp : null));
                            });
                            ld.Add(lld);
                        }
                        else
                        {
                            StartLoadScoreboardDataTournament(t, (tp.playerID != "" ? tp : null));
                        }
                        GetTournamentPrizes(t);
                        if (t.counter.endDate.ToLocalTime() > DateTime.Now)
                        {
                            if (t.counter.endDate.ToLocalTime().Subtract(DateTime.Now).TotalMinutes < 60)
                            {
                                StartCoroutine(TournamentEnder(t.counter.endDate, t, (tp.playerID != "" ? tp : null)));
                            }
                        }
                    }
                    else
                    {

                        Debug.LogError("Failed to get nested documents: " + task.Exception);
                    }
                    tournamentDataLoaded = true;
                });
                item.Reference.Collection("TournamentPrizeDistributionCategory").GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        t.TournamentPrizeDistributionCategory = new List<TournamentPrizes>();
                        QuerySnapshot snapshot = task.Result;

                        foreach (var vd in snapshot)
                        {

                            TournamentPrizes tournamentPrizes = new TournamentPrizes();


                            Dictionary<string, object> data2 = vd.ToDictionary();
                            foreach (var pd in data2)
                            {

                                TournamentPrize tournamentPrize = new TournamentPrize();
                                tournamentPrize.pValue = int.Parse(pd.Value.ToString());
                                tournamentPrize.pType = pd.Key;
                                tournamentPrizes.prize.Add(tournamentPrize);
                            }

                            t.TournamentPrizeDistributionCategory.Add(tournamentPrizes);



                        }


                    }
                    else
                    {

                        Debug.LogError("Failed to get nested documents: " + task.Exception);
                    }

                });


            }


            FirebaseManager.Instance.LoadTournamentHistory();
        });

        // GetData();
    }

    public void SavePrizeValues(TournamentDetailContainer td2, DateTime endTime)
    {
        int totalCash = 0;
        int totalGems = 0;
        int totalKeys = 0;

        foreach (var prizeCategory in td2.TournamentPrizeDistributionCategory)
        {
            foreach (var prize in prizeCategory.prize)
            {
                switch (prize.pType)
                {
                    case "cash":
                        totalCash += (int)prize.pValue;
                        break;
                    case "gems":
                        totalGems += (int)prize.pValue;
                        break;
                    case "keys":
                        totalKeys += (int)prize.pValue;
                        break;
                }
            }
        }

        PlayerPrefs.SetString("endTimeTournament", endTime.ToString());
        PlayerPrefs.SetInt("tournament_cash", totalCash);
        PlayerPrefs.SetInt("tournament_gems", totalGems);
        PlayerPrefs.SetInt("tournament_keys", totalKeys);
        PlayerPrefs.Save();

        Debug.Log("Prize values saved to PlayerPrefs: Cash = " + totalCash + ", Gems = " + totalGems + ", Keys = " + totalKeys);
    }
    public void GetTournamentPrizes(TournamentDetailContainer td2)
    {
        //foreach (var prizeCategory in td2.TournamentPrizeDistributionCategory)
        //{
        //    foreach (var prize in prizeCategory.prize)
        //    {
        //        switch (prize.pType)
        //        {
        //            case "cash":

        //                tCashss = (int)prize.pValue;
        //                break;
        //            case "gems":
        //                tGemss = (int)prize.pValue;
        //                break;
        //            case "keys":
        //                tKeyss = (int)prize.pValue;
        //                break;
        //        }
        //    }
        //}
    }
    IEnumerator TournamentEnder(DateTime endTime, TournamentDetailContainer td2, TournamentPlayerData td = null)
    {
        SavePrizeValues(td2, endTime);
        while (true)
        {
            yield return null;
            if (DateTime.Now > endTime)
            {
                StartLoadScoreboardDataTournament(td2, td);
                break;
            }
        }
    }
    public void StartLoadScoreboardDataTournament(TournamentDetailContainer td2, TournamentPlayerData td = null)
    {
        StartCoroutine(LoadScoreboardDataTournament(td2, td));
    }
    private IEnumerator LoadScoreboardDataTournament(TournamentDetailContainer td2, TournamentPlayerData td = null)
    {
     TournamentHistory th = new TournamentHistory();
        Task<DocumentSnapshot> DBTask = FirebaseManager.Instance.dbf.Collection("Tournaments").Document(td2.tNameVar).Collection("Detail").Document("Players").GetSnapshotAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            td2.leaderBoards.Clear();
            //Data has been retrieved
            DocumentSnapshot snapshot = DBTask.Result;

            Dictionary<string, object> dt = snapshot.ToDictionary();
            Dictionary<string, int> dtWithLevel = new Dictionary<string, int>();
            Dictionary<string, int> dtforPrize = new Dictionary<string, int>();
            List<TournamentPlayerData> PlayerDataList = new List<TournamentPlayerData>();
            foreach (var v in dt)
            {
                TournamentPlayerData tt = new TournamentPlayerData(JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()));
                PlayerDataList.Add(tt);
                dtWithLevel.Add(tt.playerName, tt.level);
                dtforPrize.Add(tt.playerID, tt.level);
            }





            var sortedDict = dtWithLevel.OrderByDescending(pair => pair.Value);
            var sortedDictForPrize = dtforPrize.OrderByDescending(pair => pair.Value);

            List<KeyValuePair<string, int>> sortedList = sortedDict.ToList();
            List<KeyValuePair<string, int>> sortedListForPrize = sortedDictForPrize.ToList();



            int iRange = 0;

            bool tournamentLevelShow = td2.counter.endDate.ToLocalTime() <= DateTime.Now;

            //Loop through every users UID
            foreach (KeyValuePair<string, int> childSnapshot in sortedDict)
            {



                string username = childSnapshot.Key;
                int level = -1;
                if (tournamentLevelShow)
                {
                    level = childSnapshot.Value;
                }
                TLeaderBoard tl = new TLeaderBoard();
                tl.username = username;
                TournamentPlayerData dttd = PlayerDataList.Find(dt => dt.playerName == username);
                if (dttd != null)
                {
                    tl.timeTaken = dttd.time;
                }
                tl.level = level;
                tl.iRange = iRange + 1;
                tl.cash=(int)dttd.time;
                td2.leaderBoards.Add(tl);
                iRange++;
                if (iRange >= FirebaseManager.Instance.LeaderBoardValuesCount)
                {
                    break;
                }




            }
            if (td != null)
                if (tournamentLevelShow && !td.prizeObtained)
                {

                    int prizeRange = 0;
                    int lastLevel = -1;
                    int playerPos = 1;

                    foreach (KeyValuePair<string, int> childSnapshot in sortedDictForPrize)
                    {

                        if (childSnapshot.Key == FirebaseManager.Instance.User.UserId)
                        {
                            bool tied = false;
                            if (lastLevel != -1)
                            {
                                if (lastLevel == childSnapshot.Value) tied = true;
                            }
                            if (prizeRange < td2.TournamentPrizeDistributionCategory.Count)
                            {
                                foreach (TournamentPrize tpr in td2.TournamentPrizeDistributionCategory[prizeRange].prize)
                                {
                                    TournamentRewards tp = new TournamentRewards();
                                    switch (tpr.pType)
                                    {
                                        case "keys":

                                            progressData.keys += (int)tpr.pValue;
                                            break;
                                        case "gems":
                                            progressData.gems += (int)tpr.pValue;

                                            break;
                                        case "cash":
                                            progressData.tickets += (int)tpr.pValue;

                                            break;
                                        default:
                                            progressData.gems += (int)tpr.pValue;
                                            break;
                                    }
                                    tp.pValue = tpr.pValue;
                                    tp.pType = tpr.pType;

                                    tp.tournamentName = td.tournamentName;
                                    if (tied)
                                    {
                                        tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                    }
                                    tournamentRewardPrizes.Add(tp);
                                    th.tournamentName = tp.tournamentName;
                                    th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                    th.badgeInd = 0;
                                    th.position = playerPos.ToString();
                                    th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                    th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                    th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                    FirebaseManager.Instance.UpdateTournamentHistoryData(tp.tournamentName, JsonConvert.SerializeObject(th));
                                    if (PlayerPrefs.HasKey("tournament_cash"))
                                    {
                                        PlayerPrefs.DeleteKey("tournament_cash");
                                        PlayerPrefs.DeleteKey("tournament_gems");
                                        PlayerPrefs.DeleteKey("tournament_keys");
                                        PlayerPrefs.DeleteKey("endTimeTournament");
                                    }
                                }

                            }
                            else
                            {
                                TournamentRewards tp = new TournamentRewards();
                                progressData.tickets += (int)tournamentDetailContainers.First().Value.counter.prize;
                                tp.pValue = tournamentDetailContainers.First().Value.counter.prize;
                                tp.pType = "keys";
                                tp.tournamentName = td.tournamentName;
                                if (tied)
                                {
                                    tp.tournamentName = "TIED\n" + tp.tournamentName + "\nPosition: " + playerPos;
                                }
                                tournamentRewardPrizes.Add(tp);
                                th.tournamentName = tp.tournamentName;
                                th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                                th.badgeInd = 0;
                                th.position = playerPos.ToString();
                                th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                                th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                                th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                                FirebaseManager.Instance.UpdateTournamentHistoryData(tp.tournamentName, JsonConvert.SerializeObject(th));
                            }
                            progressData.wonTournament++;
                            td.prizeObtained = true;

                            FirebaseManager.Instance.UpdateTournamentProgress(td.tournamentName, td);
                            break;
                        }
                        else
                        {
                            lastLevel = childSnapshot.Value;
                        }
                        prizeRange++;
                        if (prizeRange >= td2.tournamentPrizeDistributionCount || prizeRange >= td2.TournamentPrizeDistributionCategory.Count)
                        {
                            progressData.failedTournament++;
                            td.prizeObtained = true;
                            th.tournamentName = td.tournamentName;
                            th.timeTaken = GameHandler.Instance.tournamentCurrentPlayerData.time.ToString();
                            th.badgeInd = 0;
                            th.position = "No Position";
                            th.endDate = GameHandler.Instance.tournamentCurrentPlayerData.endDate;
                            th.entryFee = GameHandler.Instance.tournamentCurrentPlayerData.entryFee;
                            th.prize = GameHandler.Instance.tournamentCurrentPlayerData.prize;
                            FirebaseManager.Instance.UpdateTournamentHistoryData(td.tournamentName, JsonConvert.SerializeObject(th));
                            break;
                        }
                        playerPos++;
                    }
                    FirebaseManager.Instance.SaveProgressData();
                    UpdateTournamentStats();
                }
            if (td2.counter.tournamentNo == 0)
            {
                LoadLeaderBoard(td2.tNameVar);
            }
        }
        if (tournamentCounted < tournamentCreated) tournamentCounted++;
        if (tournamentCounted >= tournamentCreated)
        {
            if (PlayerPrefs.GetInt("toturialMenu", 0) != 0 && !tournamentRewardPage.activeSelf)
                ShowRewardTournament();
        }
    }

    public Animator reward;

    public void showAnimatior(GameObject obj)
    {
        Animator anim = obj.GetComponent<Animator>();
        if (anim)
        {
            anim.enabled = true;
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            float delay = stateInfo.normalizedTime;
            Debug.LogError("Delay time: " + delay);
        }
        StartCoroutine(disableAniamation(obj, 4));
        //if (PlayerPrefs.GetInt("myPosition") == 1)
        //{
        //    SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.firstPlaceSound);
        //}
        //else if (PlayerPrefs.GetInt("myPosition") == 2)
        //{
        //    SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.secondPlaceSound);
        //}
        //else if (PlayerPrefs.GetInt("myPosition") == 3)
        //{
        //    SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.thirdPlaceSound);
        //}
        //PlayerPrefs.DeleteKey("myPosition");

    }

    IEnumerator disableAniamation(GameObject obj, float delay)
    {

        yield return new WaitForSeconds(delay);
        Animator anim = obj.GetComponent<Animator>();
        if (anim)
        {
            anim.enabled = false;
            anim.Rebind();
        }
        obj.transform.parent.parent.gameObject.SetActive(false);
        //if (tournamentRewardPrizes.Count > 0)
        //    tournamentRewardPrizes.RemoveAt(0);
    }
    public void ShowRewardTournament()
    {


        if (tournamentRewardPrizes.Count > 0)
        {
            switch (tournamentRewardPrizes[0].pType)
            {
                case "keys":
                    // tournamentRewardImage.sprite = tournamentRewardImages[0];
                    // tournamentRewardImage2.sprite = tournamentRewardImages2[0];
                    tournamentRewardKeysTxt.text = tournamentRewardPrizes[0].pValue.ToString();
                    tournamentRewardNameKeysTxt.text = tournamentRewardPrizes[0].tournamentName;

                    //tournamentRewardKeysPage.SetActive(true);

                    if (tournamentRewardPrizes[0].pValue > 0)
                    {
                        Invoke(nameof(keyspage), 4);
                    }

                    break;
                case "gems":
                    //  tournamentRewardImage.sprite = tournamentRewardImages[1];
                    // tournamentRewardImage2.sprite = tournamentRewardImages2[1];
                    tournamentRewardGemsTxt.text = tournamentRewardPrizes[0].pValue.ToString();
                    tournamentRewardNameGemsTxt.text = tournamentRewardPrizes[0].tournamentName;

                    //tournamentRewardGemsPage.SetActive(true);
                    if (tournamentRewardPrizes[0].pValue > 0)
                    {
                        Invoke(nameof(gemspage), 4);
                    }

                    break;
                case "cash":
                    // tournamentRewardImage.sprite = tournamentRewardImages[2];
                    //  tournamentRewardImage2.sprite = tournamentRewardImages2[2];
                    tournamentRewardTxt.text = tournamentRewardPrizes[0].pValue.ToString();
                    tournamentRewardNameTxt.text = tournamentRewardPrizes[0].tournamentName;
                    tournamentRewardKeyplacetext.text = tournamentRewardPrizes[0].pValue.ToString();
                    tournamentRewardPage.SetActive(true);
                    if (PlayerPrefs.GetInt("myPosition") == 1)
                    {
                        SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.firstPlaceSound);
                        TournamentPlace[0].SetActive(true);
                        // tournamentRewardKeyplacetext.gameObject.SetActive(true);
                    }
                    else if (PlayerPrefs.GetInt("myPosition") == 2)
                    {
                        TournamentPlace[1].SetActive(true);
                        tournamentRewardKeyplacetext.gameObject.SetActive(true);
                        SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.secondPlaceSound);
                    }
                    else if (PlayerPrefs.GetInt("myPosition") == 3)
                    {
                        TournamentPlace[2].SetActive(true);
                        tournamentRewardKeyplacetext.gameObject.SetActive(true);
                        SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.thirdPlaceSound);
                    }
                    PlayerPrefs.DeleteKey("myPosition");
                    break;
            }
            tournamentRewardPrizes.RemoveAt(0);
        }
    }

    public void PlaySoundOfCollectAbles(string item)
    {
        if (item == "key")
            SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.keysSound);
        else if (item == "gem")
            SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.gemsSound);
        else if (item == "cash")
            SoundManager.Instance.PlayAudioClip(SoundManager.Instance.AllSounds.cashSound);

    }
    public void GiveRewardWhenNoRecord()
    {
        //PlayerPrefs.SetInt("tournament_cash", 10);
        //PlayerPrefs.SetInt("tournament_gems", 20);
        //PlayerPrefs.SetInt("tournament_keys", 5);
        if (!PlayerPrefs.HasKey("endTimeTournament") || !PlayerPrefs.HasKey("tournament_cash"))
            return;
        string endDatee = PlayerPrefs.GetString("endTimeTournament");
        DateTime endTim = DateTime.Parse(endDatee);
        DateTime endTim2 = endTim.AddHours(25);

        if (endTim2 > DateTime.Now)
            return;
        if (PlayerPrefs.HasKey("tournament_cash"))
        {
            StartCoroutine(ShowRewardTournamentWhenNoRecord());
        }
    }
    int counter = 0;
    IEnumerator ShowRewardTournamentWhenNoRecord()
    {
        yield return new WaitForSeconds(0f);

        string prizeType = "";
        if (counter == 0)
            prizeType = "cash";
        if (counter == 1)
            prizeType = "gems";
        if (counter == 2)
            prizeType = "keys";

        Debug.LogError("Auto start checking : " + counter);

        if (counter < 3)
        {
            switch (prizeType)
            {
                case "keys":
                    tournamentRewardImage.sprite = tournamentRewardImages[0];
                    tournamentRewardImage2.sprite = tournamentRewardImages2[0];
                    tournamentRewardKeysTxt.text = PlayerPrefs.GetInt("tournament_keys").ToString();
                    tournamentRewardNameKeysTxt.text = "";
                    Invoke(nameof(keyspage), 4);
                    counter++;
                    if (counter == 3)
                    {
                        removeKeys();
                    }
                    break;
                case "gems":
                    tournamentRewardImage.sprite = tournamentRewardImages[1];
                    tournamentRewardImage2.sprite = tournamentRewardImages2[1];
                    tournamentRewardGemsTxt.text = PlayerPrefs.GetInt("tournament_gems").ToString();
                    tournamentRewardNameGemsTxt.text = "";
                    Invoke(nameof(gemspage), 4);
                    counter++;
                    if (counter == 3)
                    {
                        removeKeys();
                    }
                    break;
                case "cash":
                    tournamentRewardImage.sprite = tournamentRewardImages[2];
                    tournamentRewardImage2.sprite = tournamentRewardImages2[2];
                    tournamentRewardTxt.text = PlayerPrefs.GetInt("tournament_cash").ToString();
                    tournamentRewardNameTxt.text = "";
                    tournamentRewardPage.SetActive(true);
                    progressData.gems += PlayerPrefs.GetInt("tournament_gems");
                    progressData.keys += PlayerPrefs.GetInt("tournament_keys");
                    progressData.tickets += PlayerPrefs.GetInt("tournament_cash");
                    counter++;
                    if (counter == 3)
                    {
                        removeKeys();
                    }
                    break;
            }
        }
    }
    void removeKeys()
    {
        PlayerPrefs.DeleteKey("tournament_cash");
        PlayerPrefs.DeleteKey("tournament_gems");
        PlayerPrefs.DeleteKey("tournament_keys");
        PlayerPrefs.DeleteKey("endTimeTournament");
    }
    void gemspage()
    {
        tournamentRewardGemsPage.SetActive(true);
    }
    void keyspage()
    {
        tournamentRewardKeysPage.SetActive(true);
    }
    public void PrizeDistribution()
    {
        if (tournamentRewardPrizes.Count > 0)
        {
            switch (tournamentRewardPrizes[0].pType)
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
            tournamentRewardTxt.text = tournamentRewardPrizes[0].pValue.ToString();
            tournamentRewardNameTxt.text = tournamentRewardPrizes[0].tournamentName;
            tournamentRewardPage.SetActive(true);
            tournamentRewardPrizes.RemoveAt(0);
        }
    }
    void GetDataFireStore()
    {

        FirebaseManager.Instance.dbf.Collection("counters").Document("counter").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Counter counter = task.Result.ConvertTo<Counter>();
            //countUI.text = counter.Count.ToString();
        });
    }
    public void LoadHighestLeaderBoard()
    {
        FirebaseManager.onGetHighestLeaderBoard += GetHighestLeaderBoardValue;
        //Destroy any existing scoreboard elements
        for (int i = scoreboardHighestContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreboardHighestContent.transform.GetChild(i).gameObject);
        }


        FirebaseManager.Instance.LoadHighestWinLeaderBoard();
    }
    public void GetHighestLeaderBoardValue(string username, int wins, int pos, int cash)
    {
        //Instantiate new scoreboard elements

        if (noDataHighestLeaderBoardObj.activeSelf) noDataHighestLeaderBoardObj.SetActive(false);
        GameObject scoreboardElement = Instantiate(scoreHighestElement, scoreboardHighestContent);
        scoreboardElement.GetComponent<ScoreElement>().NewHighestScoreElement(username, cash, pos);


    }
}

[System.Serializable]
public class TextWithLanguage
{
    public Text txt;
    public WordIDs wordId;
}
[System.Serializable]
public class TournamentPlayerData
{
    public string playerName;
    public string playerID;
    public string tournamentName;
    public int tournamentNo;
    public int level = 1;
    public int lamps = 2;
    public int hammers = 0;
    public float time = 0;
    public bool prizeObtained = false;
    public string wordsFound;
    public DateTime endDate;
    public int prize;
    public int entryFee;
    public TournamentPlayerData(TournamentPlayerData t)
    {
        playerName = t.playerName;
        playerID = t.playerID;
        tournamentName = t.tournamentName;
        tournamentNo = t.tournamentNo;
        level = t.level;
        time = t.time;
        prizeObtained = t.prizeObtained;
        wordsFound = t.wordsFound;
        endDate = t.endDate;
        lamps = t.lamps;
        hammers = t.hammers;
        prize = t.prize;
        entryFee = t.entryFee;
    }
    public TournamentPlayerData()
    {
        playerName = "";
        playerID = "";
        tournamentName = "";
        tournamentNo = 0;
        level = 0;
        time = 0;
        prizeObtained = false;
        wordsFound = "";
        endDate = DateTime.Now;
        lamps = 2;
        hammers = 0;
        entryFee=0;
        prize = 0;
    }
    public void Copy(TournamentPlayerData t)
    {
        playerName = t.playerName;
        playerID = t.playerID;
        tournamentName = t.tournamentName;
        tournamentNo = t.tournamentNo;
        level = t.level;
        time = t.time;
        prizeObtained = t.prizeObtained;
        wordsFound = t.wordsFound;
        endDate = t.endDate;
        hammers = t.hammers;
        lamps = t.lamps;
        prize = t.prize;
        entryFee = t.entryFee;
    
    }
}
[System.Serializable]
[FirestoreData]
public struct Counter
{


    public DateTime endDate;
    public DateTime startDate;

    public int tournamentNo;

    public int entryFee;

    public int playersReq;


    public int prize;


    public bool isStarted;
}


[System.Serializable]
public struct SpinReward
{
    public int rewardValue;
    public string rewardType;
}
[System.Serializable]
public class TournamentTypes
{
    public string typeName;
    public GameObject ContainerPanel;
    public Transform tournamentContainerParent;
    public GameObject tournamentLock;
    public int tournamentUnlockLevel;
    public Image tournamentImg;
    public Sprite tournamentPrefabSprite;
    public Sprite tournamentBadgeSprite;
}
[System.Serializable]
public class MedalAchievement
{
    public string name;
    public int levelThreshold;
    public Sprite medalImage;
}
