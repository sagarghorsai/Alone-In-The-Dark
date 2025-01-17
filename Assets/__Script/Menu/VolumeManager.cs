using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Reference to the Audio Mixer
    public static VolumeManager Instance;

    [Header("Audio Mixer Parameters")]
    public string masterVolumeParam = "MasterVolume";
    public string sfxVolumeParam = "SFXVolume";
    public string musicVolumeParam = "MusicVolume";
    public string mainMenuVolumeParam = "MainMenuVolume";

    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

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
        // Initialize sliders with saved volume values
        InitializeSliders();
    }

    // Method to set the volume for a given parameter
    public void SetVolume(string parameter, float value)
    {
        // Convert the linear slider value (0.001 to 1) to decibels (-80 to 0 dB)
        float decibelValue = Mathf.Log10(Mathf.Clamp(value, 0.001f, 1f)) * 20;
        audioMixer.SetFloat(parameter, decibelValue);

        // Save the volume setting to PlayerPrefs
        PlayerPrefs.SetFloat(parameter, value);
        PlayerPrefs.Save();
    }

    // Get the saved volume value for a parameter
    public float GetVolume(string parameter)
    {
        // Return the saved value or default to 1.0 (100%)
        return PlayerPrefs.HasKey(parameter) ? PlayerPrefs.GetFloat(parameter) : 1f;
    }

    // Initialize sliders with saved values and attach their listeners
    private void InitializeSliders()
    {
        if (masterSlider != null)
        {
            masterSlider.value = GetVolume(masterVolumeParam);
            masterSlider.onValueChanged.AddListener((value) => SetVolume(masterVolumeParam, value));
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = GetVolume(sfxVolumeParam);
            sfxSlider.onValueChanged.AddListener((value) => SetVolume(sfxVolumeParam, value));
        }

        if (musicSlider != null)
        {
            musicSlider.value = GetVolume(musicVolumeParam);
            musicSlider.onValueChanged.AddListener((value) => SetVolume(musicVolumeParam, value));
        }

     
    }

    // Retrieve the volume from the mixer as a linear value (0 to 1)
    public float GetVolumeFromMixer(string parameter)
    {
        if (audioMixer.GetFloat(parameter, out float decibelValue))
        {
            return Mathf.Pow(10, decibelValue / 20); // Convert decibels back to linear volume
        }
        return 1f; // Default to full volume if parameter not found
    }
}
