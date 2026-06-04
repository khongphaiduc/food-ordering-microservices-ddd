using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Services
{
    public interface IGetNumberOfOrders
    {
        Task<RequestGetNumberOrderOfMonthDTO> Execute(GetOrderByMonthRequest request);
    }
}
