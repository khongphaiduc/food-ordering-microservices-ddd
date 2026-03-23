namespace tracking_service.Tracking.Domain.Enums
{
    public enum EventType
    {

        ViewProduct = 1,       // User view detail product  

        SearchProduct = 2,     // User search product

        AddToCart = 3,         // User add to cart 

        RemoveFromCart = 4,    // User abandon cart 

        CompleteCheckout = 5, // User purchase successfully

        Login = 6,            // User login 

        Logout = 7,           // User  logout
    }
}
