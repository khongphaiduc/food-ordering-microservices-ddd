using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Services
{
    public interface IGetPreFitOfMonths
    {
        Task<GetPreFitOfMonthDTO> Execute(RequestGetPrefitOfMonthDTO requets);
    }
}
