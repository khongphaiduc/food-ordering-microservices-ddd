namespace tracking_service.Tracking.Domain.Enums
{
    public enum EventType
    {
        ViewHomePage = 1,      // User click home page 

        ViewProduct = 2,       // User view detail product  

        ViewCategory = 3,      // User view category page

        SearchProduct = 4,     // User search product

        ClickProduct = 5,      // User click vào một sản phẩm từ danh sách

        AddToCart = 6,         // User add to cart 

        RemoveFromCart = 7,    // User abandon cart 

        ViewCart = 8,          // User open cart page

        StartCheckout = 9,     // User start purchase process

        CompleteCheckout = 10, // User purchase successfully

        Login = 11,            // User login 

        Logout = 12,           // User  logout

        Register = 13          // User register account
    }
}
