namespace TransportApi.Models;

public abstract class Vehicle : IReservable
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public double MaxLoadKg { get; set; }
    public bool IsAvailable { get; set; } = true;

    public abstract string GetInfo();
    public void StartOrder() => IsAvailable = false;
    public void CompleteOrder() => IsAvailable = true;
}