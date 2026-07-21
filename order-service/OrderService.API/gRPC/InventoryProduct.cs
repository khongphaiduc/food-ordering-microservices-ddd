using Grpc.Core;
using order_service.OrderService.Application.DTOs.DTOsInternal;
using productService.API.Protos;

namespace order_service.OrderService.API.gRPC
{
    public class InventoryProduct
    {
        private readonly ProductInventoryGrpc.ProductInventoryGrpcClient _inventoryProduct;

        public InventoryProduct(ProductInventoryGrpc.ProductInventoryGrpcClient productInventoryGrpcClient)
        {
            _inventoryProduct = productInventoryGrpcClient;
        }

        public async Task<bool> ReduceInventoryProduct(IReadOnlyCollection<CartItemDTOsInternal> cartItems)
        {
            if (cartItems.Count == 0)
            {
                return false;
            }

            var request = new ReserveDailyInventoryRequest();
            foreach (var item in cartItems)
            {
                request.Items.Add(new ReserveDailyInventoryItem
                {
                    ProductId = item.ProductId.ToString(),
                    Quantity = item.Quantity
                    // Leave InventoryDate empty so food-service uses its current date.
                });
            }

            try
            {
                var response = await _inventoryProduct.ReserveDailyInventoryAsync(
                    request,
                    deadline: DateTime.UtcNow.AddSeconds(3));

                return response.Items.Count == cartItems.Count
                    && response.Items.All(item => item.Success);
            }
            catch (RpcException)
            {
                return false;
            }
        }
    }

}