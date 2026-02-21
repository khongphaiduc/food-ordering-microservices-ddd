using System;
using System.Collections.Generic;

namespace order_service.OrderService.Infastructure.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CartId { get; set; }

    public string OrderCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string OrderStatus { get; set; } = "PENDING";

    public string SnapshotNameCustomer { get; set; } = "Customer";

    public string SnapshotPhone { get; set; } = "0987654321";

    public virtual OrderDelivery? OrderDelivery { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}
