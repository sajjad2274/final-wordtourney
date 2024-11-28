//using UnityEditor;
//using UnityEngine;
//using Firebase.Firestore;
//using System.Threading.Tasks;

//public class DeleteTournamentTool : EditorWindow
//{
//    FirebaseFirestore db;

//    [MenuItem("Tools/Delete Tournament Document")]
//    public static void ShowWindow()
//    {
//        // Show the custom window in the editor
//        GetWindow<DeleteTournamentTool>("Delete Tournament Document");
//    }

//    private void OnGUI()
//    {
//        // GUI layout in the Unity Editor window
//        GUILayout.Label("Delete Tournament Document", EditorStyles.boldLabel);

//        if (GUILayout.Button("Delete 'Beginner' Document"))
//        {
//            DeleteBeginnerDocument();
//        }
//    }

//    private void DeleteBeginnerDocument()
//    {
//        // Initialize Firestore if not already initialized
//        db = FirebaseFirestore.DefaultInstance;

//        // Reference to the 'Beginner' document
//        DocumentReference docRef = db.Collection("Tournaments").Document("Beginner");

//        // Delete the document asynchronously
//        Task deleteTask = docRef.DeleteAsync();
//        deleteTask.ContinueWith(task =>
//        {
//            // Check if the task is completed successfully
//            if (task.IsCompleted)
//            {
//                // Run on main thread to safely update Unity's UI or state
//                EditorApplication.update += () =>
//                {
//                    Debug.Log("'Beginner' document deleted successfully.");
//                    EditorApplication.update -= null;  // Ensure we remove the delegate after use
//                };
//            }
//            else
//            {
//                // Run error logging on the main thread as well
//                EditorApplication.update += () =>
//                {
//                    Debug.LogError("Failed to delete 'Beginner' document: " + task.Exception);
//                    EditorApplication.update -= null;
//                };
//            }
//        });
//    }
//}
