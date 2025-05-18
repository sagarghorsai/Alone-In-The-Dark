using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	[Header("Audio Sources")]
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioSource sfxSource;

	[Header("Audio Mixer & Snapshots")]
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private AudioMixerSnapshot normalSnapshot;
	[SerializeField] private AudioMixerSnapshot tensionSnapshot;
	[SerializeField] private float snapshotTransitionTime = 1f;

	[Header("Background Music Clips")]
	[SerializeField] private AudioClipInfo[] backgroundList;

	[Header("Scene Music Mapping")]
	[SerializeField] private SceneMusic[] sceneMusicList;

	[Header("2D SFX Clips")]
	[SerializeField] private AudioClipInfo[] audioList;

	// Runtime lookup tables
	private Dictionary<string, AudioClip> musicClips;
	private Dictionary<string, AudioClip> sfxClips;

	private void Awake()
	{
		// Singleton
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

		// Build dictionaries for O(1) lookup
		musicClips = new Dictionary<string, AudioClip>();
		foreach (var info in backgroundList)
			if (!string.IsNullOrEmpty(info.name) && info.clip != null)
				musicClips[info.name] = info.clip;

		sfxClips = new Dictionary<string, AudioClip>();
		foreach (var info in audioList)
			if (!string.IsNullOrEmpty(info.name) && info.clip != null)
				sfxClips[info.name] = info.clip;
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
		// Play the first background track as default
		if (backgroundList.Length > 0 && musicSource != null)
		{
			var defaultInfo = backgroundList[0];
			if (defaultInfo.clip != null)
			{
				musicSource.clip = defaultInfo.clip;
				musicSource.Play();
			}
		}
	}

	// Auto‐swap music when a scene loads
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		foreach (var map in sceneMusicList)
		{
			if (map.sceneName == scene.name)
			{
				PlayMusic(map.musicName);
				return;
			}
		}
		Debug.LogWarning($"[AudioManager] No music mapped for scene '{scene.name}'.");
	}

	/// <summary>
	/// Cross‐fade to a new music clip by name.
	/// </summary>
	public void PlayMusic(string musicName, float fadeTime = 1f)
	{
		if (musicClips.TryGetValue(musicName, out var clip))
			StartCoroutine(FadeMusicTo(clip, fadeTime));
		else
			Debug.LogWarning($"[AudioManager] Music '{musicName}' not found.");
	}

	private IEnumerator FadeMusicTo(AudioClip newClip, float fadeTime)
	{
		float startVol = musicSource.volume;

		// Fade out
		for (float t = 0; t < fadeTime; t += Time.deltaTime)
		{
			musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
			yield return null;
		}
		musicSource.volume = 0f;

		// Switch clip & play
		musicSource.clip = newClip;
		musicSource.Play();

		// Fade in
		for (float t = 0; t < fadeTime; t += Time.deltaTime)
		{
			musicSource.volume = Mathf.Lerp(0f, startVol, t / fadeTime);
			yield return null;
		}
		musicSource.volume = startVol;
	}

	/// <summary>
	/// Play a simple 2D one‐shot SFX (no spatialisation).
	/// </summary>
	public void PlaySFX(string sfxName, float pitch = 1f)
	{
		if (sfxClips.TryGetValue(sfxName, out var clip))
		{
			sfxSource.pitch = pitch;
			sfxSource.PlayOneShot(clip);
			sfxSource.pitch = 1f;
		}
		else
		{
			Debug.LogWarning($"[AudioManager] SFX '{sfxName}' not found.");
		}
	}

	/// <summary>
	/// Spawn a temporary AudioSource at worldPos for 3D/spatial SFX.
	/// </summary>
	public void PlaySpatialSFX(string sfxName, Vector3 worldPos,
							   float volume = 1f,
							   float minDistance = 1f,
							   float maxDistance = 20f)
	{
		if (!sfxClips.TryGetValue(sfxName, out var clip))
		{
			Debug.LogWarning($"[AudioManager] Spatial SFX '{sfxName}' not found.");
			return;
		}

		GameObject go = new GameObject($"SFX3D_{sfxName}");
		go.transform.position = worldPos;
		var src = go.AddComponent<AudioSource>();
		src.clip = clip;
		src.spatialBlend = 1f;      // fully 3D
		src.minDistance = minDistance;
		src.maxDistance = maxDistance;
		src.volume = volume;
		src.Play();

		Destroy(go, clip.length + 0.1f);
	}

	/// <summary>
	/// Transition to “tense” or “normal” mixer snapshot.
	/// </summary>
	public void SetTension(bool isTense)
	{
		if (audioMixer == null || normalSnapshot == null || tensionSnapshot == null)
			return;

		if (isTense)
			tensionSnapshot.TransitionTo(snapshotTransitionTime);
		else
			normalSnapshot.TransitionTo(snapshotTransitionTime);
	}

	/// <summary>
	/// Immediately stops both music & SFX sources.
	/// </summary>
	public void StopAllAudio()
	{
		musicSource.Stop();
		sfxSource.Stop();
	}
}

[System.Serializable]
public class AudioClipInfo
{
	public string name;  // unique key
	public AudioClip clip;
}

[System.Serializable]
public class SceneMusic
{
	public string sceneName;  // exact name of your Scene
	public string musicName;  // matches one of the AudioClipInfo.name in backgroundList
}