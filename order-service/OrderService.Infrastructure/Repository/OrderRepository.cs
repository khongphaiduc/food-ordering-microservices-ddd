using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Domain.Aggregate;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infrastructure.Models;


namespace order_service.OrderService.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FoodOrderContext _db;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(FoodOrderContext foodOrderContext, ILogger<OrderRepository> logger)
        {
            _db = foodOrderContext;
            _logger = logger;
        }

        #region Create new order 
        public async Task<ResponseCreateNewOrder> CreateNewOrder(OrdersAggregate NewOrderAggregate)
        {

            try
            {
                _logger.LogInformation("Start Create New Order");
                var OrderBase = new Order
                {
                    Id = NewOrderAggregate.IdOrder,
                    CartId = NewOrderAggregate.IdCart,
                    CreatedAt = NewOrderAggregate.CreatedAt,
                    DiscountAmount = NewOrderAggregate.Discount.Value,
                    FinalAmount = NewOrderAggregate.FinalAmount.Value,
                    PaymentMethod = NewOrderAggregate.PaymentMethod.ToString(),
                    ShippingFee = NewOrderAggregate.ShippingFee,
                    Status = NewOrderAggregate.StatusOrderPayment.ToString(),
                    TotalAmount = NewOrderAggregate.TotalAmount.Value,
                    OrderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")).ToString(),
                    OrderStatus = "PENDING",
                    UpdatedAt = NewOrderAggregate.UpdatedAt,
                    UserId = NewOrderAggregate.IdCustomer,
                    OrderItems = NewOrderAggregate.OrderItemsEntities.Select(s => new OrderItem
                    {
                        Id = s.IdOrderItems,
                        OrderId = NewOrderAggregate.IdOrder,
                        ProductId = s.IdProduct,
                        ProductName = s.ProductName,
                        VariantId = s.IdVariant,
                        VariantName = s.VariantName,
                        Price = s.UnitPrice.Value,
                        Quantity = s.Quantity.Value,
                        TotalPrice = s.TotalPrice.Value,
                        OptionsSnapshot = s.OptionsSnapshot,
                    }).ToList(),

                    OrderPayments = NewOrderAggregate.OrderPaymentsEntities.Select(p => new OrderPayment
                    {
                        Id = p.IdOrderPayment,
                        OrderId = NewOrderAggregate.IdOrder,
                        PaymentProvider = p.PaymentProvider.ToString(),
                        Amount = p.Amount.Value,
                        Status = p.PaymentStatus.ToString(),
                        TransactionId = p.TransactionId,
                        CreatedAt = p.CreatedAt,
                        PaidAt = p.PaidAt,

                    }).ToList(),

                    OrderDelivery = new OrderDelivery
                    {
                        OrderId = NewOrderAggregate.IdOrder,
                        Address = NewOrderAggregate.Delivery.Address.Value,
                        Note = NewOrderAggregate.Delivery?.Note,
                        Phone = NewOrderAggregate.Delivery?.PhoneNumer.Value ?? "Not identity",
                        ReceiverName = NewOrderAggregate.Delivery?.ReciverName ?? "User not identity"
                    }
                    ,
                    SnapshotNameCustomer = NewOrderAggregate.SnapshotNameCustomer,
                    SnapshotPhone = NewOrderAggregate.SnapshotPhoneCustomer,
                };

                await _db.Orders.AddAsync(OrderBase);

                await _db.SaveChangesAsync();
                _logger.LogInformation("Create New Order staged successfully");

                return new ResponseCreateNewOrder
                {
                    IdOrder = OrderBase.Id,
                    OrderCode = OrderBase.OrderCode,
                    FinalAmount = OrderBase.FinalAmount,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new order");
                return new ResponseCreateNewOrder
                {
                    IdOrder = Guid.Empty,
                    Status = false,
                    FinalAmount = 0,
                    OrderCode = string.Empty
                };
            }

        }
        #endregion end create new order 



        #region update order
        public async Task UpdateOrder(OrdersAggregate NewOrderAggregate)
        {

            var OrderBase = await _db.Orders.Include(s => s.OrderPayments).Include(s => s.OrderItems).Include(s => s.OrderDelivery).FirstOrDefaultAsync(s => s.Id == NewOrderAggregate.IdOrder);

            if (OrderBase == null) return;

            OrderBase.Status = NewOrderAggregate.StatusOrderPayment.ToString();
            OrderBase.TotalAmount = NewOrderAggregate.TotalAmount.Value;
            OrderBase.ShippingFee = NewOrderAggregate.ShippingFee;
            OrderBase.DiscountAmount = NewOrderAggregate.Discount.Value;
            OrderBase.FinalAmount = NewOrderAggregate.FinalAmount.Value;
            OrderBase.PaymentMethod = NewOrderAggregate.PaymentMethod.ToString();
            OrderBase.UpdatedAt = NewOrderAggregate.UpdatedAt;

            //order item 


            var OrderItemBaseID = OrderBase.OrderItems.Select(s => s.Id).ToHashSet();

            if (NewOrderAggregate.OrderItemsEntities != null && NewOrderAggregate.OrderItemsEntities.Any())
            {
                foreach (var orderItem in NewOrderAggregate.OrderItemsEntities)
                {
                    if (OrderItemBaseID.Contains(orderItem.IdOrderItems))
                    {
                        // update order item
                        var existingOrderItem = OrderBase.OrderItems.FirstOrDefault(s => s.Id == orderItem.IdOrderItems);
                        if (existingOrderItem != null)
                        {
                            existingOrderItem.ProductId = orderItem.IdProduct;
                            existingOrderItem.ProductName = orderItem.ProductName;
                            existingOrderItem.VariantId = orderItem.IdVariant;
                            existingOrderItem.VariantName = orderItem.VariantName;
                            existingOrderItem.Price = orderItem.UnitPrice.Value;
                            existingOrderItem.Quantity = orderItem.Quantity.Value;
                            existingOrderItem.TotalPrice = orderItem.TotalPrice.Value;
                            existingOrderItem.OptionsSnapshot = orderItem.OptionsSnapshot;
                        }

                    }
                    else
                    {
                        // add new order item
                        OrderBase.OrderItems.Add(new OrderItem
                        {
                            Id = orderItem.IdOrderItems,
                            OrderId = NewOrderAggregate.IdOrder,
                            ProductId = orderItem.IdProduct,
                            ProductName = orderItem.ProductName,
                            VariantId = orderItem.IdVariant,
                            VariantName = orderItem.VariantName,
                            Price = orderItem.UnitPrice.Value,
                            Quantity = orderItem.Quantity.Value,
                            TotalPrice = orderItem.TotalPrice.Value,
                            OptionsSnapshot = orderItem.OptionsSnapshot,
                        });

                    }
                }


                // payments
                foreach (var payment in NewOrderAggregate.OrderPaymentsEntities)
                {
                    var existingPayment = OrderBase.OrderPayments
                        .FirstOrDefault(p => p.TransactionId == payment.TransactionId);

                    if (existingPayment == null)
                    {
                        // thanh toán m?i 
                        OrderBase.OrderPayments.Add(new OrderPayment
                        {
                            Id = payment.IdOrderPayment,
                            OrderId = NewOrderAggregate.IdOrder,
                            PaymentProvider = payment.PaymentProvider.ToString(),
                            Amount = payment.Amount.Value,
                            Status = payment.PaymentStatus.ToString(),
                            TransactionId = payment.TransactionId,
                            CreatedAt = payment.CreatedAt,
                            PaidAt = payment.PaidAt
                        });
                    }
                    else
                    {
                        // CÙNG GIAO D?CH -> UPDATE TR?NG THÁI
                        existingPayment.Status = payment.PaymentStatus.ToString();
                        existingPayment.PaidAt = payment.PaidAt;
                    }
                }


                await _db.SaveChangesAsync();
            }
        }
        #endregion end update order
    }
}
