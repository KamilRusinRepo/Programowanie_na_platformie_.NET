namespace TransportApi.Models;

public class Truck : Vehicle, IReservable
{
    public double TrailerLength { get; set; }

    public override string GetInfo() =>
        $"Truck {RegistrationNumber} - MaxLoad: {MaxLoadKg}kg, TrailerLength: {TrailerLength}m, Available: {IsAvailable}";
}