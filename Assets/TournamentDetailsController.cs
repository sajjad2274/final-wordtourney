using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentDetailsController : MonoBehaviour
{
    public static TournamentDetailsController Instance;

    private void Awake()
    {
        Instance = this;
    }

   // public List<TournamentDetail> tournamentDetails=new List<TournamentDetail>();


    //public void ListAllDocuments(string[] allTournamentsName)
    //{
    //    tournamentDetails.Clear();
    //    foreach (var item in allTournamentsName)
    //    {
    //        var data = new TournamentDetail();
    //        data.id = item;
    //        tournamentDetails.Add(data);
    //    }
  
    //}
    public void UpdateData()
    {
        //var dbf = FirebaseManager.Instance.dbf;
        //foreach (var item in tournamentDetails)
        //{

        //dbf.Collection("Tournaments").Document(item.id).Collection("Detail").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    if (task.IsCompleted && !task.IsFaulted)
        //    {
        //        QuerySnapshot snapshot = task.Result;
        //        foreach (DocumentSnapshot document in snapshot.Documents)
        //        {
        //            Dictionary<string, object> data = document.ToDictionary();

        //            if (document.Id == "Country")
        //            {
        //                Debug.Log("New Sadiq----------------------------------------------" + document.Id);
        //                item.countries = new List<string>();
        //                foreach (KeyValuePair<string, object> kvp in data)
        //                {
        //                    item.countries.Add(kvp.Key);
        //                }
        //            }
        //        }

        //        }

        //});
        //}

    }
}


