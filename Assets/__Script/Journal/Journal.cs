using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public List<Note> notes = new List<Note>();

    public void AddNote(Note note)
    {
        notes.Add(note);
        Debug.Log($"Note added: {note.title}");
        GameManager.Instance.AddNote(); // Notify the GameManager
    }


    public void DisplayJournal()
    {
        foreach (var note in notes)
        {
            Debug.Log($"Title: {note.title}\nContent: {note.content}");
        }
    }
}
