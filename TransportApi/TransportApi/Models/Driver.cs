namespace TransportApi.Models;

public class Driver :  IReservable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    
    public void StartOrder() => IsAvailable = false;
    public void CompleteOrder() => IsAvailable = true;
}