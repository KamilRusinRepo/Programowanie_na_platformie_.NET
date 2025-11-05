using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TextAnalytics.Core;
using TextAnalytics.Services;

var services = new ServiceCollection()
               .AddSingleton<ILoggerService, ConsoleLogger>()
               .AddSingleton<ConsoleInputProvider>()
               .AddSingleton<FileInputProvider>()
               .AddSingleton<TextAnalyzer>()
               .BuildServiceProvider();

var logger = services.GetRequiredService<ILoggerService>();
var analyzer = services.GetRequiredService<TextAnalyzer>();
var inputProvider = services.GetRequiredService<ConsoleInputProvider>();
var fileReader = services.GetRequiredService<FileInputProvider>();


logger.LogInfo("Aplikacja uruchomiona.");

string text = string.Empty;

if (args.Length > 0)
{
    if (args[0] == "--file")
    {
        if (args.Length < 2)
        {
            logger.LogError("Brak ścieżki do pliku");
            return;
        }

        string path = args[1];

        try
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Nie podano ścieżki do pliku.");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Plik '{path}' nie istnieje.");

            text = File.ReadAllText(path);
        }
        catch(Exception ex)
        {
            logger.LogError("Błędna ścieżka do pliku.");
        }
    }
    else if (args[0] == "--interactive")
    {
        text = inputProvider.GetInput();
    }
    else
    {
        Console.WriteLine($"Nieznana opcja: {args[0]}");
    }
}
else
{
    Console.WriteLine("Wybierz źródło danych:");
    Console.WriteLine("1 - wpisz tekst z klawiatury");
    Console.WriteLine("2 - podaj ścieżkę do pliku");
    Console.Write("Twój wybór: ");
    string choice = Console.ReadLine() ?? "";

    if (choice == "1")
    {
        text = inputProvider.GetInput();
    }
    else if (choice == "2")
    {

        try
        {
            text = fileReader.GetInput();
        }
        catch(Exception ex)
        {
            logger.LogError($"Błąd: {ex.Message}");
        }
    }
    else
    {
        logger.LogError("Niepoprawny wybór opcji.");
        return;
    }
}

if (string.IsNullOrWhiteSpace(text))
{
    logger.LogError("Brak danych do analizy (tekst jest pusty).");
    return;
}

// Analiza tekstu
var stats = analyzer.Analyze(text);

// Prezentacja wyników w konsoli
Console.WriteLine("\n================ WYNIKI ANALIZY =================");
Console.WriteLine($"Znaki (ze spacjami):     {stats.CharactersWithSpaces}");
Console.WriteLine($"Znaki (bez spacji):      {stats.CharactersWithoutSpaces}");
Console.WriteLine($"Litery:                  {stats.Letters}");
Console.WriteLine($"Cyfry:                   {stats.Digits}");
Console.WriteLine($"Interpunkcja:            {stats.Punctuation}");
Console.WriteLine($"Slowa:                   {stats.WordCount}");
Console.WriteLine($"Unikalne slowa:          {stats.UniqueWordCount}");
Console.WriteLine($"Najczestsze slowo:       {stats.MostCommonWord}");
Console.WriteLine($"Srednia dlugosc slowa:   {stats.AverageWordLength:F2}");
Console.WriteLine($"Najdluzsze slowo:        {stats.LongestWord}");
Console.WriteLine($"Najkrotsze slowo:        {stats.ShortestWord}");
Console.WriteLine($"Liczba zdan:             {stats.SentenceCount}");
Console.WriteLine($"Sr. slow/zdanie:         {stats.AverageWordsPerSentence:F2}");
Console.WriteLine($"Najdluzsze zdanie:       {stats.LongestSentence}");
Console.WriteLine("=================================================\n");

// Zapis wyników do JSON
string json = JsonConvert.SerializeObject(stats, Newtonsoft.Json.Formatting.Indented);
File.WriteAllText("results.json", json);

logger.LogSummary("Wyniki zapisane do pliku results.json.");
