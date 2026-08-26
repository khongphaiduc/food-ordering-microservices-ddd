using payment_service.PaymentService.Domain.Enums;

namespace payment_service.PaymentService.Application.Services
{
    public interface IPaymentRepository
    {
        Task<bool> CreatePayment(CreatePaymentPayload request);

        Task<bool> UpdateStatusPayment(string OrderCode, PaymentStatus status);
    }



    public class CreatePaymentPayload
    {
        public string OrderCode { get; set; }

        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public PaymentMethod PaymentMethods { get; set; }

        public string Provider { get; set; } = null!;
        
        public string QRCode { get; set; } 
        public string TransactionId { get; set; } = null!;

    }


}
