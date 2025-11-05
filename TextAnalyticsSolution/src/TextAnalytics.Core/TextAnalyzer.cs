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

            int wordCount = CountWords(text);
            int uniqueWordCount = CountUniqueWords(text);
            string mostCommonWord = GetMostCommonWord(text);
            double averageWordLength = AverageWordLength(text);
            string longestWord = LongestWord(text);
            string shortestWord = ShortestWord(text);

            var sentences = GetSentences(text);
            int sentenceCount = SentencesCount(text);
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

        private int CountWords(string text)
        {
            return Regex.Matches(text, @"\b\w+\b").Count;
        }

        private int CountUniqueWords(string text)
        {
            var wordsList = GetWords(text);
            return wordsList.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        }

        private double AverageWordLength(string text)
        {
            var words = GetWords(text);
            return words.Count > 0 ? words.Average(w => w.Length) : 0;
        }

        private string LongestWord(string text)
        {
            var words = GetWords(text);
            return words.OrderByDescending(w => w.Length).FirstOrDefault() ?? string.Empty;
        }

        private string ShortestWord(string text)
        {
            var words = GetWords(text);
            return words.OrderBy(w => w.Length).FirstOrDefault() ?? string.Empty;
        }

        private List<string> GetWords(string text)
        {
            return Regex.Matches(text, @"\b\w+\b")
                .Select(m => m.Value)
                .ToList();
        }

        private string GetMostCommonWord(string text)
        {
            var words = GetWords(text);
            if (words.Count == 0) return string.Empty;

            return words.GroupBy(w => w, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .First().Key;
        }

        private int SentencesCount(string text)
        {
            var sentences = GetSentences(text);
            return sentences.Count;
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
