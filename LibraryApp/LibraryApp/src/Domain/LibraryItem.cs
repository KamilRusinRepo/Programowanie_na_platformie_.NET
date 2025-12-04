namespace LibraryApp.Domain;

public abstract class LibraryItem
{
    public int Id { get; }
    public string Title { get; protected set; }
    public bool IsAvailable { get; set; } = true;

    protected LibraryItem(int id, string title)
    {
        Id = id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public abstract void DisplayInfo();
}