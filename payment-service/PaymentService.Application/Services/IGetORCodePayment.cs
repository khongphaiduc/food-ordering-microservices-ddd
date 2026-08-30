using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace payment_service.PaymentService.Application.Services
{
    public interface IGetORCodePayment
    {
        Task<ORCodeDto> GetORCodePaymentAsync(string code , CancellationToken token);
    }



    public class ORCodeDto
    {
        public Guid IdPayment { get; set; }

        public string QRCode { get; set; } = null!;

    }
}
