using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Services
{
    public interface IGetListOrderOfUser
    {
        Task<OrderHistoryPagination> GetListOrderForUser(RequestGetListOrderWithPagination request);
    }
}
