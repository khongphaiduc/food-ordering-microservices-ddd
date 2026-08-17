using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Aggregate;
using order_service.OrderService.Domain.Entities;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infrastructure.OrderRealTime;
using PaymentService.API.Proto;
using StackExchange.Redis;

namespace order_service.OrderService.Infrastructure.ServicesImplements
{
    public class CreateNewOrder : ICreateNewOrder
    {
        private readonly InventoryProduct _inventoryProduct;
        private readonly GetInformationOfCart _cartClientGRPC;
        private readonly IOrderRepository _orderRepository;
        private readonly PaymentInforGrpc.PaymentInforGrpcClient _createPaymentPayOs;
        private readonly ILogger<CreateNewOrder> _logger;
        private readonly GetAddressUserServiceSideClient _AddressUser;
        private readonly IHubContext<NotificationOrderHUB> _orderHub;
        private readonly IDatabase _cache;

        public CreateNewOrder(IConnectionMultiplexer connectionMultiplexer, InventoryProduct inventoryProduct, IHubContext<NotificationOrderHUB> hubContext, GetAddressUserServiceSideClient getAddressUserServiceSideClient, GetInformationOfCart getInformationOfCartClient, IOrderRepository orderRepository, PaymentInforGrpc.PaymentInforGrpcClient paymentInforGrpcClient, ILogger<CreateNewOrder> logger)
        {
            _inventoryProduct = inventoryProduct;
            _cartClientGRPC = getInformationOfCartClient;
            _orderRepository = orderRepository;
            _createPaymentPayOs = paymentInforGrpcClient;
            _logger = logger;
            _AddressUser = getAddressUserServiceSideClient;
            _orderHub = hubContext;
            _cache = connectionMultiplexer.GetDatabase();

        }

        public async Task<RequestCreateNewOrderAndPayment> Execute(Guid IdempotencyKey, Guid IdCart, PaymentMethod paymentMethod, Guid IdAddress)
        {

            string key = IdempotencyKey.ToString();

            var ResultRequest = await _cache.StringGetAsync(key);

            if (!string.IsNullOrEmpty(ResultRequest))
            {
                return new RequestCreateNewOrderAndPayment
                {
                    Message = "Request is already being processed",
                    StatusCreateOrder = false,
                    QRCodeString = "DUC is testing this api"
                };

            }

            var acquired = await _cache.StringSetAsync(IdempotencyKey.ToString(), IdCart.ToString(), TimeSpan.FromDays(1), When.NotExists);

            if (!acquired)
            {
                return new RequestCreateNewOrderAndPayment
                {
                    Message = "Request is already being processed",
                    StatusCreateOrder = false
                };
            }


            // yet retry
            var cart = await _cartClientGRPC.Excute(IdCart);  // data cart service 

            if (cart.CartId == Guid.Empty)
            {
                await _cache.KeyDeleteAsync(key);

                return new RequestCreateNewOrderAndPayment
                {
                    Message = "Cart not found",
                    StatusCreateOrder = false
                };
            }


            // yet retry
            var AddressUser = await _AddressUser.GetAddressUserAsync(new UserService.API.Protos.AddressformationUserRequest { IdAddress = IdAddress.ToString() });

            // order
            var newOrderAggregate = OrdersAggregate.CreateNewOrder(cart.CartId, cart.UserId, cart.Status, 0, 0, paymentMethod, AddressUser.NameUser, AddressUser.Phone);

            // order items
            if (cart.CartItems != null && cart.CartItems.Any())
            {
                foreach (var item in cart.CartItems)
                {
                    newOrderAggregate.AddOrderItem(OrderItemsEntity.CreateOrderItems(newOrderAggregate.IdOrder, item.ProductId, item.ProductName, item.VariantId, item.VariantName, (decimal)item.UnitPrice, item.Quantity, (decimal)item.TotalPrice));
                }
            }

            // User's Address
            newOrderAggregate.AddDelivery(OrderDeliveryEntity.CreateNewOrderDelivery(newOrderAggregate.IdOrder, AddressUser.NameUser, AddressUser.Phone, AddressUser.Address, AddressUser.Note, DateTime.UtcNow.AddHours(1)));

            // discount 
            decimal DiscountAmount = 0;

            if (cart.CartDiscounts != null && cart.CartDiscounts.Any())
            {
                DiscountAmount = cart.CartDiscounts.Sum(s => s.DiscountAmount);
                newOrderAggregate.SetDiscount(DiscountAmount);
            }

            // payment 
            newOrderAggregate.AddOrderPayment(OrderPaymentsEntity.CreateOrderPayment(newOrderAggregate.IdOrder, paymentMethod, PaymentStatus.PENDING, newOrderAggregate.FinalAmount.Value, null, null));

            var resultCreateNewOrder = await _orderRepository.CreateNewOrder(newOrderAggregate);  //   create order

            if (resultCreateNewOrder.Status)
            {
                var resultChangeStatusCart = await _cartClientGRPC.ChangeStatusCart(cart.CartId, StatusCart.CHECKED_OUT);  //  change status cart = Checked out

                if (resultChangeStatusCart == false)
                {
                    await _cache.KeyDeleteAsync(key);
                    return new RequestCreateNewOrderAndPayment { Message = "Cart can not change status to CheckOut", StatusCreateOrder = false };
                }




                var orderNotificationRealTimeDTO = new ViewOrderDTO
                {
                    OrderCode = "Đon Food Mi",
                    IdOrder = newOrderAggregate.IdOrder,
                    NameCustomer = newOrderAggregate.SnapshotNameCustomer,
                    CreateAt = newOrderAggregate.CreatedAt,
                    orderStatus = OrderStatus.PENDING,
                    OrderStatusPayment = newOrderAggregate.StatusOrderPayment,
                    PaymentMethod = paymentMethod,
                    TotalAmount = newOrderAggregate.TotalAmount.Value,
                };

                try
                {
                    await _orderHub.Clients.Group("ADMIN_GROUP").SendAsync("ReceiveOrder", orderNotificationRealTimeDTO);
                    await _orderHub.Clients.Group("STAFF_GROUP").SendAsync("ReceiveOrder", orderNotificationRealTimeDTO);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Realtime notification failed");
                    throw;
                }
                // Cash orders are paid when the customer receives the delivery.
                // The order payment remains PENDING until the order reaches COMPLETED.
                if (paymentMethod == PaymentMethod.Cash)
                {
                    var inventoryReservation = await _inventoryProduct.ReduceInventoryProduct(cart.CartItems ?? []);
                    if (!inventoryReservation.Success)
                    {
                        await _cartClientGRPC.ChangeStatusCart(cart.CartId, StatusCart.ACTIVE);
                        await _cache.KeyDeleteAsync(key);
                        return new RequestCreateNewOrderAndPayment
                        {
                            Message = inventoryReservation.Message,
                            ErrorCode = "INVENTORY_RESERVATION_FAILED",
                            StatusCreateOrder = false,
                        };
                    }

                    _logger.LogInformation("Cash order {OrderId} was created and is awaiting payment on delivery.", resultCreateNewOrder.IdOrder);
                    return new RequestCreateNewOrderAndPayment
                    {
                        Message = inventoryReservation.Message,
                        StatusCreateOrder = true,

                    };
                }


                // create url payment 
                var QRCodeString = await _createPaymentPayOs.CreateNewPaymentAsync(new global::PaymentService.API.Proto.RequestOrderCreatePayment
                {
                    OrderId = resultCreateNewOrder.IdOrder.ToString(),
                    DiscountAmount = 0,
                    FinalAmount = (double)resultCreateNewOrder.FinalAmount,
                    OrderCode = resultCreateNewOrder.OrderCode,
                });

                if (QRCodeString.StatusCreatePayment == "Success")
                {
                    await _cache.StringSetAsync(
                      key,
                      resultCreateNewOrder.IdOrder.ToString(),
                      TimeSpan.FromDays(1),
                      When.Exists);

                    return new RequestCreateNewOrderAndPayment { Message = "", StatusCreateOrder = true, QRCodeString = QRCodeString.QRCodeString };
                }
            }


            return new RequestCreateNewOrderAndPayment { Message = "Bug occured when process the payment", StatusCreateOrder = false };
        }
    }
}
