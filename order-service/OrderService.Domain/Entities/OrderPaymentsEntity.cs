using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Domain.ObjectValue;

namespace order_service.OrderService.Domain.Entities
{
    public class OrderPaymentsEntity
    {
        public Guid IdOrderPayment { get; private set; }

        public Guid IdOrder { get; private set; }


        public PaymentMethod PaymentProvider { get; private set; }


        public PaymentStatus PaymentStatus { get; private set; }

        public Price Amount { get; private set; }


        public string? TransactionId { get; private set; }  // Mã giao d?ch do c?ng thanh toán tr? v?


        public DateTime? PaidAt { get; private set; }// Th?i di?m ngân hàng/c?ng thanh toán xác nh?n dã thanh toán


        public DateTime CreatedAt { get; private set; }

        #region Factory

        public static OrderPaymentsEntity CreateOrderPayment(Guid idOrder, PaymentMethod PaymentProvider, PaymentStatus paymentStatus, decimal amount, string? transactionId, DateTime? paidAt = null
        )
        {
            return new OrderPaymentsEntity(
                Guid.NewGuid(),
                idOrder,
                PaymentProvider,
                paymentStatus,
                new Price(amount),
                transactionId,
                paidAt,
                DateTime.UtcNow
            );
        }

        #endregion

        #region Business Methods

        public void MarkAsPaid(DateTime paidAt)
        {
            PaymentStatus = PaymentStatus.SUCCESS;
            PaidAt = paidAt;
        }

        public void MarkAsFailed()
        {
            PaymentStatus = PaymentStatus.FAILED;
            PaidAt = null;
        }

        #endregion

        #region Constructors

        private OrderPaymentsEntity(Guid idOrderPayment, Guid idOrder, PaymentMethod PaymentProvider, PaymentStatus paymentStatus, Price amount, string? transactionId, DateTime? paidAt, DateTime createdAt)

        {
            IdOrderPayment = idOrderPayment;
            IdOrder = idOrder;
            this.PaymentProvider = PaymentProvider;
            PaymentStatus = paymentStatus;
            Amount = amount;
            TransactionId = transactionId;
            PaidAt = paidAt;
            CreatedAt = createdAt;
        }


        private OrderPaymentsEntity()
        {
        }

        #endregion
    }
}
