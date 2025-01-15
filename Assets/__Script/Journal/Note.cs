[System.Serializable]
public class Note
{
    public string title;
    public string content;

    public Note(string title, string content)
    {
        this.title = title;
        this.content = content;
    }
}
