using TransportApi.Models;

namespace TransportApi.Extensions;

public static class TransportOrderExtension
{
    public static IQueryable<TransportOrder> Active(this IQueryable<TransportOrder> query)
    {
        return query.Where(o => !o.IsCompleted);
    }
}