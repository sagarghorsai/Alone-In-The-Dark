using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [Header("Sliders and Dropdowns")]
    public Slider renderQualitySlider;
    public TMP_Dropdown textureQualityDropdown;
    public TMP_Dropdown shadowQualityDropdown;
    public TMP_Dropdown postProcessingDropdown;
    public TMP_Dropdown antiAliasingDropdown;
    public Toggle motionBlurToggle;
    public Toggle fullScreenToggle;

    private const string RenderQualityKey = "RenderQuality";
    private const string TextureQualityKey = "TextureQuality";
    private const string ShadowQualityKey = "ShadowQuality";
    private const string PostProcessingKey = "PostProcessing";
    private const string AntiAliasingKey = "AntiAliasing";
    private const string MotionBlurKey = "MotionBlur";
    private const string FullScreenKey = "FullScreen";

    void Start()
    {
        // Initialize settings
        InitializeSettings();

        // Add listeners
        renderQualitySlider.onValueChanged.AddListener(SetRenderQuality);
        textureQualityDropdown.onValueChanged.AddListener(SetTextureQuality);
        shadowQualityDropdown.onValueChanged.AddListener(SetShadowQuality);
        postProcessingDropdown.onValueChanged.AddListener(SetPostProcessing);
        antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
        motionBlurToggle.onValueChanged.AddListener(SetMotionBlur);
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
    }

    private void InitializeSettings()
    {
        // Load saved settings or set defaults
        renderQualitySlider.value = PlayerPrefs.GetInt(RenderQualityKey, QualitySettings.GetQualityLevel());
        textureQualityDropdown.value = PlayerPrefs.GetInt(TextureQualityKey, 0);
        shadowQualityDropdown.value = PlayerPrefs.GetInt(ShadowQualityKey, 0);
        postProcessingDropdown.value = PlayerPrefs.GetInt(PostProcessingKey, 0);
        antiAliasingDropdown.value = PlayerPrefs.GetInt(AntiAliasingKey, 0);
        motionBlurToggle.isOn = PlayerPrefs.GetInt(MotionBlurKey, 1) == 1;
        fullScreenToggle.isOn = PlayerPrefs.GetInt(FullScreenKey, Screen.fullScreen ? 1 : 0) == 1;

        // Apply loaded settings
        SetRenderQuality(renderQualitySlider.value);
        SetTextureQuality(textureQualityDropdown.value);
        SetShadowQuality(shadowQualityDropdown.value);
        SetPostProcessing(postProcessingDropdown.value);
        SetAntiAliasing(antiAliasingDropdown.value);
        SetMotionBlur(motionBlurToggle.isOn);
        SetFullScreen(fullScreenToggle.isOn);
    }

    public void SetRenderQuality(float value)
    {
        int qualityLevel = Mathf.RoundToInt(value);
        QualitySettings.SetQualityLevel(qualityLevel);
        PlayerPrefs.SetInt(RenderQualityKey, qualityLevel);
        PlayerPrefs.Save();
    }

    public void SetTextureQuality(int index)
    {
        QualitySettings.globalTextureMipmapLimit = index;
        PlayerPrefs.SetInt(TextureQualityKey, index);
        PlayerPrefs.Save();
    }

    public void SetShadowQuality(int index)
    {
        QualitySettings.shadowResolution = (ShadowResolution)index;
        PlayerPrefs.SetInt(ShadowQualityKey, index);
        PlayerPrefs.Save();
    }

    public void SetPostProcessing(int index)
    {
        // Adjust post-processing settings here based on the index
        PlayerPrefs.SetInt(PostProcessingKey, index);
        PlayerPrefs.Save();
    }

    public void SetAntiAliasing(int index)
    {
        QualitySettings.antiAliasing = index == 0 ? 0 : (int)Mathf.Pow(2, index); // 2x, 4x, 8x
        PlayerPrefs.SetInt(AntiAliasingKey, index);
        PlayerPrefs.Save();
    }

    public void SetMotionBlur(bool isEnabled)
    {
        // Enable/disable motion blur effect
        PlayerPrefs.SetInt(MotionBlurKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isEnabled)
    {
        Screen.fullScreen = isEnabled;
        Screen.fullScreenMode = isEnabled ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        PlayerPrefs.SetInt(FullScreenKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
