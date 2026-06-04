using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Interface
{
    public interface IUpdateStatusOrders
    {
        Task<bool> Execute(RequestUpdateStatusOrder request);
    }
}
