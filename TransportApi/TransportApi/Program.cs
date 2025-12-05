using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Dto;
using TransportApi.Extensions;
using TransportApi.Models;
using TransportApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (SQLite file)
builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseSqlite("Data Source=fleet.db"));

// Add FleetManager
builder.Services.AddScoped<FleetManager>();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure DB created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
    db.Database.EnsureCreated();
}

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Vehicles endpoints
app.MapGet("/api/vehicles", async (FleetDbContext db) =>
    Results.Ok(await db.Vehicles.ToListAsync()));

app.MapPost("/api/vehicles", async (FleetDbContext db, CreateVehicleDto dto) =>
{
    Vehicle vehicle = dto.VehicleType.ToLower() switch
    {
        "truck" => new Truck 
        {
            RegistrationNumber = dto.RegistrationNumber,
            MaxLoadKg = dto.MaxLoadKg,
            TrailerLength = dto.TrailerLength ?? 0
        },

        "van" => new Van
        {
            RegistrationNumber = dto.RegistrationNumber,
            MaxLoadKg = dto.MaxLoadKg,
            CargoVolume = dto.CargoVolume ?? 0
        },

        _ => null!
    };

    if (vehicle == null)
        return Results.BadRequest("Unknown vehicle type. Use 'truck' or 'van'.");

    db.Vehicles.Add(vehicle);
    await db.SaveChangesAsync();
    return Results.Created($"/api/vehicles/{vehicle.Id}", vehicle);
});

// Drivers endpoints
app.MapGet("/api/drivers", async (FleetDbContext db) =>
    Results.Ok(await db.Drivers.ToListAsync()));

app.MapPost("/api/drivers", async (FleetDbContext db, Driver driver) =>
{
    db.Drivers.Add(driver);
    await db.SaveChangesAsync();
    return Results.Created($"/api/drivers/{driver.Id}", driver);
});

// Orders endpoints
app.MapGet("/api/orders", async (FleetDbContext db) =>
    Results.Ok(await db.TransportOrders
        .Include(o => o.Driver)
        .Include(o => o.Vehicle)
        .Active()
        .ToListAsync()));

app.MapPost("/api/orders", async (FleetDbContext db, FleetManager manager, TransportOrder order) =>
{
    try
    {
        var created = await manager.CreateOrderAsync(order);
        return Results.Created($"/api/orders/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/orders/{id:int}/complete", async (int id, FleetManager manager) =>
{
    try
    {
        await manager.CompleteOrderAsync(id);
        return Results.NoContent();
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

// small health endpoint
app.MapGet("/", () => "TransportApi up");

app.Run();