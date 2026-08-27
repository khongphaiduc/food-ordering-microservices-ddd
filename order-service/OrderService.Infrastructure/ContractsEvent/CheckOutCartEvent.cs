namespace Foodly.Contracts.Events
{
    public class CheckOutCartEvent
    {
        public Guid IdCart
        {
            get; set;
        }
        public Guid IdUser { get; set; }
    }
}
