using cart_service.CartService.Infrastructure.Models;

namespace cart_service.CartService.Application.DTOs
{
    public class RequestUpdateCartFood
    {
        public Guid IdCart { get; set; }
        public List<CartItemrDTO>? CartItems { get; set; } = new();
    }


    public partial class CartItemrDTO
    {
        public Guid IdCartItem { get; set; }

        public Guid ProductId { get; set; }

        public Guid? VariantId { get; set; }

        public int Quantity { get; set; }   // quantity  0 => xóa     , quantity > 0 => add / update
    }

}
