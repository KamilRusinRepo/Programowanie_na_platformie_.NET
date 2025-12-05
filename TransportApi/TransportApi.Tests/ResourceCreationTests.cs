using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Extensions;
using TransportApi.Models;
using TransportApi.Services;

namespace TransportApi.Tests;

[TestFixture]
public class ResourceCreationTests
{
    private DbContextOptions<FleetDbContext> _contextOptions;

    [SetUp]
    public void Setup()
    {
        // Konfiguracja DbContext in-memory
        _contextOptions = new DbContextOptionsBuilder<FleetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private async Task<(int driverId, int vehicleId)> SeedDataAsync(FleetDbContext context)
    {
        var truck = new Truck { RegistrationNumber = "KR1A", MaxLoadKg = 10000, TrailerLength = 13.5, IsAvailable = true };
        var driver = new Driver { Name = "Jan Kowalski", IsAvailable = true, LicenseNumber = "A123" };
    
        context.Vehicles.Add(truck);
        context.Drivers.Add(driver);
        await context.SaveChangesAsync();

        // Upewnij się, że Vehicle/Driver ma zdefiniowane metody StartOrder/CompleteOrder lub właściwości
        truck.StartOrder(); // Tylko testujemy, by upewnić się, że można wywołać
        truck.IsAvailable = true; // Zwracamy do stanu dostępności
        driver.IsAvailable = true;

        return (driver.Id, truck.Id);
    }
    private FleetDbContext CreateContext() => new FleetDbContext(_contextOptions);
    private FleetManager CreateManager(FleetDbContext context) => new FleetManager(context);

    [Test]
    public async Task AddDriver_ShouldBeSavedAndIdGenerated()
    {
        using var context = CreateContext();
        
        var newDriver = new Driver 
        { 
            Name = "Anna Pilot", 
            LicenseNumber = "DRV999", 
            IsAvailable = true 
        };

        context.Drivers.Add(newDriver);
        await context.SaveChangesAsync();
        
        // 1. Sprawdzenie, czy ID zostało wygenerowane
        Assert.That(newDriver.Id, Is.GreaterThan(0), "ID powinno być automatycznie wygenerowane.");
        
        // 2. Sprawdzenie, czy obiekt istnieje w bazie
        var retrievedDriver = await context.Drivers.FindAsync(newDriver.Id);
        Assert.That(retrievedDriver, Is.Not.Null);
        Assert.That(retrievedDriver!.Name, Is.EqualTo("Anna Pilot"));
    }

    [Test]
    public async Task AddTruck_ShouldBeSavedAndDiscriminatedCorrectly()
    {
        using var context = CreateContext();

        var newTruck = new Truck
        {
            RegistrationNumber = "TRK001",
            MaxLoadKg = 25000,
            TrailerLength = 13.6
        };

        context.Vehicles.Add(newTruck);
        await context.SaveChangesAsync();

        Assert.That(newTruck.Id, Is.GreaterThan(0), "ID Trucka powinno być wygenerowane.");

        // Sprawdzenie, czy EF Core poprawnie zrekonstruował obiekt jako Truck
        var retrievedVehicle = await context.Vehicles.FindAsync(newTruck.Id);
        Assert.That(retrievedVehicle, Is.Not.Null);
        Assert.That(retrievedVehicle, Is.InstanceOf<Truck>(), "Pobrany obiekt powinien być typu Truck.");

        // Sprawdzenie pola specyficznego dla Truck
        var retrievedTruck = (Truck)retrievedVehicle!;
        Assert.That(retrievedTruck.TrailerLength, Is.EqualTo(13.6));
    }

    [Test]
    public async Task AddVan_ShouldBeSavedAndDiscriminatedCorrectly()
    {
        using var context = CreateContext();

        var newVan = new Van
        {
            RegistrationNumber = "VAN007",
            MaxLoadKg = 1500,
            CargoVolume = 12.5
        };

        context.Vehicles.Add(newVan); // Dodajemy do DbSet<Vehicle>
        await context.SaveChangesAsync();

        Assert.That(newVan.Id, Is.GreaterThan(0), "ID Vana powinno być wygenerowane.");

        // Sprawdzenie, czy EF Core poprawnie zrekonstruował obiekt jako Van
        var retrievedVehicle = await context.Vehicles.FindAsync(newVan.Id);
        Assert.That(retrievedVehicle, Is.Not.Null);
        Assert.That(retrievedVehicle, Is.InstanceOf<Van>(), "Pobrany obiekt powinien być typu Van.");

        // Sprawdzenie pola specyficznego dla Van
        var retrievedVan = (Van)retrievedVehicle!;
        Assert.That(retrievedVan.CargoVolume, Is.EqualTo(12.5));
    }
    
    [Test]
    public async Task CreateOrderAsync_ReservesResourcesAndSaves_Success()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var (driverId, vehicleId) = await SeedDataAsync(context);

        var newOrder = new TransportOrder { DriverId = driverId, VehicleId = vehicleId, CargoDescription = "Testowe Zlecenie" };

        // ACT
        var createdOrder = await manager.CreateOrderAsync(newOrder);

        // ASSERT
        Assert.That(createdOrder, Is.Not.Null);
        Assert.That(createdOrder.Id, Is.GreaterThan(0));

        // Sprawdzenie, czy zasoby zostały zajęte (IsAvailable = false)
        var reservedVehicle = await context.Vehicles.FindAsync(vehicleId);
        var reservedDriver = await context.Drivers.FindAsync(driverId);

        Assert.That(reservedVehicle!.IsAvailable, Is.False, "Pojazd powinien być zajęty.");
        Assert.That(reservedDriver!.IsAvailable, Is.False, "Kierowca powinien być zajęty.");
    }

    [Test]
    public void CreateOrderAsync_UnavailableVehicle_ThrowsException()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var (driverId, vehicleId) = SeedDataAsync(context).Result;

        // Ustawienie pojazdu jako niedostępnego ręcznie przed próbą utworzenia zlecenia
        context.Vehicles.Find(vehicleId)!.IsAvailable = false;
        context.SaveChanges();

        var newOrder = new TransportOrder { DriverId = driverId, VehicleId = vehicleId, CargoDescription = "Test" };

        // ASSERT
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await manager.CreateOrderAsync(newOrder);
        }, "Oczekiwano wyjątku, gdy pojazd jest niedostępny.");
    }

    [Test]
    public void CreateOrderAsync_UnavailableDriver_ThrowsException()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var (driverId, vehicleId) = SeedDataAsync(context).Result;

        // Ustawienie kierowcy jako niedostępnego ręcznie
        context.Drivers.Find(driverId)!.IsAvailable = false;
        context.SaveChanges();

        var newOrder = new TransportOrder { DriverId = driverId, VehicleId = vehicleId, CargoDescription = "Test" };

        // ASSERT
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await manager.CreateOrderAsync(newOrder);
        }, "Oczekiwano wyjątku, gdy kierowca jest niedostępny.");
    }

    [Test]
    public async Task CompleteOrderAsync_MarksOrderAsCompletedAndReleasesResources_Success()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var (driverId, vehicleId) = await SeedDataAsync(context);

        // 1. Utworzenie zlecenia, które zajmie zasoby
        var orderToComplete = await manager.CreateOrderAsync(new TransportOrder { DriverId = driverId, VehicleId = vehicleId, CargoDescription = "Do Zakończenia" });

        // ACT
        await manager.CompleteOrderAsync(orderToComplete.Id);

        // ASSERT
        // Sprawdzenie stanu zlecenia
        var completedOrder = await context.TransportOrders.FindAsync(orderToComplete.Id);
        Assert.That(completedOrder!.IsCompleted, Is.True, "Zlecenie powinno być oznaczone jako zakończone.");

        // Sprawdzenie zwolnienia zasobów (zostają ponownie IsAvailable = true)
        var releasedVehicle = await context.Vehicles.FindAsync(vehicleId);
        var releasedDriver = await context.Drivers.FindAsync(driverId);

        Assert.That(releasedVehicle!.IsAvailable, Is.True, "Pojazd powinien być zwolniony.");
        Assert.That(releasedDriver!.IsAvailable, Is.True, "Kierowca powinien być zwolniony.");
    }

    [Test]
    public void CompleteOrderAsync_OrderNotFound_ThrowsException()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);

        // ACT & ASSERT
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await manager.CompleteOrderAsync(999); // Nieistniejące ID
        }, "Oczekiwano wyjątku InvalidOperationException, gdy zlecenie nie istnieje.");
    }
    
    [Test]
    public async Task CreateOrderAsync_FiresOnNewOrderCreatedEvent()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var (driverId, vehicleId) = await SeedDataAsync(context);

        string? receivedMessage = null;
        
        manager.OnNewOrderCreated += (message) => receivedMessage = message; 

        var newOrder = new TransportOrder { DriverId = driverId, VehicleId = vehicleId, CargoDescription = "Test Zdarzenia" };

        await manager.CreateOrderAsync(newOrder);

        // 1. Sprawdzenie, czy zdarzenie faktycznie zostało wywołane
        Assert.That(receivedMessage, Is.Not.Null, "Zdarzenie OnNewOrderCreated powinno zostać wywołane.");
    }

    [Test]
    public async Task GetAvailableVehicles_ExtensionMethod_ReturnsOnlyAvailableVehicles()
    {
        using var context = CreateContext();

        var availableTruck = new Truck { RegistrationNumber = "AVA1", MaxLoadKg = 1000, IsAvailable = true };
        var unavailableVan = new Van { RegistrationNumber = "UNA2", MaxLoadKg = 500, IsAvailable = false };
        var availableVan = new Van { RegistrationNumber = "AVA3", MaxLoadKg = 700, IsAvailable = true };

        context.Vehicles.AddRange(availableTruck, unavailableVan, availableVan);
        await context.SaveChangesAsync();

        var availableVehicles = await context.Vehicles
            .GetAvailableVehicles()
            .ToListAsync();

        // 1. Sprawdzenie liczby: Powinny być tylko 2 dostępne pojazdy
        Assert.That(availableVehicles.Count, Is.EqualTo(2), "Metoda powinna zwrócić dokładnie 2 dostępne pojazdy.");
        
        // 2. Sprawdzenie logiki: Upewnienie się, że wszystkie zwrócone są faktycznie dostępne
        Assert.That(availableVehicles.All(v => v.IsAvailable), Is.True, "Wszystkie zwrócone pojazdy muszą mieć IsAvailable = true.");
        
        // 3. Sprawdzenie, czy ten niedostępny został odfiltrowany
        Assert.That(availableVehicles.Any(v => v.RegistrationNumber == "UNA2"), Is.False, "Niedostępny pojazd nie powinien znaleźć się na liście.");
    }
}