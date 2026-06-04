using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Domain.Aggregate;

namespace order_service.OrderService.Domain.Interface
{
    public interface IOrderRepository
    {
        Task<ResponseCreateNewOrder> CreateNewOrder(OrdersAggregate orders);

        Task UpdateOrder(OrdersAggregate orders);

    }
}
