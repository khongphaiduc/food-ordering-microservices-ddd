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

        public async Task<InventoryReservationResult> ReduceInventoryProduct(
            IReadOnlyCollection<CartItemDTOsInternal> cartItems)
        {
            if (cartItems.Count == 0)
            {
                return new InventoryReservationResult
                {
                    Success = false,
                    Message = "At least one cart item is required to reserve inventory."
                };
            }

            var cartItemList = cartItems.ToList();
            var request = new ReserveDailyInventoryRequest();

            foreach (var item in cartItemList)
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

                var result = new InventoryReservationResult
                {
                    Success = response.Success,
                    Message = string.IsNullOrWhiteSpace(response.Message)
                        ? response.Success
                            ? "All inventory items were reserved successfully."
                            : "One or more inventory items could not be reserved."
                        : response.Message
                };

                for (var index = 0; index < cartItemList.Count; index++)
                {
                    var cartItem = cartItemList[index];
                    var grpcItem = index < response.Items.Count ? response.Items[index] : null;

                    result.Items.Add(new InventoryReservationItemResult
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        RequestedQuantity = cartItem.Quantity,
                        Success = grpcItem?.Success ?? false,
                        Message = grpcItem?.Message
                            ?? "Food Service did not return a result for this product."
                    });
                }

                result.Success = result.Success
                    && response.Items.Count == cartItemList.Count
                    && result.Items.All(item => item.Success);

                return result;
            }
            catch (RpcException ex)
            {
                const string message = "Inventory could not be reserved because Food Service is unavailable.";

                return new InventoryReservationResult
                {
                    Success = false,
                    Message = $"{message} gRPC status: {ex.StatusCode}.",
                    Items = cartItemList.Select(item => new InventoryReservationItemResult
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        RequestedQuantity = item.Quantity,
                        Success = false,
                        Message = message
                    }).ToList()
                };
            }
        }
    }
}
