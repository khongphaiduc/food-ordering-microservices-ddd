namespace Foodly.Contracts.Events
{
    public class RestoreStatusCartEvent
    {
        public Guid IdCart
        {
            get; set;
        }
        public Guid IdUser { get; set; }
    }
}
