using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videeoplay : MonoBehaviour
{
  
    public VideoPlayer videoPlayer;
    public string[] videoChunks; 
    private int currentChunkIndex = 0;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

      
        videoPlayer.loopPointReached += OnVideoChunkFinished;

       
        PlayNextVideoChunk();
    }

    void PlayNextVideoChunk()
    {
        if (currentChunkIndex < videoChunks.Length)
        {
            string videoChunkPath = videoChunks[currentChunkIndex];
            videoPlayer.url = videoChunkPath;
            videoPlayer.Play();
            currentChunkIndex++;
        }
        else
        {
           
            Debug.Log("Video streaming completed.");
        }
    }

    void OnVideoChunkFinished(VideoPlayer vp)
    {
    
        PlayNextVideoChunk();
    }
}
