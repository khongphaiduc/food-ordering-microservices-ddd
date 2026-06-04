using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Services
{
    public interface IRevenue
    {
        Task<RevenueDashboardResponse> Execute(GetRevenueDashboardRequest request);
    }
}
