using Microsoft.AspNetCore.SignalR;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Aggregate;
using order_service.OrderService.Domain.Entities;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infrastructure.OrderRealTime;
using PaymentService.API.Proto;

namespace order_service.OrderService.Infrastructure.ServicesImplements
{
    public class CreateNewOrder : ICreateNewOrder
    {
        private readonly GetInformationOfCart _cartClientGRPC;
        private readonly IOrderRepository _orderRepository;
        private readonly PaymentInforGrpc.PaymentInforGrpcClient _createPaymentPayOs;
        private readonly ILogger<CreateNewOrder> _logger;
        private readonly GetAddressUserServiceSideClient _AddressUser;
        private readonly IHubContext<NotificationOrderHUB> _orderHub;

        public CreateNewOrder(IHubContext<NotificationOrderHUB> hubContext, GetAddressUserServiceSideClient getAddressUserServiceSideClient, GetInformationOfCart getInformationOfCartClient, IOrderRepository orderRepository, PaymentInforGrpc.PaymentInforGrpcClient paymentInforGrpcClient, ILogger<CreateNewOrder> logger)
        {
            _cartClientGRPC = getInformationOfCartClient;
            _orderRepository = orderRepository;
            _createPaymentPayOs = paymentInforGrpcClient;
            _logger = logger;
            _AddressUser = getAddressUserServiceSideClient;
            _orderHub = hubContext;
        }

        public async Task<string> Execute(Guid IdCart, PaymentMethod paymentMethod, Guid IdAddress)
        {
            // yet retry
            var cart = await _cartClientGRPC.Excute(IdCart);  // data cart service 

            if (cart.CartId == Guid.Empty) return string.Empty;


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

            // d?a ch? giao hàng 
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

            var resultCreateNewOrder = await _orderRepository.CreateNewOrder(newOrderAggregate);  //   t?o order

            if (resultCreateNewOrder.Status)
            {
                var resultChangeStatusCart = await _cartClientGRPC.ChangeStatusCart(cart.CartId, StatusCart.CHECKED_OUT);  //  change status cart = Checked out

                if (resultChangeStatusCart == false) return string.Empty;


                var orderNotificationRealTimeDTO = new ViewOrderDTO
                {
                    OrderCode = "Ðon Food M?i",
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
                }


                // thanh toán ti?n m?t
                if (paymentMethod != PaymentMethod.PayOS)
                {
                    return "Success";
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
                    return QRCodeString.QRCodeString;
                }
            }


            return string.Empty;
        }
    }
}
