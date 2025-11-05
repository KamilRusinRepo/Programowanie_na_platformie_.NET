namespace TextAnalytics.Services;

public class ConsoleInputProvider : IInputProvider
{
    public string GetInput()
    {
        Console.WriteLine("Podaj tekst do analizy:");
        return Console.ReadLine() ?? string.Empty;
    }
}