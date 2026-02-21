using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Appilcation.DTOs.DTOsInternal
{
    public class CartDTOsInternal
    {
        public Guid CartId { get; set; }

        public Guid UserId { get; set; }

        public OrderStatusPayment Status { get; set; } 

        public long TotalPrice { get; set; }

        public List<CartItemDTOsInternal> CartItems { get; set; } = new();
        public List<CartDiscountDTOsInternal> CartDiscounts { get; set; } = new();
    }



    public class CartItemDTOsInternal
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public Guid? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public long UnitPrice { get; set; }
        public long TotalPrice { get; set; }
    }


    public class CartDiscountDTOsInternal
    {
        public Guid CartId { get; set; }
        public string DiscountCode { get; set; } = null!;
        public long DiscountAmount { get; set; }
        public long TotalDiscountAmount { get; set; }

    }
}
