using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace order_service.OrderService.Infastructure.OrderRealTime
{
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public class NotificationOrderHUB : Hub
    {

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort();
                return;
            }

            if (role == "Admin")
                await Groups.AddToGroupAsync(Context.ConnectionId, "ADMIN_GROUP");

            if (role == "Staff")
                await Groups.AddToGroupAsync(Context.ConnectionId, "STAFF_GROUP");

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
