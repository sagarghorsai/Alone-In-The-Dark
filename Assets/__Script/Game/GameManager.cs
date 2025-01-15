using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance
    public int requiredNotes = 10; // Number of notes required to win
    private int collectedNotes = 0; // Track the number of collected notes
    public string winSceneName = "WinScene"; // Name of the win scene

    public TextMeshProUGUI noteText; // Reference to the TextMeshPro UI element

    private void Awake()
    {
        // Implementing Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one GameManager
        }
    }

    private void Start()
    {
        UpdateGUI(); // Initialize the UI with the starting note count
    }

    public void AddNote()
    {
        collectedNotes++;
        Debug.Log($"Notes collected: {collectedNotes}/{requiredNotes}");

        UpdateGUI(); // Update the UI with the new count

        if (collectedNotes >= requiredNotes)
        {
            WinGame();
        }
    }

    private void UpdateGUI()
    {
        if (noteText != null)
        {
            noteText.text = $"{collectedNotes}/{requiredNotes}";
        }
    }

    private void WinGame()
    {
        Debug.Log("Win condition met! Loading win scene...");
        SceneManager.LoadScene(winSceneName);
    }
}
