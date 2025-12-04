    using LibraryApp.Domain;
    using LibraryApp.Services;

    var library = new LibraryService();
    var analytics = new AnalyticsService(library);

    library.OnNewReservation += r => Console.WriteLine($"[INFO] Nowa rezerwacja: {r.Item.Title} dla {r.UserEmail} (id {r.Id})");
    library.OnReservationCancelled += r => Console.WriteLine($"[INFO] Rezerwacja anulowana: {r.Id} ({r.Item.Title})");

    // dodaj przykładowe dane
    library.AddItem(new Book(library.NextId(), "C# in Depth", "Jon Skeet", "978-1617294532"));
    library.AddItem(new EBook(library.NextId(), "Clean Code", "Robert C. Martin", "978-0132350884", "PDF"));

    while (true)
    {
        Console.WriteLine("\n=== System Biblioteczny ===");
        Console.WriteLine("1. Dodaj książkę");
        Console.WriteLine("2. Dodaj e-booka");
        Console.WriteLine("3. Zarejestruj użytkownika");
        Console.WriteLine("4. Zarezerwuj pozycję");
        Console.WriteLine("5. Anuluj rezerwację");
        Console.WriteLine("6. Pokaż dostępne pozycje");
        Console.WriteLine("7. Moje rezerwacje");
        Console.WriteLine("8. Statystyki");
        Console.WriteLine("0. Wyjście");
        Console.Write("> ");

        var choice = Console.ReadLine();
        try
        {
            switch (choice)
            {
                case "1":
                    Console.Write("Tytuł: "); var title = Console.ReadLine();
                    Console.Write("Autor: "); var author = Console.ReadLine();
                    Console.Write("ISBN: "); var isbn = Console.ReadLine();
                    library.AddItem(new Book(library.NextId(), title, author, isbn));
                    Console.WriteLine("Dodano książkę.");
                    break;
                case "2":
                    Console.Write("Tytuł: "); var t = Console.ReadLine();
                    Console.Write("Autor: "); var a = Console.ReadLine();
                    Console.Write("ISBN: "); var i = Console.ReadLine();
                    Console.Write("Format: "); var f = Console.ReadLine();
                    library.AddItem(new EBook(library.NextId(), t, a, i, f));
                    Console.WriteLine("Dodano e-booka.");
                    break;
                case "3":
                    Console.Write("Email użytkownika: "); var email = Console.ReadLine();
                    library.RegisterUser(email);
                    Console.WriteLine("Zarejestrowano użytkownika.");
                    break;
                case "4":
                    Console.Write("ID pozycji: "); int id = int.Parse(Console.ReadLine());
                    Console.Write("Email: "); var u = Console.ReadLine();
                    Console.Write("From (yyyy-MM-dd) lub ENTER = teraz: ");
                    var fromStr = Console.ReadLine();
                    DateTime from = string.IsNullOrWhiteSpace(fromStr) ? DateTime.Now : DateTime.Parse(fromStr);
                    Console.Write("To (yyyy-MM-dd) lub ENTER = +7 dni: ");
                    var toStr = Console.ReadLine();
                    DateTime to = string.IsNullOrWhiteSpace(toStr) ? DateTime.Now.AddDays(7) : DateTime.Parse(toStr);
                    var res = library.CreateReservation(id, u, from, to);
                    Console.WriteLine($"Utworzono rezerwację id {res.Id}");
                    break;
                case "5":
                    Console.Write("ID rezerwacji do anulowania: ");
                    var rid = int.Parse(Console.ReadLine());
                    library.CancelReservation(rid);
                    Console.WriteLine("Anulowano.");
                    break;
                case "6":
                    Console.Write(": ");
                    var filter = Console.ReadLine();
                    var items = library.ListAvailableItems();
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        items = items.Where(i =>
                            i.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                            || (i is Book b && b.Author.Contains(filter, StringComparison.OrdinalIgnoreCase)));
                    }

                    foreach (var item in items)
                        item.DisplayInfo();
                    break;
                case "7":
                    Console.Write("Podaj email: ");
                    var ue = Console.ReadLine();
                    var userRes = library.GetUserReservations(ue);
                    foreach (var r in userRes)
                    {
                        Console.WriteLine($"Id:{r.Id} Item:{r.Item.Title} From:{r.From} To:{r.To} Active:{r.IsActive}");
                    }
                    break;
                case "8":
                    Console.WriteLine($"Średni czas wypożyczenia: {analytics.AverageLoanLengthDays():F2} dni");
                    Console.WriteLine($"Najpopularniejszy tytuł: {analytics.MostPopularItemTitle()}");
                    Console.WriteLine($"Łączna liczba rezerwacji: {analytics.TotalLoans()}");
                    Console.WriteLine($"Fulfillment rate: {analytics.FulfillmentRate():P2}");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Nieznana opcja.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd: {ex.Message}");
        }
    }