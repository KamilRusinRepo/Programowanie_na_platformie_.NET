namespace LibraryApp.Domain;

public class Reservation
{
    private static int id = 1;
    public int Id { get; }
    public LibraryItem Item { get; }
    public string UserEmail { get; }
    public DateTime From { get; }
    public DateTime To { get; }
    public bool IsActive { get; private set; } = true;

    public Reservation(LibraryItem item, string userEmail, DateTime from, DateTime to)
    {
        if (from >= to) throw new ArgumentException("From must be earlier than To.");

        Id = id;
        Item = item ?? throw new ArgumentNullException(nameof(item));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        From = from;
        To = to;
        id++;
    }

    public void Cancel()
    {
        IsActive = false;
    }
}