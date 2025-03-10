using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
using System;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using Newtonsoft.Json;
using Unity.VisualScripting;




public class TournamentDetailContainer : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] Image onJoinHighligher;
    [SerializeField] Button updateCountButton;
    [SerializeField] Button joinButton;
    public Dictionary<string, object> players;
    public Counter counter;
    public List<string> countries;
    [SerializeField] Text tFee;
    [SerializeField] Text tPrize;
    [SerializeField] Text tName;
    [SerializeField] Text tStatus;
    [SerializeField] Text tTime;
    [SerializeField] Text tStartTime;
    public Image bgImg;
    public string tNameVar = "";
    //  public DateTime endDate;
   public TournamentPlayerData tournamentPlayerData;


    public List<TLeaderBoard> leaderBoards;

    public int tournamentPrizeDistributionCount = 1;
    public List<TournamentPrizes> TournamentPrizeDistributionCategory;

    public TournamentDetailContainer(TournamentDetailContainer t)
    {
        updateCountButton = t.updateCountButton;
        joinButton = t.joinButton;
        players = t.players;
        counter = t.counter;
        countries = t.countries;
        tFee = t.tFee;
        tPrize = t.tPrize;
        tName = t.tName;
        tStatus = t.tStatus;
        tTime = t.tTime;
        tStartTime = t.tStartTime;
        tNameVar = t.tNameVar;
        tournamentPlayerData.Copy(t.tournamentPlayerData);
        leaderBoards = t.leaderBoards;
        tournamentPrizeDistributionCount = t.tournamentPrizeDistributionCount;
        TournamentPrizeDistributionCategory = t.TournamentPrizeDistributionCategory;

    }
    public TournamentDetailContainer()
    {



    }

    private void Awake()
    {
        // endDate = new DateTime();

    }


    public void SaveDatail(int fee, int prize, string tname, bool status, DateTime time, int playerReq, int tournamentNo, DateTime startTime)
    {

        tNameVar = tname;
        tName.text = tNameVar + "\n" + (counter.playersReq == 0 ? "Unlimited" : players.Count + "/" + counter.playersReq) + " Players";
        tFee.text = fee.ToString();
        tPrize.text = prize.ToString();

        tStatus.text = DateTime.Now >= counter.startDate.ToLocalTime() ? "Started" : "Coming Soon";
        if(tStatus.text == "Started")
        {
            onJoinHighligher.gameObject.SetActive(true);
            onJoinHighligher.color = colors[1];
        }
        else
            onJoinHighligher.gameObject.SetActive(false);
        tTime.text = time.ToString();
        tStartTime.text = startTime.ToString();
        // endDate = time;
        counter = new Counter();
        counter.entryFee = fee;
        counter.endDate = time;
        counter.startDate = startTime;
        counter.isStarted = status;
        counter.playersReq = playerReq;
        counter.prize = prize;
        counter.tournamentNo = tournamentNo;
        CheckPlayers(players);
        leaderBoards = new List<TLeaderBoard>();

        if (DateTime.Now >= time)
        {
            tStatus.text = "Ended";

            onJoinHighligher.gameObject.SetActive(true);
            onJoinHighligher.color = colors[2];
        }
        DateTime tempDate = counter.endDate;
        tempDate = tempDate.AddHours(24);
        //tempDate = tempDate.AddMinutes(3);
        if (DateTime.Now >= tempDate)
        {
            this.gameObject.SetActive(false);
            FirebaseManager.Instance.DeleteDocument(tNameVar);
        }

    }

    public void CheckPlayers(Dictionary<string, object> p)
    {


        players = p;
        if (counter.endDate.ToLocalTime() < DateTime.Now)
        {
            joinButton.gameObject.SetActive(false);
            updateCountButton.gameObject.SetActive(false);
        }
        else
        {
            if (!players.ContainsKey(FirebaseManager.Instance.User.UserId))
            {


                joinButton.gameObject.SetActive(false);
                updateCountButton.gameObject.SetActive(true);

            }
            else
            {
                joinButton.gameObject.SetActive(true);
                updateCountButton.gameObject.SetActive(false);

                tournamentPlayerData.Copy(JsonConvert.DeserializeObject<TournamentPlayerData>(players[FirebaseManager.Instance.User.UserId].ToString()));


               
            }
        }


        tName.text = tNameVar + "\n" + (counter.playersReq == 0 ? "Unlimited" : players.Count + "/" + counter.playersReq) + " Players";
    }
    public void CheckPlayers(Dictionary<string, object> p, bool loadLeader)
    {


        players = p;

        if (!players.ContainsKey(FirebaseManager.Instance.User.UserId))
        {


            joinButton.gameObject.SetActive(false);
            updateCountButton.gameObject.SetActive(true);
            onJoinHighligher.gameObject.SetActive(false);
        }
        else
        {
            joinButton.gameObject.SetActive(true);
            updateCountButton.gameObject.SetActive(false);

            tournamentPlayerData.Copy(JsonConvert.DeserializeObject<TournamentPlayerData>(players[FirebaseManager.Instance.User.UserId].ToString()));
            onJoinHighligher.gameObject.SetActive(true);
            onJoinHighligher.color = colors[1];
            
        }



        tName.text = tNameVar + "\n" + (counter.playersReq == 0 ? "Unlimited" : players.Count + "/" + counter.playersReq) + " Players";
    }
    public void CheckCountries(Dictionary<string, object> c)
    {
        countries = new List<string>();
        foreach (KeyValuePair<string, object> kvp in c)
        {
            countries.Add(kvp.Key);
        }
        if (!countries.Contains(GameHandler.Instance.countryName))
        {
            this.gameObject.SetActive(false);
        }
    }
    public void OnHandleClick()
    {

        if (!countries.Contains(GameHandler.Instance.countryName))
        {
            MainMenuHandler.Instance.countryNotAllowedPanel.SetActive(true);
        }
        else if (!players.ContainsKey(FirebaseManager.Instance.User.UserId))
        {
            if (players.Count < counter.playersReq || counter.playersReq == 0)
            {
                if (DateTime.Now <= counter.endDate.ToLocalTime())
                {
                    if (GameHandler.Instance.progressData.keys >= counter.entryFee)
                    {

                        tournamentPlayerData = new TournamentPlayerData();
                        tournamentPlayerData.tournamentName = tNameVar;
                        tournamentPlayerData.playerName = GameHandler.Instance.username;
                        tournamentPlayerData.playerID = FirebaseManager.Instance.User.UserId;
                        tournamentPlayerData.level = 1;
                        tournamentPlayerData.time = 0f;
                        tournamentPlayerData.prizeObtained = false;
                        tournamentPlayerData.endDate = counter.endDate;
                        tournamentPlayerData.entryFee = counter.entryFee;
                        tournamentPlayerData.prize = counter.prize;

                        DocumentReference countRef = FirebaseManager.Instance.dbf.Collection("Tournaments")
                            .Document(tNameVar).Collection("Detail").Document("Players");
                        Dictionary<string, object> newPlayer = new Dictionary<string, object>();
                        newPlayer.Add(FirebaseManager.Instance.User.UserId, JsonConvert.SerializeObject(tournamentPlayerData));
                        countRef.UpdateAsync(newPlayer).ContinueWithOnMainThread(task =>
                        {
                            PlayerPrefs.SetInt(tNameVar + "Level", 0);
                            GameHandler.Instance.progressData.keys -= (int)counter.entryFee;
                            joinButton.gameObject.SetActive(true);
                            updateCountButton.gameObject.SetActive(false);
                            GameHandler.Instance.progressData.playedTournament++;
                            MainMenuHandler.Instance.UpdateTournamentStats();
                            MainMenuHandler.Instance.UpdateTxts();
                            MainMenuHandler.Instance.StartFireStore();
                            FirebaseManager.Instance.SaveProgressData();
                            TournamentHistory th = new TournamentHistory();
                            th.tournamentName = tNameVar;
                            th.timeTaken = "";
                            th.badgeInd = 0;
                            th.position = "";
                            th.endDate = counter.endDate;
                            th.entryFee = counter.entryFee;
                            if (TournamentPrizeDistributionCategory.Count > 0)
                            {
                                if (TournamentPrizeDistributionCategory[0].prize.Count > 0)
                                {
                                    th.prize = TournamentPrizeDistributionCategory[0].prize[0].pValue;
                                }
                            }
                            else
                            {
                                th.prize = counter.prize;
                            }
                            FirebaseManager.Instance.UpdateTournamentHistoryData(tNameVar, JsonConvert.SerializeObject(th));
                            MainMenuHandler.Instance.soundJoinTournament.Play();
                        });
                    }
                    else
                    {
                        MainMenuHandler.Instance.detailPanelTxt.text = "Not Enough Keys";
                        MainMenuHandler.Instance.OpenPanel(MainMenuHandler.Instance.detailPanel);
                    }

                }
                else
                {
                    MainMenuHandler.Instance.detailPanelTxt.text = "Tournament Ended";
                    MainMenuHandler.Instance.OpenPanel(MainMenuHandler.Instance.detailPanel);
                    onJoinHighligher.gameObject.SetActive(true);
                    onJoinHighligher.color = colors[2];
                }
            }
        }
        else if (players.ContainsKey(FirebaseManager.Instance.User.UserId))
        {
            if (DateTime.Now >= counter.startDate.ToLocalTime())
            {
                if (DateTime.Now <= counter.endDate.ToLocalTime())
                {
                    if (players.Count >= counter.playersReq || counter.playersReq == 0)
                    {
                        GameHandler.Instance.isTournament = true;
                        tournamentPlayerData.tournamentNo = counter.tournamentNo;
                        GameHandler.Instance.tournamentCurrentPlayerData = new TournamentPlayerData(tournamentPlayerData);
                        GameHandler.Instance.tournamentCurrentPrizeDistributionCount = tournamentPrizeDistributionCount;
                        GameHandler.Instance.tournamentCurrentDefaultPrize = counter.prize;
                        GameHandler.Instance.TournamentCurrentPrizeDistributionCategory = new List<TournamentPrizes>(TournamentPrizeDistributionCategory);
                        MainMenuHandler.Instance.soundEnterTournament.Play();
                        SceneManager.LoadScene("Demo - Portrait");
                    }
                    else
                    {
                        MainMenuHandler.Instance.lowPlayers.SetActive(true);
                    }
                }
                else
                {
                    MainMenuHandler.Instance.detailPanelTxt.text = "Tournament Ended";
                    MainMenuHandler.Instance.OpenPanel(MainMenuHandler.Instance.detailPanel);
                    onJoinHighligher.gameObject.SetActive(true);
                    onJoinHighligher.color = colors[2];
                }
            }
            else
            {
                MainMenuHandler.Instance.tournamentWaitingPanel.Init(counter.startDate.ToLocalTime(), tNameVar, this);
            }


        }


    }
    public void ShowLeaderBoard()
    {
        MainMenuHandler.Instance.LoadNewLeaderBoard(tNameVar);
    }

}
[System.Serializable]
public class TLeaderBoard
{
    public string username;
    public int level;
    public int iRange;
    public float timeTaken;
    public int cash;
}