namespace TransportApi.Models;

public interface IReservable
{
    void StartOrder();
    void CompleteOrder();
}