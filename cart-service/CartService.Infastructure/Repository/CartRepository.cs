using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Domain.Interface;
using cart_service.CartService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace cart_service.CartService.Infastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly FoodProductsDbContext _db;
        private readonly IMapModel _map;

        public CartRepository(FoodProductsDbContext foodProductsDbContext, IMapModel mapModel)
        {
            _db = foodProductsDbContext;
            _map = mapModel;
        }

        public async Task<Guid> CreateCartAsync(CartAggregate cartAggregate,CancellationToken cancellationToken)
        {
            var cart = _map.MapAggregateToCartModel(cartAggregate);

            _db.Carts.Add(cart);

            var result = await _db.SaveChangesAsync();

            return result > 0 ? cart.Id : Guid.Empty;
        }


        public Task<CartAggregate?> GetCartByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }


        public async Task UpdateCartAsync(CartAggregate cartNewAggregate)
        {
            var cartBase = await _db.Carts.Include(s => s.CartItems).Include(s => s.CartDiscounts).FirstOrDefaultAsync(s => s.Id == cartNewAggregate.Id);

            if (cartBase == null) return;


            if (cartNewAggregate.Status != null) cartBase.Status = cartNewAggregate.Status;

            if (cartNewAggregate.TotalPrice != null) cartBase.TotalPrice = cartNewAggregate.TotalPrice.Value;


            if (cartNewAggregate.CartItemList != null && cartNewAggregate.CartItemList.Any())
            {

                var existingCartItemIds = cartBase.CartItems.Select(s => s.Id).ToHashSet();

                foreach (var item in cartNewAggregate.CartItemList)
                {

                    if (!existingCartItemIds.Contains(item.Id))   // thêm mới  
                    {
                        cartBase.CartItems.Add(new CartItem
                        {
                            Id = item.Id,
                            CartId = cartBase.Id,
                            ProductId = item.ProductId,
                            VariantId = item.VariantId,
                            ProductName = item.ProductName,
                            VariantName = item.VariantName,
                            UnitPrice = item.UnitPrice.Value,
                            Quantity = item.Quantitys.Value,
                            TotalPrice = item.TotalPrice.Value,
                            CreatedAt = item.CreatedAt,
                            UpdatedAt = item.UpdatedAt
                        });
                    }
                    else                                       // update
                    {
                        if (item.Quantitys.Value == 0) continue;
                        
                        var cartItemUpdate = cartBase.CartItems.FirstOrDefault(s => s.Id == item.Id);

                        if (cartItemUpdate != null)
                        {
                            cartItemUpdate.ProductName = item.ProductName;
                            cartItemUpdate.VariantName = item.VariantName;
                            cartItemUpdate.UnitPrice = item.UnitPrice.Value;
                            cartItemUpdate.Quantity = item.Quantitys.Value;
                            cartItemUpdate.TotalPrice = item.TotalPrice.Value;
                            cartItemUpdate.UpdatedAt = item.UpdatedAt;
                        }
                    }

                }


                // request không có , database có , => xóa

                var newIds = cartNewAggregate.CartItemList.Where(s => s.Quantitys.Value <= 0).Select(i => i.Id).ToHashSet();

                var removedItems = cartBase.CartItems
                    .Where(dbItem => newIds.Contains(dbItem.Id))
                    .ToList();

                foreach (var item in removedItems)
                {
                    _db.CartItems.Remove(item);
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}
