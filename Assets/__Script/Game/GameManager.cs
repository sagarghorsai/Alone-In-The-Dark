using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton instance
	public int requiredNotes = 10; // Number of notes required to win
	private int collectedNotes = 0; // Track the number of collected notes
	public string winSceneName = "WinScene"; // Name of the win scene

	[Header("UI References")]
	public TextMeshProUGUI noteText; // Reference to the TextMeshPro UI element
	public GameObject noteUIContainer; // Reference to the GameObject containing the note UI

	[Header("Animation Settings")]
	public float jumpAnimationDuration = 0.5f; // Duration of the jumping animation
	public float jumpHeight = 30f; // Height of the jump in pixels
	public float displayDuration = 3f; // How long to display the UI after pickup

	private Vector3 noteTextOriginalPosition; // To store the original position for animation
	private Coroutine hideUICoroutine; // Reference to the coroutine for hiding UI

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
		// Store the original position of the text for animation purposes
		if (noteText != null)
		{
			noteTextOriginalPosition = noteText.rectTransform.position;
		}

		// Initialize the UI with the starting note count
		UpdateGUI();

		// Make sure the UI is hidden at the start
		if (noteUIContainer != null)
		{
			noteUIContainer.SetActive(false);
		}
	}

	public void AddNote()
	{
		collectedNotes++;
		Debug.Log($"Notes collected: {collectedNotes}/{requiredNotes}");

		// Show UI and play animation
		ShowNoteUI();

		if (collectedNotes >= requiredNotes)
		{
			WinGame();
		}
	}

	private void UpdateGUI()
	{
		if (noteText != null)
		{
			noteText.text = $"{collectedNotes}/{requiredNotes} Notes";
		}
	}

	private void ShowNoteUI()
	{
		// Cancel any existing hide coroutine
		if (hideUICoroutine != null)
		{
			StopCoroutine(hideUICoroutine);
		}

		// Show the UI
		if (noteUIContainer != null)
		{
			noteUIContainer.SetActive(true);
		}

		// Update the UI with the new count
		UpdateGUI();

		// Play the jumping animation
		StartCoroutine(AnimateNoteText());

		// Start the coroutine to hide the UI after display time
		hideUICoroutine = StartCoroutine(HideUIAfterDelay());
	}

	private IEnumerator AnimateNoteText()
	{
		if (noteText != null)
		{
			float elapsedTime = 0f;
			Vector3 startPos = noteTextOriginalPosition;

			while (elapsedTime < jumpAnimationDuration)
			{
				float yOffset = jumpHeight * Mathf.Sin((elapsedTime / jumpAnimationDuration) * Mathf.PI);
				noteText.rectTransform.position = new Vector3(
					startPos.x,
					startPos.y + yOffset,
					startPos.z
				);

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			// Make sure the text returns to its original position
			noteText.rectTransform.position = noteTextOriginalPosition;
		}
	}

	private IEnumerator HideUIAfterDelay()
	{
		// Wait for the specified display duration
		yield return new WaitForSeconds(displayDuration);

		// Hide the UI
		if (noteUIContainer != null)
		{
			noteUIContainer.SetActive(false);
		}

		hideUICoroutine = null;
	}

	private void WinGame()
	{
		Debug.Log("Win condition met! Loading win scene...");
		SceneManager.LoadScene(winSceneName);
	}
}