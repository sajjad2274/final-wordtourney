using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundMgrScriptable AllSounds;
    Transform AudioSourcePoolParent;
    public enum SoundType
    {
        TurnLeft = 0,
        TurnRight = 1,
        GoStraight = 2,

    };
    [HideInInspector]
    public SoundType currentSound;
    #region Creating Instance
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SoundManager>();
            }
            return _instance;
        }
    }
    #endregion
    void Start()
    {
        if (_instance == null)
            _instance = this;
    }
    AudioSource ASource;
    public void PlayAudioClip(AudioClip audioClip)
    {
        //if (audioClip == null)
        //{
        //    Debug.LogError("Sound missing: ");
        //    return;
        //}
        getAudioSource(Vector3.zero);
        ASource.loop = false;
        ASource.clip = audioClip;
        ASource.playOnAwake = true;
        ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
    }
    int soundNumber;
    public AudioSource PlayAudioClip(AudioClip audioClip, Vector3 position, bool isLoop, bool isReturntype)
    {
        getAudioSource(position);
        ASource.loop = isLoop;
        ASource.clip = audioClip;
        ASource.playOnAwake = true;
        if (isLoop)
            ASource.Play();
        else
            ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
        return ASource;
    }

    void getAudioSource(Vector3 position)
    {
        ASource = null;
        if (!AudioSourcePoolParent)
        {
            AudioSourcePoolParent = (new GameObject("AudioSourcesPoolParent")).transform;
        }
        if (AudioSourcePoolParent.childCount > 0)
        {
            for (int i = 0; i < AudioSourcePoolParent.childCount; i++)
            {
                if (!AudioSourcePoolParent.GetChild(i).gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    ASource = AudioSourcePoolParent.GetChild(i).gameObject.GetComponent<AudioSource>();
                    ASource.gameObject.transform.position = position;
                    break;
                }
            }
        }
        if (!ASource)
        {
            GameObject Audiosource = Instantiate(AllSounds.AudioSourcePrefab, position, Quaternion.identity);
            Audiosource.transform.parent = AudioSourcePoolParent;
            ASource = Audiosource.GetComponent<AudioSource>();
        }
        print("playing: " + soundNumber++);
    }

}