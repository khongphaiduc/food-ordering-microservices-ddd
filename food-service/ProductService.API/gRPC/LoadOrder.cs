using order_service.OrderService.API.Proto;

namespace food_service.ProductService.API.gRPC
{
    public class LoadOrder
    {
        private readonly OrderGrpc.OrderGrpcClient _orderClient;

        public LoadOrder(OrderGrpc.OrderGrpcClient orderGrpcClient)
        {
            _orderClient = orderGrpcClient;
        }


        public async Task<ReponseOrder> LoadOrderAsync(Guid IdOrder)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var result = await _orderClient.ViewOrderDetailAsync(
                        new RequestOrder
                        {
                            IdOrder = IdOrder.ToString()
                        },
                        deadline: DateTime.UtcNow.AddSeconds(3)
                    );

                    return new ReponseOrder
                    {
                        IdOrder = Guid.Parse(result.IdOrder),
                        OrderItems = result.OrderItems.Select(x => new LoadOrderResponse
                        {
                            IdProduct = Guid.Parse(x.ProductId),
                            Quantity = x.Quantity
                        }).ToList()
                    };
                }
                catch (Exception)
                {
                    if (i == 2) throw;

                    await Task.Delay(700);
                }
            }

            throw new Exception("Unable to load order.");
        }

    }

    public class LoadOrderResponse
    {
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
    }


    public class ReponseOrder
    {
        public Guid IdOrder { get; set; }

        public List<LoadOrderResponse> OrderItems { get; set; } = new();
    }

}