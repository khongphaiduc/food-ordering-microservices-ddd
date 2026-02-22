using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Services
{
    public interface IGetPreFitOfMonths
    {
        Task<GetPreFitOfMonthDTO> Excute(RequestGetPrefitOfMonthDTO requets);
    }
}
