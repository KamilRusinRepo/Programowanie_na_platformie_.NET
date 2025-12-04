using LibraryApp.Domain;
using LibraryApp.Services;

namespace LibraryApp.tests.LibraryApp.Tests;

[TestFixture]
public class AnalyticsServiceTests
{
    [Test]
    public void AverageLoanLength_Empty_ReturnsZero()
    {
        var lib = new LibraryService();
        var analytics = new AnalyticsService(lib);
        Assert.That(0.0, Is.EqualTo(analytics.AverageLoanLengthDays()));
    }

    [Test]
    public void MostPopularItemTitle_Tie_Stability()
    {
        var lib = new LibraryService();
        var id1 = lib.NextId();
        var id2 = lib.NextId();
        lib.AddItem(new Book(id1, "A", "X", "1"));
        lib.AddItem(new Book(id2, "B", "Y", "2"));
        lib.RegisterUser("u@example.com");

        lib.CreateReservation(id1, "u@example.com", DateTime.Now, DateTime.Now.AddDays(1));
        lib.CreateReservation(id2, "u@example.com", DateTime.Now, DateTime.Now.AddDays(1));

        var analytics = new AnalyticsService(lib);
        var most = analytics.MostPopularItemTitle();
        Assert.That(most == "A" || most == "B", Is.True);
    }
}