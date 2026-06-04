namespace order_service.OrderService.Application.DTOs
{
    public class ResponseOrderInformations
    {
        public Guid IdOrder { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public decimal ShippingFee { get; set; }    

        public string Status { get; set; } = string.Empty;

        public List<ResponseOrderItemInformations> OrderItems { get; set; } = new List<ResponseOrderItemInformations>();

    }


    public class ResponseOrderItemInformations
    {
        public string ProductName { get; set; } = null!;
        public string? VariantName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }





}
