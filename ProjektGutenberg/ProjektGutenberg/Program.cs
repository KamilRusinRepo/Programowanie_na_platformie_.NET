using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

var urls = new[]
{
    "https://www.gutenberg.org/files/84/84-0.txt",
    "https://www.gutenberg.org/files/11/11-0.txt",
    "https://www.gutenberg.org/files/1661/1661-0.txt",
    "https://www.gutenberg.org/files/2701/2701-0.txt"
};

using HttpClient client = new HttpClient();

var downloadWatch = Stopwatch.StartNew();

var downloadTasks = urls.Select(url => client.GetStringAsync(url));
string[] texts = await Task.WhenAll(downloadTasks);

downloadWatch.Stop();

var processingWatch = Stopwatch.StartNew();
var wordCounts = new ConcurrentDictionary<string, int>();

Parallel.ForEach(texts, text =>
{

    var words = Regex.Split(text, @"\W+")
                     .Where(w => w.Length > 0)
                     .Select(w => w.ToLowerInvariant());

    foreach (var word in words)
    {
        wordCounts.AddOrUpdate(word, 1, (_, old) => old + 1);
    }
});

processingWatch.Stop();

var top10 = wordCounts
    .OrderByDescending(kvp => kvp.Value)
    .Take(10);

Console.WriteLine("Najczęstsze słowa:");
int i = 1;
foreach (var entry in top10)
{
    Console.WriteLine($"{i++}. {entry.Key}: {entry.Value}");
}

Console.WriteLine($"Czas pobierania: {downloadWatch.Elapsed.TotalSeconds:F2}s");
Console.WriteLine($"Czas przetwarzania: {processingWatch.Elapsed.TotalSeconds:F2}s");