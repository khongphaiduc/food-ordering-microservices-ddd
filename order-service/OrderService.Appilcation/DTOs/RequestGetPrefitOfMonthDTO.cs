namespace order_service.OrderService.Appilcation.DTOs
{
    public class RequestGetPrefitOfMonthDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class GetPreFitOfMonthDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<PrefitByDayDTO> Data { get; set; }
    }

    public class PrefitByDayDTO
    {
        public int Day { get; set; }
        public int Amount { get; set; }
    }


}
