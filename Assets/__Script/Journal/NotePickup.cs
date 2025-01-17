using UnityEngine;

public class NotePickup : MonoBehaviour, IInteractable
{
    public string noteTitle;
    [TextArea]
    public string noteContent;

    public AudioClip pickupAudioClip; // Audio to play when the note is picked up

    public string GetInteractionText()
    {
        return $"Pick up note: {noteTitle}";
    }

    public InteractType GetInteractType()
    {
        return InteractType.Click; // Notes require a simple click to pick up
    }

    public void Interact()
    {
        // Play the pickup audio
        if (pickupAudioClip != null)
        {
            // Create a temporary audio source to play the clip
            GameObject tempAudioSource = new GameObject("TempAudio");
            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = pickupAudioClip;
            audioSource.Play();

            // Destroy the audio source GameObject after the clip finishes playing
            Destroy(tempAudioSource, pickupAudioClip.length);
        }

        // Find the player's journal and add the note
        Journal playerJournal = FindObjectOfType<Journal>();
        if (playerJournal != null)
        {
            Note newNote = new Note(noteTitle, noteContent);
            playerJournal.AddNote(newNote);
            Debug.Log($"Picked up note: {noteTitle}");
        }

        // Immediately remove the note GameObject from the scene
        Destroy(gameObject);
    }
}
