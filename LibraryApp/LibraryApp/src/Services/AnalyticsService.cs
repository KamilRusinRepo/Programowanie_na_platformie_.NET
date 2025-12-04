namespace LibraryApp.Services;

public class AnalyticsService
{
    private readonly LibraryService _library;

    public AnalyticsService(LibraryService library)
    {
        _library = library ?? throw new ArgumentNullException(nameof(library));
    }

    public double AverageLoanLengthDays()
    {
        var reservations = _library.GetAllReservations().Where(r => r.IsActive || !r.IsActive).ToList(); // all
        if (!reservations.Any()) return 0.0;
        return reservations.Average(r => (r.To - r.From).TotalDays);
    }

    public int TotalLoans()
    {
        return _library.GetAllReservations().Count;
    }

    public string MostPopularItemTitle()
    {
        var reservations = _library.GetAllReservations();
        if (!reservations.Any()) return "(no data)";

        var grouped = reservations.GroupBy(r => r.Item.Title)
            .Select(g => new { Title = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Title)
            .First();

        return grouped.Title;
    }

    public double FulfillmentRate()
    {
        var all = _library.GetAllReservations();
        if (!all.Any()) return 0.0;
        var realized = all.Count(r => r.IsActive);
        return (double)realized / all.Count();
    }
}