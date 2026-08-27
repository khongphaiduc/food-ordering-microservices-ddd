using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace order_service.OrderService.Infrastructure.OrderRealTime
{
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public class OrderOfUser : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
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
