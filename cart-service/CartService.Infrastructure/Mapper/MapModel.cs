using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Domain.Interface;
using cart_service.CartService.Infrastructure.Models;

namespace cart_service.CartService.Infrastructure.Mapper
{
    public class MapModel : IMapModel
    {
        public Cart MapAggregateToCartModel(CartAggregate cartAggregate)
        {
            return new Cart
            {
                Id = cartAggregate.Id,
                UserId = cartAggregate.UserId,
                Status = cartAggregate.Status,
                TotalPrice = cartAggregate.TotalPrice.Value,
                CreatedAt = cartAggregate.CreatedAt,
                UpdatedAt = cartAggregate.UpdatedAt,
                CartDiscounts = cartAggregate.CartDiscountList.Select(s => new CartDiscount
                {
                    Id = s.Id,
                    CartId = s.CartId,
                    Code = s.Code,
                    DiscountType = s.DiscountType,
                    DiscountValue = s.DiscountValue.Value,
                    AppliedAmount = s.AppliedAmount.Value,

                }).ToList(),


                CartItems = cartAggregate.CartItemList.Select(a => new CartItem
                {
                    Id = a.Id,
                    CartId = a.CartId,
                    ProductId = a.ProductId,
                    VariantId = a.VariantId,
                    ProductName = a.ProductName,
                    VariantName = a.VariantName,
                    UnitPrice = a.UnitPrice.Value,
                    Quantity = a.Quantitys.Value,
                    TotalPrice = a.TotalPrice.Value,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,

                }).ToList()

            };
        }
    }
}
