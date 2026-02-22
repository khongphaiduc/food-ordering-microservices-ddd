using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Services
{
    public interface IRevenue
    {
        Task<RevenueDashboardResponse> Excute(GetRevenueDashboardRequest request);
    }
}
