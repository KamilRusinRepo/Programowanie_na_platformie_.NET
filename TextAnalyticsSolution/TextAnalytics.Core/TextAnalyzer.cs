using System.Text.RegularExpressions;

namespace TextAnalytics.Core
{
    public sealed class TextAnalyzer
    {
        public TextStatistics Analyze(string text)
        {
            int charactersWithSpaces = CountCharacters(text, includeSpaces: true);
            int charactersWithoutSpaces = CountCharacters(text, includeSpaces: false);
            int letters = CountLetters(text);
            int digits = CountDigits(text);
            int punctuation = CountPunctuation(text);

            var words = GetWords(text);
            int wordCount = words.Count;
            int uniqueWordCount = words.Distinct(StringComparer.OrdinalIgnoreCase).Count();
            string mostCommonWord = GetMostCommonWord(words);
            double averageWordLength = words.Count > 0 ? words.Average(w => w.Length) : 0;
            string longestWord = words.OrderByDescending(w => w.Length).FirstOrDefault() ?? string.Empty;
            string shortestWord = words.OrderBy(w => w.Length).FirstOrDefault() ?? string.Empty;

            var sentences = GetSentences(text);
            int sentenceCount = sentences.Count;
            double averageWordsPerSentence = sentenceCount > 0 ? (double)wordCount / sentenceCount : 0;
            string longestSentence = sentences.OrderByDescending(s => GetWords(s).Count).FirstOrDefault() ?? string.Empty;

            return new TextStatistics(
                charactersWithSpaces,
                charactersWithoutSpaces,
                letters,
                digits,
                punctuation,
                wordCount,
                uniqueWordCount,
                mostCommonWord,
                averageWordLength,
                longestWord,
                shortestWord,
                sentenceCount,
                averageWordsPerSentence,
                longestSentence
            );
        }

        public int CountCharacters(string text, bool includeSpaces = true)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return includeSpaces
                ? text.Length
                : text.Count(c => !char.IsWhiteSpace(c));
        }

        public int CountLetters(string text)
        {
            return text.Count(char.IsLetter);
        }

        public int CountDigits(string text)
        {
            return text.Count(char.IsDigit);
        }

        public int CountPunctuation(string text)
        {
            return text.Count(char.IsPunctuation);
        }

        private List<string> GetWords(string text)
        {
            return Regex.Matches(text, @"\b\w+\b")
                .Select(m => m.Value)
                .ToList();
        }

        private string GetMostCommonWord(List<string> words)
        {
            if (words.Count == 0) return string.Empty;

            return words.GroupBy(w => w, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .First().Key;
        }

        private List<string> GetSentences(string text)
        {
            return Regex.Split(text, @"(?<=[.!?])\s+")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

    }

    public sealed record TextStatistics(
        int CharactersWithSpaces,
        int CharactersWithoutSpaces,
        int Letters,
        int Digits,
        int Punctuation,
        int WordCount,
        int UniqueWordCount,
        string MostCommonWord,
        double AverageWordLength,
        string LongestWord,
        string ShortestWord,
        int SentenceCount,
        double AverageWordsPerSentence,
        string LongestSentence
    );
}
