namespace TransportApi.Dto;

public class CreateVehicleDto
{
    public string VehicleType { get; set; } = string.Empty; // "truck" lub "van"
    public string RegistrationNumber { get; set; } = string.Empty;
    public double MaxLoadKg { get; set; }

    public double? TrailerLength { get; set; }
    public double? CargoVolume { get; set; }
}