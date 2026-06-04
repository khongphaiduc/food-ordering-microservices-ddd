namespace order_service.OrderService.Application.DTOs
{
    public class RequestGetListOrderWithPagination
    {
        public Guid IdUser { get; set; }

        public int PageIndex { get; set; } = 1;

    }
}
