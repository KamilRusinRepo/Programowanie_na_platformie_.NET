namespace LibraryApp.Domain;

public class Book : LibraryItem
{
    public string Author { get; }
    public string Isbn { get; }

    public Book(int id, string title, string author, string isbn) : base(id, title)
    {
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Isbn = isbn ?? throw new ArgumentNullException(nameof(isbn));
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Książka] ID: {Id}, Tytuł: {Title}, Autor: {Author}, ISBN: {Isbn}, Dostępna: {(IsAvailable ? "Tak" : "Nie")}");
    }
}