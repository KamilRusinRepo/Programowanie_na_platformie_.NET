using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;

namespace TransportApi.Services;

public class FleetManager
{
    private readonly FleetDbContext _db;

    public FleetManager(FleetDbContext db)
    {
        _db = db;
    }

    // Event fired when new order is created
    public event Action<string>? OnNewOrderCreated;

    public async Task<TransportOrder> CreateOrderAsync(TransportOrder order)
    {
        // validate availability
        var vehicle = await _db.Vehicles.FindAsync(order.VehicleId);
        if (vehicle == null || !vehicle.IsAvailable) throw new InvalidOperationException("Vehicle not available");

        var driver = await _db.Drivers.FindAsync(order.DriverId);
        if (driver == null || !driver.IsAvailable) throw new InvalidOperationException("Driver not available");

        // reserve
        vehicle.StartOrder();
        driver.StartOrder();

        _db.TransportOrders.Add(order);
        await _db.SaveChangesAsync();

        OnNewOrderCreated?.Invoke($"Order {order.Id} created for vehicle {vehicle.RegistrationNumber} and driver {driver.Name}");
        return order;
    }

    public async Task CompleteOrderAsync(int orderId)
    {
        var order = await _db.TransportOrders.Include(o => o.Driver).Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) throw new InvalidOperationException("Order not found");
        if (order.IsCompleted) return;

        order.IsCompleted = true;
        if (order.Vehicle != null) order.Vehicle.CompleteOrder();
        if (order.Driver != null) order.Driver.CompleteOrder();

        await _db.SaveChangesAsync();
    }
}