using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	[Header("---------- Audio Sources ----------")]
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioSource sfxSource;

	[Header("---------- Background Music ----------")]
	public AudioClipInfo[] backgroundList;

	[Header("---------- Scene Music Mapping ----------")]
	public SceneMusic[] sceneMusicList;

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

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void Start()
	{
		if (backgroundList.Length > 0 && musicSource != null)
		{
			musicSource.clip = backgroundList[0].clip;
			musicSource.Play();
			Debug.Log($"Playing default background music: {backgroundList[0].name} at volume {musicSource.volume}");
		}
		else
		{
			Debug.LogWarning("No background music found or musicSource is not assigned.");
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		foreach (SceneMusic mapping in sceneMusicList)
		{
			if (mapping.sceneName == scene.name)
			{
				PlayMusic(mapping.musicName);
				return;
			}
		}

		Debug.LogWarning($"No music mapped for scene '{scene.name}'.");
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
	public string name; // Name of the audio clip
	public AudioClip clip; // The actual audio clip
}

[System.Serializable]
public class SceneMusic
{
	public string sceneName;   // Name of the Unity scene
	public string musicName;   // Name of the music in backgroundList to play in that scene
}
