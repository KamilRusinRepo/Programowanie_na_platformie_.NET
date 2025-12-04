using LibraryApp.Domain;

namespace LibraryApp.Services;

public class LibraryService
{
    private readonly List<LibraryItem> _items = new List<LibraryItem>(); 
    private readonly List<Reservation> _reservations = new List<Reservation>();
    private readonly List<string> _users = new List<string>();

    public event Action<Reservation> OnNewReservation;
    public event Action<Reservation> OnReservationCancelled;

    public IReadOnlyCollection<Reservation> GetAllReservations() => _reservations.AsReadOnly();
    public IReadOnlyCollection<LibraryItem> GetAllItems() => _items.AsReadOnly();
    
    private int _nextItemId = 1;
    
    public int NextId() => _nextItemId++;


    public void AddItem(LibraryItem item)
    {
        if (_items.Any(i => i.Id == item.Id))
        {
            throw new ArgumentException($"Pozycja o ID {item.Id} już istnieje.");
        }
        _items.Add(item);
    }

    public void RegisterUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email nie może być pusty.", nameof(email));

        if (!_users.Contains(email.ToLower()))
        {
            _users.Add(email.ToLower());
        }
    }

    public IEnumerable<LibraryItem> ListAvailableItems()
    {
        var query = _items.Where(i => i.IsAvailable);

        return query;
    }

    public Reservation CreateReservation(int itemId, string userEmail, DateTime from, DateTime to)
    {
        // Walidacja użytkownika
        if (!_users.Contains(userEmail.ToLower()))
            throw new ArgumentException($"Użytkownik {userEmail} nie jest zarejestrowany.");

        // Walidacja parametrów
        if (from >= to)
            throw new ArgumentException("Data 'od' musi być wcześniejsza niż data 'do'.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException($"Nie znaleziono pozycji o ID {itemId}.");

        // Walidacja dostępności
        if (!item.IsAvailable)
            throw new InvalidOperationException($"Pozycja '{item.Title}' jest aktualnie niedostępna.");

        // Walidacja konfliktu terminów
        if (_reservations.Any(r => r.Item.Id == itemId && r.IsActive && IsConflict(r, from, to)))
        {
            throw new ReservationConflictException($"Kolizja terminów rezerwacji dla '{item.Title}'.");
        }

        // Rezerwacja
        var newReservation = new Reservation(item, userEmail, from, to);
        _reservations.Add(newReservation);
        item.IsAvailable = false;

        // Zdarzenie
        OnNewReservation?.Invoke(newReservation);

        return newReservation;
    }

    public void CancelReservation(int reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId && r.IsActive);

        if (reservation == null)
        {
            throw new ArgumentException($"Nie znaleziono aktywnej rezerwacji o ID {reservationId}.");
        }

        reservation.Cancel();
        reservation.Item.IsAvailable = true; // Zmiana stanu pozycji

        // Zdarzenie
        OnReservationCancelled?.Invoke(reservation);
    }

    public IEnumerable<Reservation> GetUserReservations(string userEmail)
    {
        return _reservations.Where(r => r.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
    }

    // Funkcja pomocnicza do walidacji konfliktu
    private static bool IsConflict(Reservation existing, DateTime from, DateTime to)
    {
        // Konflikt zachodzi, gdy interwały się nakładają:
        return (existing.From < to) && (existing.To > from);
    }
}