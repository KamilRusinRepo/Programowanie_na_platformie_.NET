namespace LibraryApp.Domain;

public class EBook : Book
{
    public string FileFormat { get; }

    public EBook(int id, string title, string author, string isbn, string fileFormat)
        : base(id, title, author, isbn)
    {
        FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[E-book] ID: {Id}, Tytuł: {Title}, Autor: {Author}, Format: {FileFormat}, Dostępna: {(IsAvailable ? "Tak" : "Nie")}");
    }
}