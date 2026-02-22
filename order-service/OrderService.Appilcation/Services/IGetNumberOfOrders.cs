using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Services
{
    public interface IGetNumberOfOrders
    {
        Task<RequestGetNumberOrderOfMonthDTO> Excute(GetOrderByMonthRequest request);
    }
}
