namespace order_service.OrderService.Application.DTOs
{
    public class RevenueDashboardResponse
    {
        // T?ng doanh thu c?a kho?ng hi?n t?i
        public decimal CurrentRevenue { get; set; }

        // T?ng doanh thu c?a kho?ng so sánh
        public decimal CompareRevenue { get; set; }

        // Chênh l?ch gi?a 2 kho?ng
        public decimal RevenueDifference { get; set; }

        // % tang/gi?m
        public decimal RevenueGrowthPercent { get; set; }

        // T?ng s? don (n?u mu?n thêm insight)
        public int CurrentOrderCount { get; set; }  // don c?a hi?n t?i 

        public int CompareOrderCount { get; set; }  // don c?a quá kh? 

        public decimal PercentComplation { get; set; }  //t? l? hoàn thành don
    }
}
