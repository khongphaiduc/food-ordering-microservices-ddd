namespace Foodly.Contracts.Events
{


    public class PaySuccessfullyEvent
    {
        public string OrderCode { get; set; } = null!;

        public DateTime Happen { get; set; }
    }
}
