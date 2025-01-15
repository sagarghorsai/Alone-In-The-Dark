using UnityEngine;

public class NotePickup : MonoBehaviour, IInteractable
{
    public string noteTitle;
    [TextArea]
    public string noteContent;

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
        // Find the player's journal and add the note
        Journal playerJournal = FindObjectOfType<Journal>();
        if (playerJournal != null)
        {
            Note newNote = new Note(noteTitle, noteContent);
            playerJournal.AddNote(newNote);
            Debug.Log($"Picked up note: {noteTitle}");
            Destroy(gameObject); // Remove the note from the world
        }
    }
}
