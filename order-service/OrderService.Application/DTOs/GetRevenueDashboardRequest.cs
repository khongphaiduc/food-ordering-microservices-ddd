namespace order_service.OrderService.Application.DTOs
{
    public class GetRevenueDashboardRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public RevenueCompareType CompareType { get; set; }
    }

    public enum RevenueCompareType
    {
        None = 0,
        Yesterday = 1,
        LastWeek = 2,
        LastMonth = 3
    }
}
