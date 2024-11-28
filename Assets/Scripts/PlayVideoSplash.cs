
using UnityEngine;
using UnityEngine.Video;


public class PlayVideoSplash : MonoBehaviour
{
    public VideoPlayer VP;
    [SerializeField] string videoFileName;


    private void Awake()
    {
        Invoke(nameof(PlayVideos),1f);
    }



    public void PlayVideos()
    {
        if (VP)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            VP.url = videoPath;
            VP.Play();
        }

    }
}
