using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.Repositories
{
    public class ProductReserve : IProductReserve
    {
        private readonly FoodProductsDbContext _db;

        public ProductReserve(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<bool> ReservecProduct(string productId, int quantity)
        {

            var affectedRows = await _db.Database.ExecuteSqlInterpolatedAsync($"""
    UPDATE public.product_daily_inventories
    SET
        remaining_quantity = remaining_quantity - {quantity},
        sold_quantity = sold_quantity + {quantity},
        updated_at = NOW()
    WHERE remaining_quantity >= {quantity}
      AND inventory_date = CURRENT_DATE
      AND product_id = {productId}
    """);
            return affectedRows > 0;
        }
    }
}
