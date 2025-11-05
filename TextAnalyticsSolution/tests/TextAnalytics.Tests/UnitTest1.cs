using TextAnalytics.Core;

namespace TextAnalytics.Tests
{
    [TestFixture]
    public class TextAnalyzerTests
    {
        private TextAnalyzer analyzer;

        [SetUp]
        public void Setup()
        {
            analyzer = new TextAnalyzer();
        }

        [Test]
        public void CountWords_Returns2_ForHelloWorld()
        {
            
            var stats = analyzer.Analyze("Hello world!");
            Assert.That(stats.WordCount, Is.EqualTo(2));
        }

        [Test]
        public void CountWords_Returns0_ForEmptyText()
        {
            var stats = analyzer.Analyze("");
            Assert.That(stats.WordCount, Is.EqualTo(0));
        }

        [Test]
        public void CountWords_Returns0_ForWhitespaceOnly()
        {
            var stats = analyzer.Analyze("     ");
            Assert.That(stats.WordCount, Is.EqualTo(0));
        }

        [Test]
        public void Analyze_CorrectlyCountsCharactersWithAndWithoutSpaces()
        {
            var stats = analyzer.Analyze("Hi there");
            Assert.That(stats.CharactersWithSpaces, Is.EqualTo(8));
            Assert.That(stats.CharactersWithoutSpaces, Is.EqualTo(7));
        }

        [Test]
        public void Analyze_CorrectlyCountsLettersDigitsAndPunctuation()
        {
            var stats = analyzer.Analyze("Hi! 123...");
            Assert.That(stats.Letters, Is.EqualTo(2));
            Assert.That(stats.Digits, Is.EqualTo(3));
            Assert.That(stats.Punctuation, Is.EqualTo(4));
        }

        [Test]
        public void Analyze_CorrectlyIdentifiesMostCommonWord()
        {
            var stats = analyzer.Analyze("dog cat dog cat dog");
            Assert.That(stats.MostCommonWord, Is.EqualTo("dog"));
        }

        [Test]
        public void Analyze_ResolvesWordFrequencyTies_ByFirstOccurrence()
        {
 
            var stats = analyzer.Analyze("apple banana apple banana");
            Assert.That(stats.MostCommonWord, Is.EqualTo("apple"));
        }

        [Test]
        public void Analyze_CorrectlyCountsSentencesAndLongestSentence()
        {
            var text = "This is a sentence. This is another one! Short?";
            var stats = analyzer.Analyze(text);

            Assert.That(stats.SentenceCount, Is.EqualTo(3));
            Assert.That(stats.LongestSentence.Contains("sentence"));
        }
    }
}