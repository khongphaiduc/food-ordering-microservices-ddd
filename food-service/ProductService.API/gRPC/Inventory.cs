using food_service.ProductService.Application.Service;
using Grpc.Core;
using productService.API.Protos;
using System.Globalization;

namespace food_service.ProductService.API.gRPC
{
    public class Inventory : ProductInventoryGrpc.ProductInventoryGrpcBase
    {
        private readonly IReserveProductDailyInventory _reserve;

        public Inventory(IReserveProductDailyInventory reserveProductDailyInventory)
        {
            _reserve = reserveProductDailyInventory;
        }

        public override async Task<ReserveDailyInventoryResponse> ReserveDailyInventory(ReserveDailyInventoryRequest request, ServerCallContext context)
        {
            var response = new ReserveDailyInventoryResponse();
            var inventoryRequests = new List<Application.DTOs.Request.ReserveProductDailyInventoryRequest>();

            foreach (var item in request.Items)
            {
                if (!Guid.TryParse(item.ProductId, out var productId))
                {
                    response.Items.Add(new ReserveDailyInventoryItemResult
                    {
                        ProductId = item.ProductId,
                        Success = false,
                        Message = "Invalid product_id. No inventory was reserved."
                    });
                }
                else if (!string.IsNullOrWhiteSpace(item.InventoryDate)
                    && !DateOnly.TryParseExact(item.InventoryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    response.Items.Add(new ReserveDailyInventoryItemResult
                    {
                        ProductId = item.ProductId,
                        Success = false,
                        Message = "inventory_date must use yyyy-MM-dd format. No inventory was reserved."
                    });
                }
                else
                {
                    DateOnly? inventoryDate = string.IsNullOrWhiteSpace(item.InventoryDate)
                        ? null
                        : DateOnly.ParseExact(item.InventoryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                    inventoryRequests.Add(new Application.DTOs.Request.ReserveProductDailyInventoryRequest
                    {
                        ProductId = productId,
                        Quantity = item.Quantity,
                        InventoryDate = inventoryDate
                    });
                }
            }

            if (response.Items.Count > 0)
            {
                return response;
            }

            var batchResult = await _reserve.ExecuteBatchAsync(inventoryRequests);
            foreach (var item in request.Items)
            {
                var itemProductId = Guid.TryParse(item.ProductId, out var productId) ? productId : Guid.Empty;
                response.Items.Add(new ReserveDailyInventoryItemResult
                {
                    ProductId = item.ProductId,
                    Success = batchResult.Success,
                    Message = batchResult.Success
                        ? "Inventory reserved successfully."
                        : batchResult.FailedProductId == itemProductId
                            ? batchResult.Message
                            : "No inventory was reserved because the batch was rolled back."
                });
            }

            return response;
        }
    }
}
