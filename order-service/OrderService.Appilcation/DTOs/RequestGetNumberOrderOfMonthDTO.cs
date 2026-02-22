namespace order_service.OrderService.Appilcation.DTOs
{
    public class RequestGetNumberOrderOfMonthDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<OrderByDayDTO> Data { get; set; }
    }

    public class OrderByDayDTO
    {
        public int Day { get; set; }
        public int OrderCount { get; set; }
    }

    public class GetOrderByMonthRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

}
