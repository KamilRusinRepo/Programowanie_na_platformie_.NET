namespace TransportApi.Models;

public class Van : Vehicle, IReservable
{
    public double CargoVolume { get; set; }

    public override string GetInfo() =>
        $"Van {RegistrationNumber} - MaxLoad: {MaxLoadKg}kg, Volume: {CargoVolume}m3, Available: {IsAvailable}";
}