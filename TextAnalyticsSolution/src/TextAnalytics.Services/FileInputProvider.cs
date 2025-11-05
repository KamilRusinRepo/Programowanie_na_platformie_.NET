namespace TextAnalytics.Services;

public class FileInputProvider : IInputProvider
{
    public string GetInput()
    {
        Console.Write("Podaj ścieżkę do pliku: ");
        string? path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Nie podano ścieżki do pliku.");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Plik '{path}' nie istnieje.");

        string content = File.ReadAllText(path);

        return content;
    }
}