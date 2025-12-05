namespace TransportApi.Models;

public class TransportOrder
{
    public int Id { get; set; }
    public string CargoDescription { get; set; } = string.Empty;
    public double Weight { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsCompleted { get; set; } = false;

    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public int DriverId { get; set; }
    public Driver? Driver { get; set; }
}