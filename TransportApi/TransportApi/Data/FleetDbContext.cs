using Microsoft.EntityFrameworkCore;
using TransportApi.Models;

namespace TransportApi.Data;

public class FleetDbContext : DbContext
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Truck> Trucks { get; set; } = null!;
    public DbSet<Van> Vans { get; set; } = null!;
    public DbSet<Driver> Drivers { get; set; } = null!;
    public DbSet<TransportOrder> TransportOrders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TPH mapping for Vehicle
        modelBuilder.Entity<Vehicle>()
            .HasDiscriminator<string>("VehicleType")
            .HasValue<Truck>(nameof(Truck))
            .HasValue<Van>(nameof(Van));

        // relationship config
        modelBuilder.Entity<TransportOrder>()
            .HasOne(o => o.Vehicle)
            .WithMany()
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransportOrder>()
            .HasOne(o => o.Driver)
            .WithMany()
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}