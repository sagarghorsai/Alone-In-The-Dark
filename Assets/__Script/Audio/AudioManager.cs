using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("---------- Audio Sources ----------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("---------- Background Music ----------")]
    public AudioClipInfo[] backgroundList;

    [Header("---------- Audio Clips ----------")]
    public AudioClipInfo[] audioList;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (backgroundList.Length > 0 && musicSource != null)
        {
            musicSource.clip = backgroundList[0].clip;
            musicSource.Play();
            Debug.Log($"Playing background music: {backgroundList[0].name} at volume {musicSource.volume}");
        }
        else
        {
            Debug.LogWarning("No background music found or musicSource is not assigned.");
        }
    }


    public void PlayMusic(string audioName)
    {
        foreach (AudioClipInfo audioInfo in backgroundList)
        {
            if (audioInfo.name == audioName)
            {
                musicSource.clip = audioInfo.clip;
                musicSource.Play();
                Debug.Log($"Playing music: {audioInfo.name}");
                return;
            }
        }
        Debug.LogWarning($"Music clip '{audioName}' not found.");
    }

    public void PlaySFX(string audioName, float playbackSpeed = 1f)
    {
        foreach (AudioClipInfo audioInfo in audioList)
        {
            if (audioInfo.name == audioName)
            {
                sfxSource.pitch = playbackSpeed; // Adjust the playback speed
                sfxSource.PlayOneShot(audioInfo.clip);
                sfxSource.pitch = 1f; // Reset pitch to default after playing
                return;
            }
        }
        Debug.LogWarning($"SFX clip '{audioName}' not found.");
    }



    public void StopAllAudio()
    {
        musicSource.Stop();
        sfxSource.Stop();
    }
}

[System.Serializable]
public class AudioClipInfo
{
    public string name; // Word associated with the audio clip
    public AudioClip clip; // The audio clip
}
