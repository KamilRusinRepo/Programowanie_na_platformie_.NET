using LibraryApp.Domain;
using LibraryApp.Services;

namespace LibraryApp.tests.LibraryApp.Tests;

public class LibraryServiceTests
{
    [Test]
    public void AddItemAndRegisterUser_Workflow()
    {
        var lib = new LibraryService();
        var id = lib.NextId();
        lib.AddItem(new Book(id, "T", "A", "isbn"));
        lib.RegisterUser("u@example.com");

        var items = lib.ListAvailableItems();
        Assert.That(1, Is.EqualTo(System.Linq.Enumerable.Count(items)));
    }

    [Test]
    public void CancelReservation_FiresEvent()
    {
        var lib = new LibraryService();
        var id = lib.NextId();
        lib.AddItem(new Book(id, "T", "A", "isbn"));
        lib.RegisterUser("u@example.com");

        var r = lib.CreateReservation(id, "u@example.com", DateTime.Now, DateTime.Now.AddDays(3));
        Reservation cancelled = null;
        lib.OnReservationCancelled += rr => cancelled = rr;

        lib.CancelReservation(r.Id);

        Assert.That(r.IsActive, Is.False);
        Assert.That(r.Id, Is.EqualTo(cancelled.Id));
    }
}