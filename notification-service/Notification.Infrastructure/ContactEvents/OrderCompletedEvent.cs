using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Foodly.Contracts.Events
{
    public class OrderCompletedEvent
    {

        public string OrderCode { get; set; }

        public string UserName { get; set; }

        public long TotalPrice { get; set; }

        public string Email { get; set; }

    }
}
