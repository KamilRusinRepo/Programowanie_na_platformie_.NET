namespace TextAnalytics.Services;

public class FileInputProvider : IInputProvider
{
    private readonly string _filePath;

    public FileInputProvider(string filePath)
    {
        _filePath = filePath;
    }

    public string GetInput()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"Plik {_filePath} nie istnieje.");

        return File.ReadAllText(_filePath);
    }
}