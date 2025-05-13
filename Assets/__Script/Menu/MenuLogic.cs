using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject loadingMenu;

    [Header("Options Submenus")]
    public GameObject controlsMenu;
    public GameObject graphicsMenu;
    public GameObject audioMenu;

    void Start()
    {
        // Initialize menu states
        ShowMainMenu();
    }

    // Load the game scene
    public void StartButton()
    {
        loadingMenu?.SetActive(true); // Show loading screen if assigned
        SceneManager.LoadScene("Test");
    }

    // Display the options menu
    public void OptionsButton()
    {
        SetMenuActive(mainMenu, false);
        SetMenuActive(optionsMenu, true);
        ShowAudioSettings(); // Default to audio settings
    }

    // Display the controls submenu in options
    public void ControlsButton()
    {
        ShowSubMenu(controlsMenu);
    }

    // Display the graphics submenu in options
    public void GraphicsButton()
    {
        ShowSubMenu(graphicsMenu);
    }

    // Display the audio submenu in options
    public void AudioButton()
    {
        ShowSubMenu(audioMenu);
    }

    // Display the credits menu
    public void CreditsButton()
    {
        SetMenuActive(mainMenu, false);
        SetMenuActive(creditsMenu, true);
    }

    // Exit the application
    public void ExitGameButton()
    {
        Debug.Log("App Has Exited");
        Application.Quit();
    }

    // Return to the main menu
    public void ReturnToMainMenuButton()
    {
        ShowMainMenu();
    }

    // Helper method to enable the main menu
    private void ShowMainMenu()
    {
        SetMenuActive(mainMenu, true);
        SetMenuActive(optionsMenu, false);
        SetMenuActive(creditsMenu, false);
        SetMenuActive(loadingMenu, false);

        HideAllSubmenus();
    }

    // Helper method to show a specific submenu in options
    private void ShowSubMenu(GameObject submenu)
    {
        HideAllSubmenus();
        if (submenu != null) submenu.SetActive(true);
    }

    // Hide all submenus under options
    private void HideAllSubmenus()
    {
        SetMenuActive(controlsMenu, false);
        SetMenuActive(graphicsMenu, false);
        SetMenuActive(audioMenu, false);
    }

    // Show audio settings by default in options
    private void ShowAudioSettings()
    {
        ShowSubMenu(audioMenu);
    }

    // Helper method to safely activate/deactivate GameObjects
    private void SetMenuActive(GameObject menu, bool isActive)
    {
        if (menu != null) menu.SetActive(isActive);
    }
}
