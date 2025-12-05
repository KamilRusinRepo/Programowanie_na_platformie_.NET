using TransportApi.Models;

namespace TransportApi.Extensions;

public static class VehicleExtensions
{
    public static IQueryable<Vehicle> GetAvailableVehicles(this IQueryable<Vehicle> query)
    {
        return query.Where(v => v.IsAvailable);
    }
}