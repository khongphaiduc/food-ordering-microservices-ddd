using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace payment_service.PaymentService.Infrastructure.NotificationRealTimeSignalR
{
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public class ViewQRCodeOrder : Hub
    {
        public async override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId is NULL");
                Context.Abort();
                return;
            }

            Console.WriteLine($"User connected: {userId}");

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
