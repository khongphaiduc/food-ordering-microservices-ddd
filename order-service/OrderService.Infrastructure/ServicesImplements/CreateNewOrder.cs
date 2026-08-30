using Foodly.Contracts.Events;
using MassTransit;
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
using order_service.OrderService.Infrastructure.Models;
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
        private readonly IPublishEndpoint _ipublishEvent;
        private readonly FoodOrderContext _db;

        public CreateNewOrder(IPublishEndpoint publishEndpoint, IConnectionMultiplexer connectionMultiplexer, InventoryProduct inventoryProduct, IHubContext<NotificationOrderHUB> hubContext, GetAddressUserServiceSideClient getAddressUserServiceSideClient, GetInformationOfCart getInformationOfCartClient, IOrderRepository orderRepository, PaymentInforGrpc.PaymentInforGrpcClient paymentInforGrpcClient, FoodOrderContext foodOrderContext, ILogger<CreateNewOrder> logger)
        {
            _inventoryProduct = inventoryProduct;
            _cartClientGRPC = getInformationOfCartClient;
            _orderRepository = orderRepository;
            _createPaymentPayOs = paymentInforGrpcClient;
            _logger = logger;
            _AddressUser = getAddressUserServiceSideClient;
            _orderHub = hubContext;
            _cache = connectionMultiplexer.GetDatabase();
            _ipublishEvent = publishEndpoint;
            _db = foodOrderContext;

        }

        public async Task<RequestCreateNewOrderAndPayment> Execute(Guid Iduser,Guid IdempotencyKey, Guid IdCart, PaymentMethod paymentMethod, Guid IdAddress)
        {

            string key = IdempotencyKey.ToString();

            var ResultRequest = await _cache.StringGetAsync(key);
            // case đã tồn tại 
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

            if (cart.CartId == Guid.Empty)// case không  tìm thấy id cart 
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

            
            var resultCreateNewOrder = await _orderRepository.CreateNewOrder(newOrderAggregate);  //   create order

            if (!resultCreateNewOrder.Status)
            {
                //await _ipublishEvent.Publish(new CreatedOrderEventFail
                //{
                //    IdOrder = resultCreateNewOrder.IdOrder,
                //    OrderMethodPayment = paymentMethod.ToString(),
                //    IdUser = Iduser
                //});

                //await _db.SaveChangesAsync();

                return new RequestCreateNewOrderAndPayment
                {
                    Message = "Create order failed",
                    StatusCreateOrder = false
                };
            }

            await _ipublishEvent.Publish(new CheckOutCartEvent
            {
                IdCart = cart.CartId,
                IdUser = Iduser
            });

            _logger.LogInformation(
                "Staged CheckOutCartEvent in outbox. CartId: {CartId}",
                cart.CartId);

            await _ipublishEvent.Publish(new CreatedOrderEvent
            {
                IdOrder = resultCreateNewOrder.IdOrder,
                OrderMethodPayment = paymentMethod.ToString(),
                IdUser = Iduser
            });

            await _db.SaveChangesAsync();

            var orderNotificationRealTimeDTO = new ViewOrderDTO
            {
                OrderCode = resultCreateNewOrder.OrderCode,
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
            return new RequestCreateNewOrderAndPayment { Message = "Please, To Pay your order before the recieve", StatusCreateOrder = true };
        }
    }
}
