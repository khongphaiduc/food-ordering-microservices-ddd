using order_service.OrderService.Domain.Entities;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Domain.ObjectValue;
using order_service.OrderService.Infrastructure.ServicesImplements;

namespace order_service.OrderService.Domain.Aggregate
{
    public class OrdersAggregate
    {
        public Guid IdOrder { get; private set; }
        public Guid IdCustomer { get; private set; }

        public Guid IdCart { get; private set; }

        public OrderStatusPayment StatusOrderPayment { get; private set; }

        public Price TotalAmount { get; private set; }
        public decimal ShippingFee { get; private set; }      // phí giao hàng
        public DiscountValue Discount { get; private set; }   // gi?m giá
        public Price FinalAmount { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }


        public OrderStatus OrderStatus { get; private set; }

        public string SnapshotNameCustomer { get; private set; }

        public string SnapshotPhoneCustomer { get; private set; }


        private readonly List<OrderItemsEntity> orderItemsEntities = new();
        public IReadOnlyList<OrderItemsEntity> OrderItemsEntities => orderItemsEntities.AsReadOnly();

        private readonly List<OrderPaymentsEntity> orderPaymentsEntities = new();
        public IReadOnlyList<OrderPaymentsEntity> OrderPaymentsEntities => orderPaymentsEntities.AsReadOnly();


        public OrderDeliveryEntity? Delivery { get; private set; }


        public static OrdersAggregate CreateNewOrder(Guid IdCart, Guid IdCustomer, OrderStatusPayment statusOrder, decimal ShippingFee, decimal Discount, PaymentMethod paymentMethod, string SnapshotNameCustomer, string SnapshotPhoneCustomer)
        {
            return new OrdersAggregate
            {
                IdOrder = Guid.NewGuid(),
                IdCart = IdCart,
                IdCustomer = IdCustomer,
                StatusOrderPayment = statusOrder,
                TotalAmount = new Price(0),
                ShippingFee = ShippingFee,
                Discount = new DiscountValue(Discount),
                FinalAmount = new Price(0),
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SnapshotNameCustomer = SnapshotNameCustomer,
                SnapshotPhoneCustomer = SnapshotPhoneCustomer
            };
        }

        internal OrdersAggregate(Guid idOrder, Guid idCustomer, Guid idCart, OrderStatusPayment status, Price totalAmount, decimal shippingFee, DiscountValue discount, Price finalAmount, PaymentMethod paymentMethod, DateTime createdAt, DateTime updatedAt, List<OrderItemsEntity> orderItemsEntities, List<OrderPaymentsEntity> orderPaymentsEntities, OrderDeliveryEntity? delivery)
        {
            IdOrder = idOrder;
            IdCustomer = idCustomer;
            IdCart = idCart;
            StatusOrderPayment = status;
            TotalAmount = totalAmount;
            ShippingFee = shippingFee;
            Discount = discount;
            FinalAmount = finalAmount;
            PaymentMethod = paymentMethod;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            this.orderItemsEntities = orderItemsEntities;
            this.orderPaymentsEntities = orderPaymentsEntities;

        }

        private OrdersAggregate()
        {
        }

        #region Business Methods

        public void AddOrderItem(OrderItemsEntity orderItem)
        {
            if (StatusOrderPayment != OrderStatusPayment.PENDING) throw new InvalidOperationException("Không th? thêm item khi order không ? tr?ng thái Pending");
            orderItemsEntities.Add(orderItem);
            RecalculateAmount();
        }

        public void AddOrderPayment(OrderPaymentsEntity orderPayment)
        {
            if (StatusOrderPayment == OrderStatusPayment.CANCELLED) throw new InvalidOperationException("Order dã b? h?y, không th? thanh toán");
            orderPaymentsEntities.Add(orderPayment);
        }

        public void SetDelivery(OrderDeliveryEntity delivery)
        {

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrderStatusPayment(OrderStatusPayment newStatus)
        {
            StatusOrderPayment = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }


        public void SetDiscount(decimal discount)
        {
            if (StatusOrderPayment != OrderStatusPayment.PENDING) throw new InvalidOperationException("Không th? c?p nh?t gi?m giá khi order không ? tr?ng thái Pending");
            Discount = new DiscountValue(discount);
            RecalculateAmount();
        }

        #endregion


        #region Private Helpers

        public void RecalculateAmount()
        {
            var total = orderItemsEntities.Sum(i => i.TotalPrice.Value);

            TotalAmount = new Price(total);
            FinalAmount = new Price(total + ShippingFee - Discount.Value);
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        public void AddDelivery(OrderDeliveryEntity delivery)
        {
            if (StatusOrderPayment == OrderStatusPayment.CANCELLED) throw new InvalidOperationException("Order dã b? h?y, không th? thêm thông tin giao hàng");
            Delivery = delivery;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrderStatus(OrderStatus status)
        {
            OrderStatus = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
