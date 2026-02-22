namespace order_service.OrderService.Appilcation.DTOs
{
    public class RevenueDashboardResponse
    {
        // Tổng doanh thu của khoảng hiện tại
        public decimal CurrentRevenue { get; set; }

        // Tổng doanh thu của khoảng so sánh
        public decimal CompareRevenue { get; set; }

        // Chênh lệch giữa 2 khoảng
        public decimal RevenueDifference { get; set; }

        // % tăng/giảm
        public decimal RevenueGrowthPercent { get; set; }

        // Tổng số đơn (nếu muốn thêm insight)
        public int CurrentOrderCount { get; set; }  // đơn của hiện tại 

        public int CompareOrderCount { get; set; }  // đơn của quá khứ 

        public decimal PercentComplation { get; set; }  //tỉ lệ hoàn thành đơn
    }
}
