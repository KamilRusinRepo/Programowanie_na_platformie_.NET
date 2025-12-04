using LibraryApp.Domain;
using LibraryApp.Extensions;

namespace LibraryApp.tests.LibraryApp.Tests;

[TestFixture]
public class ExtensionsTests
{
    [Test]
    public void AvailableAndNewest_Works()
    {
        var b1 = new Book(1, "a", "x", "1");
        var b2 = new Book(2, "b", "y", "2");

        var all = new[] { b1, b2 };
        var avail = all.Available().ToList();
        var newest = all.Newest(1).ToList();
        Assert.That(1, Is.EqualTo(newest.Count));
        Assert.That(2, Is.EqualTo(newest[0].Id));
    }
}