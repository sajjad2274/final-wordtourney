using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Messaging;
//using Facebook.Unity;
using Google;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using DTT.WordConnect;
using Newtonsoft.Json.Linq;
using System.Net;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using System.IO;



public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public delegate void FireBaseManagerDelegate();
    public delegate void FireBaseManagerDelegateString(string msg);
    public delegate void FireBaseManagerDelegateStringInt(string n, int p1, int p2, int p3);
    public delegate void FireBaseManagerDelegateTournamentLeader(string n, int p1, int p3);
    public delegate void FireBaseManagerDelegatePaypalHistory(string n, string p1, bool done);
    public delegate void FireBaseManagerDelegateTournamentHistory(string n, TournamentHistory p1);


    public static event FireBaseManagerDelegateString onSuccessLogin;
    public static event FireBaseManagerDelegateString onFailedLogin;
    public static event FireBaseManagerDelegateString onSuccessAnonymous;
    public static event FireBaseManagerDelegateString onFailedAnonymous;
    public static event FireBaseManagerDelegate onFailedAutoLogin;
    public static event FireBaseManagerDelegateStringInt onGetLeaderBoard;
    public static event FireBaseManagerDelegateStringInt onGetHighestLeaderBoard;
    public static event FireBaseManagerDelegateTournamentLeader onGetLeaderBoardTournament;
    public static event FireBaseManagerDelegatePaypalHistory onGetPaypalHistory;
    public static event FireBaseManagerDelegateTournamentHistory onGetTournamentHistory;


    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    public FirebaseFirestore dbf;
    private GoogleSignInConfiguration configuration;

    public GameProgressData progressData;


    public int LeaderBoardValuesCount;

    public string webClientIdGoogle = "<your client id here>";

    [Header("Profile Picture")]
    public Sprite avatarPicTexture;
    string pgsAuthCode = "";

    DatabaseReference dataReference;
    DatabaseReference dataReference2;

    public GameObject fetchTournamentLevelsObj;
    // public TextMeshProUGUI infoText;

    #region Init
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        PlayGamesPlatform.Activate();
        // StartCoroutine(AskForPermissions());
    }

    public async Task LoadData()
    {


        //InitializeFirebase();
        // Check that all of the necessary dependencies for Firebase are present on the system
        configuration = new GoogleSignInConfiguration { WebClientId = webClientIdGoogle, RequestEmail = true, RequestIdToken = true };

        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();


        if (dependencyStatus == DependencyStatus.Available)
        {
            //If they are avalible Initialize Firebase
            InitializeFirebase();


        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }

        LoadTournamentLevels();
        await Task.Delay((int)(3000));

    }
    public async Task LoadLevelsStart()
    {

        await LoadLevelsEnum().AsTask(this);
    }
    public void CheckAutoLogin()
    {

        if (auth.CurrentUser != null)
        {
            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + auth.CurrentUser.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
            GameHandler.Instance.isLoggedIn = true;
            StartCoroutine(AutoLogin());
        }
        else
        {
            Gley.Localization.API.SetCurrentLanguage(Gley.Localization.SupportedLanguages.English);
            PlayGamesPlatform.Instance.Authenticate(OnSignInResult);

        }





    }
    private void OnSignInResult(SignInStatus status)
    {

        if (status == SignInStatus.Success)
        {
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            GameHandler.Instance.username = name;
            GameHandler.Instance.pID = id;
            ConnectToPlayGamesFireBase();
            // if (progressData.playerProfilePic == string.Empty) StartCoroutine(ProfilePhoto());
            // Status = "Authenticated. Hello, " + Social.localUser.userName + " (" + Social.localUser.id + ")";
        }
        else
        {
            //Status = "*** Failed to authenticate with " + signInStatus;
            PlayerPrefs.SetInt("SignedIn", 1);
            PlayerPrefs.Save();
            GameHandler.Instance.isLoggedIn = false;
            SignInAnonymous();
            onFailedAutoLogin?.Invoke();
        }


    }

    IEnumerator AutoLogin()
    {
        Task ProfileTask = auth.CurrentUser.ReloadAsync();
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
        if (User.Email != string.Empty)
        {
            GameHandler.Instance.pID = auth.CurrentUser.Email;
        }
        else
        {
            GameHandler.Instance.pID = auth.CurrentUser.UserId;
        }

        GameHandler.Instance.username = auth.CurrentUser.DisplayName;
        yield return StartCoroutine(LoadUserData());
        onSuccessLogin?.Invoke("Logged In");
        AddToInformation("Sign In Successful.");
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        dbf = FirebaseFirestore.DefaultInstance;

        //FB.Init(() =>
        //{
        //    if (FB.IsInitialized)
        //    {
        //        print("fb initialize");
        //        FB.ActivateApp();
        //    }

        //    else
        //        print("Couldn't initialize");
        //},
        //      isGameShown =>
        //      {

        //      });

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        loadMessaging();
        // LoadLevels();
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        //This checks if the user (your local user) is the same as the one from the auth
        if (auth.CurrentUser != User)
        {
            //this seems the same, but user could have been null before
            bool signedIn = User != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && User != null)
            {
                Debug.Log("Signed out " + User.UserId);
            }
            //this is important step, this user is the one you should be working with
            User = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + User.UserId);
            }
        }
    }

    //it does not directly log the user out but invalidates the auth
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    #endregion



    #region Permissions

    // Initialize permissions
    private List<bool> permissions = new List<bool> { false, false };
    private List<bool> permissionsAsked = new List<bool> { false, false };

    // Enumerator to ask for permissions
    private IEnumerator AskForPermissions()
    {
        // List of permission actions to check and request
        List<System.Action> actions = new List<System.Action>
        {
            () => CheckAndRequestPermission(0, Permission.Microphone)

        };

        // Iterate through the actions
        foreach (var action in actions)
        {
            action.Invoke();
            // Wait for the end of frame to give time for the permission request dialogs to appear
            yield return new WaitForEndOfFrame();
        }
    }

    // Method to check and request permissions
    private void CheckAndRequestPermission(int index, string permission)
    {
        permissions[index] = Permission.HasUserAuthorizedPermission(permission);
        if (!permissions[index] && !permissionsAsked[index])
        {
            Permission.RequestUserPermission(permission);
            permissionsAsked[index] = true;
        }
    }
    #endregion
    #region UserName
    public void UpdateUserName(string _username)
    {
        StartCoroutine(UpdateUsernameAuth(_username));
        StartCoroutine(UpdateUsernameDatabase(_username));
    }
    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        Task ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }


    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }
    #endregion
    #region UserData
    private IEnumerator UpdateProgressData(string itemName, int val)
    {

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child(itemName).SetValueAsync(val);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }
    private IEnumerator UpdateProgressData(string itemName, bool val)
    {

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child(itemName).SetValueAsync(val);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }
    public void UpdatePayPalData(string _transactionId, string _cash)
    {
        StartCoroutine(UpdatePayPalDataEnum(_transactionId, _cash));
    }
    private IEnumerator UpdatePayPalDataEnum(string _transactionId, string _cash)
    {

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("PayPal").Child(_transactionId).SetValueAsync(_cash);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }

    private IEnumerator UpdateProgressData(string itemName, string val)
    {

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child(itemName).SetValueAsync(val);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }
    public void SaveProgressData()
    {
        StartCoroutine(UpdateProgressData("levelsUnlocked", progressData.levelCompleted));
        StartCoroutine(UpdateProgressData("cash", progressData.tickets));
        StartCoroutine(UpdateProgressData("keys", progressData.keys));
        StartCoroutine(UpdateProgressData("gems", progressData.gems));
        StartCoroutine(UpdateProgressData("noAds", progressData.noAds));
        StartCoroutine(UpdateProgressData("level", progressData.level));
        StartCoroutine(UpdateProgressData("currentLevelPoints", progressData.currentLevelPoints));
        StartCoroutine(UpdateProgressData("requiredLevelPoints", progressData.requiredLevelPoints));
        StartCoroutine(UpdateProgressData("currentRewardGiftPoints", progressData.currentRewardGiftPoints));
        StartCoroutine(UpdateProgressData("requiredRewardGiftPoints", progressData.requiredRewardGiftPoints));
        StartCoroutine(UpdateProgressData("commonButterfly", progressData.commonButterfly));
        StartCoroutine(UpdateProgressData("goldenButterfly", progressData.goldenButterfly));
        StartCoroutine(UpdateProgressData("legendaryButterfly", progressData.legendaryButterfly));
        StartCoroutine(UpdateProgressData("hammer", progressData.hammer));
        StartCoroutine(UpdateProgressData("bulb", progressData.bulb));
        StartCoroutine(UpdateProgressData("fireCracker", progressData.fireCracker));
        StartCoroutine(UpdateProgressData("Country", progressData.userCountry));
        StartCoroutine(UpdateProgressData("Gender", progressData.userGender));
        StartCoroutine(UpdateProgressData("Email", progressData.userEmail));

        StartCoroutine(UpdateProgressData("spinWheelLastDate", progressData.spinWheelLastDate));
        StartCoroutine(UpdateProgressData("spinCount", progressData.spinWheelCount));
        StartCoroutine(UpdateProgressData("freeSpin", progressData.freeSpinWheel));
        StartCoroutine(UpdateProgressData("invite1", progressData.invite1RewardGot));
        StartCoroutine(UpdateProgressData("invite2", progressData.invite2RewardGot));
        StartCoroutine(UpdateProgressData("invite3", progressData.invite3RewardGot));
        StartCoroutine(UpdateProgressData("invites", progressData.invites));
        StartCoroutine(UpdateProgressData("wordsFound", progressData.wordsFound));
        StartCoroutine(UpdateProgressData("failedTournament", progressData.failedTournament));

        StartCoroutine(UpdateProgressData("wonTournament", progressData.wonTournament));

        StartCoroutine(UpdateProgressData("playedTournament", progressData.playedTournament));

        if (progressData.playerProfilePic != string.Empty) StartCoroutine(UpdateProgressData("profilePic", progressData.playerProfilePic));

    }
    public void LoadProgressData()
    {

        StartCoroutine(LoadUserData());


    }
    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
        Task<DataSnapshot> DBTask2 = DBreference.Child("OfficialData").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet

        }
        else
        {


            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Child("levelsUnlocked").Exists) progressData.levelCompleted = int.Parse(snapshot.Child("levelsUnlocked").Value.ToString());
            if (snapshot.Child("cash").Exists) progressData.tickets = int.Parse(snapshot.Child("cash").Value.ToString());
            if (snapshot.Child("keys").Exists) progressData.keys = int.Parse(snapshot.Child("keys").Value.ToString());
            if (snapshot.Child("gems").Exists) progressData.gems = int.Parse(snapshot.Child("gems").Value.ToString());
            if (snapshot.Child("noAds").Exists) progressData.noAds = (bool)snapshot.Child("noAds").Value;
            if (snapshot.Child("level").Exists) progressData.level = int.Parse(snapshot.Child("level").Value.ToString());
            if (snapshot.Child("currentLevelPoints").Exists) progressData.currentLevelPoints = int.Parse(snapshot.Child("currentLevelPoints").Value.ToString());
            if (snapshot.Child("requiredLevelPoints").Exists) progressData.requiredLevelPoints = int.Parse(snapshot.Child("requiredLevelPoints").Value.ToString());
            if (snapshot.Child("currentRewardGiftPoints").Exists) progressData.currentRewardGiftPoints = int.Parse(snapshot.Child("currentRewardGiftPoints").Value.ToString());
            if (snapshot.Child("requiredRewardGiftPoints").Exists) progressData.requiredRewardGiftPoints = int.Parse(snapshot.Child("requiredRewardGiftPoints").Value.ToString());
            if (snapshot.Child("commonButterfly").Exists) progressData.commonButterfly = int.Parse(snapshot.Child("commonButterfly").Value.ToString());
            if (snapshot.Child("goldenButterfly").Exists) progressData.goldenButterfly = int.Parse(snapshot.Child("goldenButterfly").Value.ToString());
            if (snapshot.Child("legendaryButterfly").Exists) progressData.legendaryButterfly = int.Parse(snapshot.Child("legendaryButterfly").Value.ToString());
            if (snapshot.Child("hammer").Exists) progressData.hammer = int.Parse(snapshot.Child("hammer").Value.ToString());
            if (snapshot.Child("bulb").Exists) progressData.bulb = int.Parse(snapshot.Child("bulb").Value.ToString());
            if (snapshot.Child("fireCracker").Exists) progressData.fireCracker = int.Parse(snapshot.Child("fireCracker").Value.ToString());
            if (snapshot.Child("spinWheelLastDate").Exists) progressData.spinWheelLastDate = snapshot.Child("spinWheelLastDate").Value.ToString();
            if (snapshot.Child("spinCount").Exists) progressData.spinWheelCount = int.Parse(snapshot.Child("spinCount").Value.ToString());
            if (snapshot.Child("freeSpin").Exists) progressData.freeSpinWheel = (bool)snapshot.Child("freeSpin").Value;
            if (snapshot.Child("invite1").Exists) progressData.invite1RewardGot = (bool)snapshot.Child("invite1").Value;
            if (snapshot.Child("invite2").Exists) progressData.invite2RewardGot = (bool)snapshot.Child("invite2").Value;
            if (snapshot.Child("invite3").Exists) progressData.invite3RewardGot = (bool)snapshot.Child("invite3").Value;
            if (snapshot.Child("invites").Exists) progressData.invites = int.Parse(snapshot.Child("invites").Value.ToString());
            if (snapshot.Child("wordsFound").Exists) progressData.wordsFound = snapshot.Child("wordsFound").Value.ToString();
            if (snapshot.Child("wonTournament").Exists) progressData.wonTournament = int.Parse(snapshot.Child("wonTournament").Value.ToString());
            if (snapshot.Child("failedTournament").Exists) progressData.failedTournament = int.Parse(snapshot.Child("failedTournament").Value.ToString());

            if (snapshot.Child("playedTournament").Exists) progressData.playedTournament = int.Parse(snapshot.Child("playedTournament").Value.ToString());


            if (snapshot.Child("Gender").Exists) progressData.userGender = snapshot.Child("Gender").Value.ToString();
            if (snapshot.Child("Email").Exists) progressData.userEmail = snapshot.Child("Email").Value.ToString();


            if (snapshot.Child("profilePic").Exists) progressData.playerProfilePic = (string)snapshot.Child("profilePic").Value;

        }
        //Get the currently logged in user data



        if (DBTask2.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }
        else if (DBTask2.Result.Value == null)
        {
            //No data exists yet

        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask2.Result;

            if (snapshot.Child("tournamentUnlockLevel").Exists) progressData.tournamentUnlockLevel = int.Parse(snapshot.Child("tournamentUnlockLevel").Value.ToString());
            if (snapshot.Child("paypalWithDrawAmount").Exists) progressData.paypalWithDrawAmount = int.Parse(snapshot.Child("paypalWithDrawAmount").Value.ToString());

            if (snapshot.Child("avatarPic").Exists) avatarPicTexture = ConvertTextureStringToSprite((string)snapshot.Child("avatarPic").Value);


        }
    }
    public void LoadLevels()
    {

        StartCoroutine(LoadLevelsEnum());

    }
    private IEnumerator LoadLevelsEnum()
    {

        //Get the currently logged in user data
        Task<DataSnapshot> DBTask2 = DBreference.Child("OfficialData").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);
        if (DBTask2.Exception != null)
        {
            GameHandler.Instance.LoadDefaultGameData();
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }
        else if (DBTask2.Result.Value == null)
        {
            //No data exists yet
            GameHandler.Instance.LoadDefaultGameData();
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask2.Result;
            yield return new WaitUntil(() => FetchWords.isNewJsonReceived);
            if (GameHandler.Instance.countryName != "None")
            {
                Debug.LogError("URL link is: _________________________ " + snapshot.Child("levelURL").Value.ToString());
                if (snapshot.Child(GameHandler.Instance.countryName).Exists)
                {
                    GameHandler.Instance.CheckJson(snapshot.Child(GameHandler.Instance.countryName).Value.ToString(), FetchWords.newJsonString);
                }
                else if (snapshot.Child("levelURL").Exists)
                {
                    GameHandler.Instance.CheckJson(snapshot.Child("levelURL").Value.ToString(), FetchWords.newJsonString);
                }
                else
                {
                    GameHandler.Instance.LoadDefaultGameData();
                }
            }
            else if (snapshot.Child("levelURL").Exists)
            {
                GameHandler.Instance.CheckJson(snapshot.Child("levelURL").Value.ToString(), FetchWords.newJsonString);
            }
            else
            {
                GameHandler.Instance.LoadDefaultGameData();
            }



        }

    }
    public void updateT(string val)
    {
        StartCoroutine(UpdateTextTure(val));
    }
    private IEnumerator UpdateTextTure(string val)
    {

        Task DBTask = DBreference.Child("OfficialData").Child("avatarPic").SetValueAsync(val);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }
    public void UpdateTournamentProgress(float tTimeFunc, int tLevelFunc)
    {
        GameHandler.Instance.tournamentCurrentPlayerData.time += tTimeFunc;
        GameHandler.Instance.tournamentCurrentPlayerData.level += tLevelFunc;


        DocumentReference countRef = dbf.Collection("Tournaments").Document(GameHandler.Instance.tournamentCurrentPlayerData.tournamentName).Collection("Detail").Document("Players");
        Dictionary<string, object> newPlayer = new Dictionary<string, object>();
        newPlayer.Add(User.UserId, JsonConvert.SerializeObject(GameHandler.Instance.tournamentCurrentPlayerData));
        countRef.UpdateAsync(newPlayer).ContinueWithOnMainThread(task =>
        {
        });

    }
    public void UpdateTournamentProgress(string tName, TournamentPlayerData pData)
    {



        DocumentReference countRef = dbf.Collection("Tournaments").Document(tName).Collection("Detail").Document("Players");
        Dictionary<string, object> newPlayer = new Dictionary<string, object>();
        newPlayer.Add(User.UserId, JsonConvert.SerializeObject(pData));
        countRef.UpdateAsync(newPlayer).ContinueWithOnMainThread(task =>
        {
        });

    }
    public void RewardReferer(string userId)
    {
        StartCoroutine(RewardRefererEnum(userId));
    }
    private IEnumerator RewardRefererEnum(string userId)
    {
        Task<DataSnapshot> DBTask1 = DBreference.Child("users").Child(userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);
        if (DBTask1.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask1.Exception}");
        }
        else if (DBTask1.Result.Value == null)
        {
            //No data exists yet

        }
        else
        {


            //Data has been retrieved
            DataSnapshot snapshot = DBTask1.Result;

            if (snapshot.Child("invites").Exists)
            {
                uint iVal = (uint)int.Parse(snapshot.Child("invites").Value.ToString());
                Task DBTask = DBreference.Child("users").Child(userId).Child("invites").SetValueAsync(iVal + 1);

                yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

                if (DBTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                }
                else
                {
                    //Data is now updated
                }
            }


        }


    }
    #endregion
    #region Tournament History
    public void UpdateTournamentHistoryData(string tournamentName, string data)
    {
        StartCoroutine(UpdateTournamentHistoryDataEnum(tournamentName, data));
    }
    private IEnumerator UpdateTournamentHistoryDataEnum(string tournamentName, string data)
    {

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("TournamentHistory").Child(tournamentName).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data is now updated
        }
    }
    public void LoadTournamentHistory()
    {
        StartCoroutine(LoadTournamentHistoryEnum());
    }
    private IEnumerator LoadTournamentHistoryEnum()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;




            if (snapshot.Child("TournamentHistory").Exists)
            {
                foreach (DataSnapshot childSnapshot2 in snapshot.Child("TournamentHistory").Children)
                {
                    onGetTournamentHistory?.Invoke(childSnapshot2.Key, JsonConvert.DeserializeObject<TournamentHistory>(childSnapshot2.Value.ToString()));
                }



            }




        }
    }
    #endregion
    #region LeaderBoard
    public void LoadLeaderBoard()
    {
        StartCoroutine(LoadScoreboardData());
    }
    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("level").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;


            int iRange = 0;


            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                if (childSnapshot.Child("username").Exists)
                {
                    string userN = childSnapshot.Child("username").Value.ToString();
                    if (userN != string.Empty && userN.Length > 0 && userN != ""/*&&userN!="Guest"*/)
                    {

                        string username = childSnapshot.Child("username").Value.ToString();
                        int level = int.Parse(childSnapshot.Child("level").Value.ToString());
                        int cash = int.Parse(childSnapshot.Child("cash").Value.ToString());
                        int keys = int.Parse(childSnapshot.Child("keys").Value.ToString());
                        onGetLeaderBoard?.Invoke(username, level, iRange + 1, cash);
                        iRange++;
                        if (iRange >= LeaderBoardValuesCount)
                        {
                            break;
                        }
                    }

                }

            }


        }
    }
    public void LoadHighestWinLeaderBoard()
    {
        StartCoroutine(LoadHighestWinLeaderBoardEnum());
    }
    private IEnumerator LoadHighestWinLeaderBoardEnum()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("wonTournament").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;


            int iRange = 0;


            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                if (childSnapshot.Child("username").Exists)
                {
                    string userN = childSnapshot.Child("username").Value.ToString();
                    if (userN != string.Empty && userN.Length > 0 && userN != ""/*&&userN!="Guest"*/)
                    {

                        string username = childSnapshot.Child("username").Value.ToString();
                        int Wins = int.Parse(childSnapshot.Child("wonTournament").Value.ToString());
                        int cash = int.Parse(childSnapshot.Child("cash").Value.ToString());
                        if (Wins > 0)
                        {
                            onGetHighestLeaderBoard?.Invoke(username, Wins, iRange + 1, cash);
                            iRange++;
                            if (iRange >= LeaderBoardValuesCount)
                            {
                                break;
                            }
                        }
                       
                    }

                }

            }


        }
    }

    //public void LoadLeaderBoardTournament()
    //{
    //    StartCoroutine(LoadScoreboardDataTournament());
    //}

    //private IEnumerator LoadScoreboardDataTournament()
    //{
    //    //Get all the users data ordered by kills amount
    //    Task<DocumentSnapshot> DBTask = dbf.Collection("Tournaments").Document("Tournament1").Collection("Detail").Document("Players").GetSnapshotAsync();

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else
    //    {

    //        //Data has been retrieved
    //        DocumentSnapshot snapshot = DBTask.Result;

    //        Dictionary<string, object> dt = snapshot.ToDictionary();
    //        Dictionary<string, int> dtWithLevel = new Dictionary<string, int>();
    //        Dictionary<string, int> dtforPrize = new Dictionary<string, int>();

    //        foreach (var v in dt)
    //        {

    //            dtWithLevel.Add(JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()).playerName, JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()).level);
    //            dtforPrize.Add(JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()).playerID, JsonConvert.DeserializeObject<TournamentPlayerData>(v.Value.ToString()).level);
    //        }





    //        var sortedDict = dtWithLevel.OrderByDescending(pair => pair.Value);
    //        var sortedDictForPrize = dtforPrize.OrderByDescending(pair => pair.Value);

    //        List<KeyValuePair<string, int>> sortedList = sortedDict.ToList();
    //        List<KeyValuePair<string, int>> sortedListForPrize = sortedDictForPrize.ToList();



    //        int iRange = 0;

    //        bool tournamentLevelShow = MainMenuHandler.Instance.tournamentDetailContainers.First().Value.endDate < DateTime.Now;
    //        Debug.Log(MainMenuHandler.Instance.tournamentDetailContainers.First().Value.endDate + " " + DateTime.Now);
    //        //Loop through every users UID
    //        foreach (KeyValuePair<string, int> childSnapshot in sortedDict)
    //        {



    //            string username = childSnapshot.Key;
    //            int level = -1;
    //            if (tournamentLevelShow)
    //            {
    //                level = childSnapshot.Value;
    //            }

    //            onGetLeaderBoardTournament?.Invoke(username, level, iRange + 1);
    //            iRange++;
    //            if (iRange >= LeaderBoardValuesCount)
    //            {
    //                break;
    //            }




    //        }
    //        if (tournamentLevelShow && !GameHandler.Instance.tournamentPlayerData.prizeObtained)
    //        {
    //            int prizeRange = 0;
    //            foreach (KeyValuePair<string, int> childSnapshot in sortedDictForPrize)
    //            {

    //                if (childSnapshot.Key == User.UserId)
    //                {
    //                    if (prizeRange < GameHandler.Instance.TournamentPrizeDistributionCategory.Count)
    //                    {
    //                        switch (GameHandler.Instance.TournamentPrizeDistributionCategory[prizeRange].pType)
    //                        {
    //                            case "keys":
    //                                progressData.keys += (uint)GameHandler.Instance.TournamentPrizeDistributionCategory[prizeRange].pValue;
    //                                break;
    //                            case "gems":
    //                                progressData.gems += (uint)GameHandler.Instance.TournamentPrizeDistributionCategory[prizeRange].pValue;
    //                                break;
    //                            case "cash":
    //                                progressData.tickets += (uint)GameHandler.Instance.TournamentPrizeDistributionCategory[prizeRange].pValue;
    //                                break;
    //                            default:
    //                                progressData.gems += (uint)GameHandler.Instance.TournamentPrizeDistributionCategory[prizeRange].pValue;
    //                                break;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        progressData.tickets += (uint)MainMenuHandler.Instance.tournamentDetailContainers.First().Value.counter.prize;
    //                    }

    //                    GameHandler.Instance.tournamentPlayerData.prizeObtained = true;
    //                    UpdateTournamentProgress();
    //                    break;
    //                }




    //                prizeRange++;
    //                if (prizeRange >= GameHandler.Instance.tournamentPrizeDistributionCount)
    //                {
    //                    break;
    //                }




    //            }
    //        }

    //    }
    //}
    public void SendPayPalRequest(string _transactionId, string _cash, int cpaymant)
    {
        StartCoroutine(SendPayPalRequestEnum(_transactionId, _cash, cpaymant));
    }
    private IEnumerator SendPayPalRequestEnum(string _transactionId, string _cash, int cpaymant)
    {
        long PayPalRequestsCount = 0;
        long PayPalRequestsCount2 = 0;
        Task<DataSnapshot> DBTask2 = DBreference.Child("PayPalWithDrawRequests").GetValueAsync();
        Task<DataSnapshot> DBTask3 = DBreference.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);
        yield return new WaitUntil(predicate: () => DBTask3.IsCompleted);
        if (DBTask2.Exception == null)
        {
            if (DBTask2.Result.HasChildren)
            {
                PayPalRequestsCount = DBTask2.Result.ChildrenCount;
            }
        }


        if (DBTask3.Exception == null)
        {


            if (DBTask3.Result.HasChildren)
            {
                if (DBTask3.Result.Child("PayPal").Exists)
                {
                    PayPalRequestsCount2 = DBTask3.Result.Child("PayPal").ChildrenCount;
                }

            }


        }


        Task DBTask4 = DBreference.Child("PayPalWithDrawRequests").Child(User.UserId + PayPalRequestsCount.ToString() + PayPalRequestsCount2.ToString()).SetValueAsync(_transactionId + ":" + _cash);


        yield return new WaitUntil(predicate: () => DBTask4.IsCompleted);

        if (DBTask4.Exception != null)
        {

            MainMenuHandler.Instance.payPalAmountInputFieldResultDetail.text = ("error");
            MainMenuHandler.Instance.payoutInProgress = false;
        }
        else
        {
            MainMenuHandler.Instance.payPalAmountInputFieldResultDetail.text = ("transaction done!\n waiting for approval");
            progressData.tickets -= cpaymant;
            SaveProgressData();
            UpdatePayPalData(User.UserId + PayPalRequestsCount.ToString() + PayPalRequestsCount2.ToString(), _cash);
            MainMenuHandler.Instance.GetPayPalHistory(User.UserId + PayPalRequestsCount.ToString() + PayPalRequestsCount2.ToString(), _cash, false);
            //Application.OpenURL(data.Links[0].Href);
            // StartCoroutine(GetPayerID(data.Links[0].Href, accessToken));
            MainMenuHandler.Instance.payoutInProgress = false;
            MainMenuHandler.Instance.UpdateTxts();
        }
    }
    public void LoadPayPalHistory()
    {
        StartCoroutine(LoadPayPalHistoryEnum());
    }
    private IEnumerator LoadPayPalHistoryEnum()
    {
        //Get all the users data ordered by kills amount
        Task<DataSnapshot> DBTask = DBreference.Child("users").GetValueAsync();
        Task<DataSnapshot> DBTask2 = DBreference.Child("PayPalWithDrawRequests").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);
        Debug.Log("paypal loaded history");
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            DataSnapshot snapshotAllRequests = null;
            if (DBTask2.Exception == null)
            {
                snapshotAllRequests = DBTask2.Result;
                Debug.Log("paypal loaded history has requests");
            }

            int iRange = 0;
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                if (childSnapshot.Child("PayPal").Exists)
                {
                    foreach (DataSnapshot childSnapshot2 in childSnapshot.Child("PayPal").Children)
                    {
                        onGetPaypalHistory?.Invoke(childSnapshot2.Key, childSnapshot2.Value.ToString(), CheckPaypayRequestDone(childSnapshot2.Key, snapshotAllRequests));
                        iRange++;
                        if (iRange >= LeaderBoardValuesCount)
                        {
                            break;
                        }
                    }



                }

            }


        }
    }
    public bool CheckPaypayRequestDone(string requestId, DataSnapshot dt)
    {
        if (dt.HasChildren && dt != null)
        {
            if (dt.Child(requestId).Exists)
            {
                return false;
            }
            else { return true; }
        }
        else
        {
            return true;
        }
    }
    #endregion
    #region Play Games

    public void ConnectToPlayGamesFireBase()
    {
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            pgsAuthCode = code;
            Credential pgsCred = PlayGamesAuthProvider.GetCredential(pgsAuthCode);

            StartCoroutine(SignInWithPlayGamesServicesEnum(pgsCred));
        });
    }
    IEnumerator SignInWithPlayGamesServicesEnum(Credential credential)
    {

        Debug.Log("TaskCompleted Started login Enum");
        Task<FirebaseUser> LoginTask2 = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => LoginTask2.IsCompleted);
        if (LoginTask2.Exception != null)
        {

            Debug.Log(message: $"Failed to register task with {LoginTask2.Exception}");


            string message2 = "Register Failed!";
            PlayerPrefs.SetInt("SignedIn", 0);
            PlayerPrefs.Save();
            GameHandler.Instance.isLoggedIn = false;
            SignInAnonymous();
            onFailedLogin?.Invoke(message2);
        }
        else
        {
            User = LoginTask2.Result;

            yield return StartCoroutine(UpdateUsernameAuth(GameHandler.Instance.username));
            yield return StartCoroutine(UpdateUsernameDatabase(GameHandler.Instance.username));
            yield return StartCoroutine(LoadUserData());
            PlayerPrefs.SetInt("SignedIn", 2);
            PlayerPrefs.Save();
            GameHandler.Instance.isLoggedIn = true;
            onSuccessLogin?.Invoke("Logged In");
            SaveProgressData();

            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
        }
    }

    #endregion
    #region google SignIn






    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }
    public void SignOutFromGoogle()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }





    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {

            GameHandler.Instance.username = task.Result.DisplayName;

            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);
            if (progressData.playerProfilePic == string.Empty) StartCoroutine(ProfilePhoto(task.Result.ImageUrl));
            StartCoroutine(LinkInWithGoogleOnFirebase(task.Result.IdToken));
            //SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private IEnumerator SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        Task<FirebaseUser> LoginTask = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        if (LoginTask.Exception != null)
        {

            Debug.Log(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }

            onFailedLogin?.Invoke(message + " " + errorCode.ToString());
        }
        else
        {
            User = LoginTask.Result;
            GameHandler.Instance.pID = User.Email;



            UpdateUserName(GameHandler.Instance.username);


            yield return StartCoroutine(LoadUserData());

            PlayerPrefs.SetInt("SignedIn", 1);
            PlayerPrefs.Save();
            onSuccessLogin?.Invoke("Logged In");
            AddToInformation("Sign In Successful.");

            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
        }

    }
    private IEnumerator LinkInWithGoogleOnFirebase(string idToken)
    {
        Credential credential =
       GoogleAuthProvider.GetCredential(idToken, null);


        Task<AuthResult> LoginTask = auth.CurrentUser.LinkWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        Debug.Log("TaskCompleted");
        if (LoginTask.Exception != null)
        {
            Debug.Log("TaskCompleted with Error");
            // Debug.Log(message: $"Failed to register task with {LoginTask.Exception}");


            FirebaseManager.Instance.OnFailedGoogleLink(credential);

        }
        else
        {
            User = LoginTask.Result.User;

            GameHandler.Instance.pID = User.Email;
            yield return StartCoroutine(UpdateUsernameAuth(GameHandler.Instance.username));
            yield return StartCoroutine(UpdateUsernameDatabase(GameHandler.Instance.username));

            PlayerPrefs.SetInt("SignedIn", 1);
            PlayerPrefs.Save();
            onSuccessLogin?.Invoke("Logged In");
            SaveProgressData();
            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
        }


    }
    public void OnFailedGoogleLink(Credential credential)
    {
        Debug.Log("TaskCompleted Started login");
        StartCoroutine(OnFailedGoogleLinkEnum(credential));
    }
    IEnumerator OnFailedGoogleLinkEnum(Credential credential)
    {

        Debug.Log("TaskCompleted Started login Enum");
        Task<FirebaseUser> LoginTask2 = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => LoginTask2.IsCompleted);
        if (LoginTask2.Exception != null)
        {

            Debug.Log(message: $"Failed to register task with {LoginTask2.Exception}");


            string message2 = "Register Failed!";
            onFailedLogin?.Invoke(message2);
        }
        else
        {
            User = LoginTask2.Result;
            GameHandler.Instance.pID = User.Email;



            yield return StartCoroutine(UpdateUsernameAuth(GameHandler.Instance.username));
            yield return StartCoroutine(UpdateUsernameDatabase(GameHandler.Instance.username));


            yield return StartCoroutine(LoadUserData());

            PlayerPrefs.SetInt("SignedIn", 1);
            PlayerPrefs.Save();
            onSuccessLogin?.Invoke("Logged In");
            AddToInformation("Sign In Successful.");

            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
        }
    }
    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void AddToInformation(string str)
    { //infoText.text += "\n" + str; 
      // Debug.Log(str);
    }
    #endregion

    //#region FacebookSignIN



    //void DealWithFbMenus(bool isLoggedIn)
    //{
    //    if (isLoggedIn)
    //    {
    //        FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
    //        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
    //    }
    //    else
    //    {
    //        print("Not logged in");
    //    }
    //}
    //void DisplayUsername(IResult result)
    //{
    //    if (result.Error == null)
    //    {
    //        string name = "" + result.ResultDictionary["first_name"];
    //        //if (FB_userName != null) FB_userName.text = name;

    //        Debug.Log("" + name);
    //    }
    //    else
    //    {
    //        Debug.Log(result.Error);
    //    }
    //}
    //void DisplayProfilePic(IGraphResult result)
    //{
    //    if (result.Texture != null)
    //    {
    //        Debug.Log("Profile Pic");
    //        progressData.playerProfilePic = ConvertTextureToString(result.Texture);
    //        // profilePicTexture = result.Texture;
    //        //progressData.playerProfilePic = ConvertTextureToString(result.Texture);
    //        // if (FB_profilePic != null) FB_profilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
    //        /*JSONObject json = new JSONObject(result.RawResult);

    //        StartCoroutine(DownloadTexture(json["picture"]["data"]["url"].str, profile_texture));*/
    //    }
    //    else
    //    {
    //        Debug.Log(result.Error);
    //    }
    //}

    //public void Facebook_LogIn()
    //{
    //    List<string> permissions = new List<string>() { "public_profile", "email" };
    //    FB.LogInWithReadPermissions(permissions, AuthCallBack);

    //}
    //void AuthCallBack(IResult result)
    //{
    //    if (FB.IsLoggedIn)
    //    {
    //        string name = "" + result.ResultDictionary["first_name"];
    //        if (name != string.Empty) GameHandler.Instance.username = name;
    //        var aToken = AccessToken.CurrentAccessToken;

    //        LinkInWithFacebookOnFirebase(aToken.TokenString);
    //        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
    //        //SignInWithFacebookOnFirebase(aToken.TokenString);



    //    }
    //    else
    //    {
    //        print("Failed to log in");
    //    }

    //}

    //IEnumerator SignInWithFacebookOnFirebase(string ac)
    //{
    //    Credential credential = FacebookAuthProvider.GetCredential(ac);



    //    Task<FirebaseUser> LoginTask = auth.SignInWithCredentialAsync(credential);

    //    yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

    //    if (LoginTask.Exception != null)
    //    {

    //        Debug.Log(message: $"Failed to register task with {LoginTask.Exception}");
    //        FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
    //        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

    //        string message = "Register Failed!";
    //        switch (errorCode)
    //        {
    //            case AuthError.MissingEmail:
    //                message = "Missing Email";
    //                break;
    //            case AuthError.MissingPassword:
    //                message = "Missing Password";
    //                break;
    //            case AuthError.WeakPassword:
    //                message = "Weak Password";
    //                break;
    //            case AuthError.EmailAlreadyInUse:
    //                message = "Email Already In Use";
    //                break;
    //        }

    //        onFailedLogin?.Invoke(message + " " + errorCode.ToString());
    //    }
    //    else
    //    {
    //        User = LoginTask.Result;


    //        GameHandler.Instance.pID = User.Email;


    //        UpdateUserName(GameHandler.Instance.username);





    //        yield return StartCoroutine(LoadUserData());

    //        PlayerPrefs.SetInt("SignedIn", 1);
    //        PlayerPrefs.Save();
    //        onSuccessLogin?.Invoke("Logged In");
    //        AddToInformation("Sign In Successful.");

    //    }


    //}

    //private IEnumerator LinkInWithFacebookOnFirebase(string idToken)
    //{
    //    Credential credential =
    //    FacebookAuthProvider.GetCredential(idToken);


    //    Task<AuthResult> LoginTask = auth.CurrentUser.LinkWithCredentialAsync(credential);

    //    yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
    //    Debug.Log("TaskCompleted");
    //    if (LoginTask.Exception != null)
    //    {
    //        Debug.Log("TaskCompleted with Error");
    //        // Debug.Log(message: $"Failed to register task with {LoginTask.Exception}");


    //        FirebaseManager.Instance.OnFailedFacebookLink(credential);

    //    }
    //    else
    //    {
    //        User = LoginTask.Result.User;

    //        GameHandler.Instance.pID = User.Email;
    //        yield return StartCoroutine(UpdateUsernameAuth(GameHandler.Instance.username));
    //        yield return StartCoroutine(UpdateUsernameDatabase(GameHandler.Instance.username));

    //        PlayerPrefs.SetInt("SignedIn", 1);
    //        PlayerPrefs.Save();
    //        onSuccessLogin?.Invoke("Logged In");
    //        SaveProgressData();
    // dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
    // dataReference.ValueChanged += HandleValueChanged;
    //    }


    //}
    //public void OnFailedFacebookLink(Credential credential)
    //{
    //    Debug.Log("TaskCompleted Started login");
    //    StartCoroutine(OnFailedFacebookLinkEnum(credential));
    //}
    //IEnumerator OnFailedFacebookLinkEnum(Credential credential)
    //{

    //    Debug.Log("TaskCompleted Started login Enum");
    //    Task<FirebaseUser> LoginTask2 = auth.SignInWithCredentialAsync(credential);

    //    yield return new WaitUntil(predicate: () => LoginTask2.IsCompleted);
    //    if (LoginTask2.Exception != null)
    //    {

    //        Debug.Log(message: $"Failed to register task with {LoginTask2.Exception}");


    //        string message2 = "Register Failed!";
    //        onFailedLogin?.Invoke(message2);
    //    }
    //    else
    //    {
    //        User = LoginTask2.Result;
    //        GameHandler.Instance.pID = User.Email;



    //        yield return StartCoroutine(UpdateUsernameAuth(GameHandler.Instance.username));
    //        yield return StartCoroutine(UpdateUsernameDatabase(GameHandler.Instance.username));


    //        yield return StartCoroutine(LoadUserData());

    //        PlayerPrefs.SetInt("SignedIn", 1);
    //        PlayerPrefs.Save();
    //        onSuccessLogin?.Invoke("Logged In");
    //        AddToInformation("Sign In Successful.");

    // dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
    // dataReference.ValueChanged += HandleValueChanged;
    //    }
    //}
    //#endregion
    #region Anonymous
    public void SignInAnonymous()
    {
        StartCoroutine(StartSignInAnonymous());
    }
    IEnumerator StartSignInAnonymous()
    {


        Task<AuthResult> RegisterTask = auth.SignInAnonymouslyAsync();
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);


        if (RegisterTask.IsCanceled)
        {
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
            onFailedAnonymous?.Invoke("SignInAnonymouslyAsync was canceled.");
            yield break;
        }
        if (RegisterTask.IsFaulted)
        {
            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + RegisterTask.Exception);
            onFailedAnonymous?.Invoke("SignInAnonymouslyAsync encountered an error: " + RegisterTask.Exception);
            yield break;
        }

        AuthResult result = RegisterTask.Result;

        if (RegisterTask.Exception != null)
        {
            //If there are errors handle them
            Debug.Log(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            //warningRegisterText.text = message;
            onFailedAnonymous?.Invoke(message);
        }
        else
        {
            //User has now been created
            //Now get the result
            User = RegisterTask.Result.User;


            string usern = "Guest" + UnityEngine.Random.Range(1000, 900000);
            yield return StartCoroutine(LoadUserData());
            yield return StartCoroutine(UpdateUsernameDatabase(usern));
            yield return StartCoroutine(UpdateUsernameAuth(usern));
            GameHandler.Instance.username = usern;
            GameHandler.Instance.pID = User.UserId;
            SaveProgressData();
            onSuccessAnonymous?.Invoke("SignedIn");

            dataReference = FirebaseDatabase.DefaultInstance.GetReference("/users/" + User.UserId);
            dataReference.ValueChanged += HandleValueChanged;
            dataReference2 = FirebaseDatabase.DefaultInstance.GetReference("/OfficialData");
            dataReference2.ValueChanged += HandleValueChanged2;
        }
    }
    #endregion

    #region Save And Load Pic From String
    //Convert a texture to a string and then store it in Json
    public string ConvertTextureToString(Texture2D tex)
    {
        string TextureArray = Convert.ToBase64String(tex.EncodeToPNG());

        return TextureArray;
    }
    //Convert a json string to Sprite
    public Sprite ConvertTextureStringToSprite(string texString)
    {

        byte[] b64_bytes = Convert.FromBase64String(texString);
        Texture2D tex = new Texture2D(212, 212);
        tex.LoadImage(b64_bytes);
        tex.Apply();
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.zero);
        return sprite;
    }
    IEnumerator ProfilePhoto(Uri fbu)
    {


        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fbu))
        {
            yield return uwr.SendWebRequest();


            Texture2D profilePicTexture = DownloadHandlerTexture.GetContent(uwr);
            if (profilePicTexture != null)
            {
                progressData.playerProfilePic = ConvertTextureToString(profilePicTexture);
            }

        }
    }
    #endregion

    #region Messaging
    public void loadMessaging()
    {

        FirebaseMessaging.TokenReceived += OnTokenRecieved;
        FirebaseMessaging.MessageReceived += OnMessageRecieved;
        FirebaseMessaging.SubscribeAsync("/topics/notice");
        FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("RequestPermissionAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"RequestPermissionAsync encountered an error: {task.Exception}");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("Notification permission granted.");
            }
        });
    }

    private void OnMessageRecieved(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log(e.Message);
    }

    private void OnTokenRecieved(object sender, TokenReceivedEventArgs e)
    {
        Debug.Log(e.Token);
    }
    #endregion
    #region Load Tournament Level
    public void LoadTournamentLevels()
    {
        dbf.Collection("Tournaments").Listen(snapshot =>
        {
            GameHandler.Instance.tournamentLevels = new List<TournamentLevels>();

            // Convert IEnumerable<DocumentSnapshot> to List<DocumentSnapshot>
            var documentsList = snapshot.Documents.ToList();

            // Start processing the first item
            ProcessTournamentItems(documentsList, 0);
        });
    }

    private void ProcessTournamentItems(IList<DocumentSnapshot> documents, int index)
    {
        if (index >= documents.Count)
        {
            // All documents processed
            return;
        }

        var item = documents[index];
        item.Reference.Collection("Detail").Document("PrimaryDetail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                var data = snapshot.ToDictionary();
                TournamentLevels tLevels = new TournamentLevels();
                GameHandler.Instance.tournamentLevels.Add(tLevels);

                Debug.LogError("Sajjad tournament levels 6: " + item.Id);
                PlayerPrefs.SetString("tableName", item.Id);
                fetchTournamentLevelsObj.SetActive(true);
                string url = data["levelURL"].ToString();

                Debug.LogError("Sadiq---------------------> PrimaryDetail:|");

                // Process the current URL and then move to the next item
                StartCoroutine(ProcessJsonForTournamentLevels(url, GameHandler.Instance.tournamentLevels.IndexOf(tLevels), documents, index + 1, item.Id));
            }
        });
    }

    private IEnumerator ProcessJsonForTournamentLevels(string url, int tInd, IList<DocumentSnapshot> documents, int nextIndex, string tournamentId)
    {
        // Load words and create the JSON file using FetchWordsTournament
        FetchWordsTournament.instance.LoadWordsFromExternalUrl(url, tournamentId);

        // Wait until the JSON file is created
        yield return new WaitUntil(() => FetchWordsTournament.isNewJsonReceived);

        // Now load the levels from the newly created JSON file
        string filePath = Path.Combine(Application.persistentDataPath, $"{tournamentId}_words.json");
        string json_data = File.ReadAllText(filePath);

        if (!string.IsNullOrEmpty(json_data))
        {
            JArray jlevels = JArray.Parse(json_data);
            Debug.LogError("Level: " + jlevels);
            foreach (JObject level in jlevels)
            {
                int levelNumber = (int)level["level"];
                JArray words = (JArray)level["words"];
                WordHintDictionary newLevel = new WordHintDictionary();
                GameHandler.AssignLevel(ref newLevel);
                newLevel.DictionaryName = "Level" + levelNumber.ToString();
                newLevel.WordHintPairs = new WordHintPair[words.Count];
                int j = 0;

                foreach (string word in words)
                {
                    WordHintPair wp = new WordHintPair();
                    GameHandler.AssignWord(ref wp);
                    newLevel.WordHintPairs[j] = wp;
                    newLevel.WordHintPairs[j].Word = word;
                    newLevel.WordHintPairs[j].Hint = "";
                    j++;
                }
                GameHandler.Instance.tournamentLevels[tInd].levels.Add(newLevel);
                Debug.LogError("check level ads or not" + GameHandler.Instance.tournamentLevels.Count);
            }

            Debug.Log("Tournament levels loaded from JSON file.");
        }
        else
        {
            Debug.LogError("Failed to load tournament levels: JSON data is empty.");
        }

        // Process the next tournament item
        ProcessTournamentItems(documents, nextIndex);
    }
    #endregion

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Handle the data you received
        //    Debug.Log(args.Snapshot.GetRawJsonValue());
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot.Child("levelsUnlocked").Exists) progressData.levelCompleted = int.Parse(snapshot.Child("levelsUnlocked").Value.ToString());
        if (snapshot.Child("cash").Exists) progressData.tickets = int.Parse(snapshot.Child("cash").Value.ToString());
        if (snapshot.Child("keys").Exists) progressData.keys = int.Parse(snapshot.Child("keys").Value.ToString());
        if (snapshot.Child("gems").Exists) progressData.gems = int.Parse(snapshot.Child("gems").Value.ToString());
        if (snapshot.Child("noAds").Exists) progressData.noAds = (bool)snapshot.Child("noAds").Value;
        if (snapshot.Child("level").Exists) progressData.level = int.Parse(snapshot.Child("level").Value.ToString());
        if (snapshot.Child("currentLevelPoints").Exists) progressData.currentLevelPoints = int.Parse(snapshot.Child("currentLevelPoints").Value.ToString());
        if (snapshot.Child("requiredLevelPoints").Exists) progressData.requiredLevelPoints = int.Parse(snapshot.Child("requiredLevelPoints").Value.ToString());
        if (snapshot.Child("currentRewardGiftPoints").Exists) progressData.currentRewardGiftPoints = int.Parse(snapshot.Child("currentRewardGiftPoints").Value.ToString());
        if (snapshot.Child("requiredRewardGiftPoints").Exists) progressData.requiredRewardGiftPoints = int.Parse(snapshot.Child("requiredRewardGiftPoints").Value.ToString());
        if (snapshot.Child("commonButterfly").Exists) progressData.commonButterfly = int.Parse(snapshot.Child("commonButterfly").Value.ToString());
        if (snapshot.Child("goldenButterfly").Exists) progressData.goldenButterfly = int.Parse(snapshot.Child("goldenButterfly").Value.ToString());
        if (snapshot.Child("legendaryButterfly").Exists) progressData.legendaryButterfly = int.Parse(snapshot.Child("legendaryButterfly").Value.ToString());
        if (snapshot.Child("hammer").Exists) progressData.hammer = int.Parse(snapshot.Child("hammer").Value.ToString());
        if (snapshot.Child("bulb").Exists) progressData.bulb = int.Parse(snapshot.Child("bulb").Value.ToString());
        if (snapshot.Child("fireCracker").Exists) progressData.fireCracker = int.Parse(snapshot.Child("fireCracker").Value.ToString());
        if (snapshot.Child("spinWheelLastDate").Exists) progressData.spinWheelLastDate = snapshot.Child("spinWheelLastDate").Value.ToString();
        if (snapshot.Child("spinCount").Exists) progressData.spinWheelCount = int.Parse(snapshot.Child("spinCount").Value.ToString());
        if (snapshot.Child("freeSpin").Exists) progressData.freeSpinWheel = (bool)snapshot.Child("freeSpin").Value;
        if (snapshot.Child("invite1").Exists) progressData.invite1RewardGot = (bool)snapshot.Child("invite1").Value;
        if (snapshot.Child("invite2").Exists) progressData.invite2RewardGot = (bool)snapshot.Child("invite2").Value;
        if (snapshot.Child("invite3").Exists) progressData.invite3RewardGot = (bool)snapshot.Child("invite3").Value;
        if (snapshot.Child("invites").Exists) progressData.invites = int.Parse(snapshot.Child("invites").Value.ToString());
        if (snapshot.Child("wordsFound").Exists) progressData.wordsFound = snapshot.Child("wordsFound").Value.ToString();
        if (snapshot.Child("wonTournament").Exists) progressData.wonTournament = int.Parse(snapshot.Child("wonTournament").Value.ToString());

        if (snapshot.Child("playedTournament").Exists) progressData.playedTournament = int.Parse(snapshot.Child("playedTournament").Value.ToString());


        if (snapshot.Child("profilePic").Exists) progressData.playerProfilePic = (string)snapshot.Child("profilePic").Value;
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            MainMenuHandler.Instance.UpdateTxts();
            MainMenuHandler.Instance.LoadTournamentTypes();
        }
    }

    public void DeleteDocument(string documentId)
    {
        DocumentReference docRef = dbf.Collection("Tournaments").Document(documentId);
        docRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            Debug.LogError("Document id: " + documentId);
            if (task.IsCompleted)
            {
                Debug.Log("Document successfully deleted.");
            }
            else
            {
                Debug.LogError("Error deleting document: " + task.Exception);
            }
        });
    }
    void HandleValueChanged2(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Handle the data you received
        //    Debug.Log(args.Snapshot.GetRawJsonValue());
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot.Child("tournamentUnlockLevel").Exists) progressData.tournamentUnlockLevel = int.Parse(snapshot.Child("tournamentUnlockLevel").Value.ToString());
        if (snapshot.Child("paypalWithDrawAmount").Exists) progressData.paypalWithDrawAmount = int.Parse(snapshot.Child("paypalWithDrawAmount").Value.ToString());


        if (SceneManager.GetActiveScene().name == "Menu")
        {
            MainMenuHandler.Instance.SetTournamentLocks();

        }
    }
}
