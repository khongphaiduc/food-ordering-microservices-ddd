using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infastructure.Models;

namespace order_service.OrderService.Infastructure.ServicesImplements
{
    #region view order detail for staff and  admin
    public class StaffViewDetailOrders : IStaffViewDetailOrders
    {
        private readonly FoodOrderContext _db;

        public StaffViewDetailOrders(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<StaffViewDetailOrderDTO> Excute(Guid IdOrder)
        {

            var orderInfor = await _db.Orders.Where(o => o.Id == IdOrder)

                .Select(s => new StaffViewDetailOrderDTO
                {
                    OrderCode = s.OrderCode,
                    IdUser = s.UserId,
                    OrderId = s.Id,
                    FinalAmount = s.FinalAmount,
                    TotalAmount = s.TotalAmount,
                    OrderStatus = Enum.Parse<OrderStatus>(s.OrderStatus),
                    OrderStatusPayment = Enum.Parse<OrderStatusPayment>(s.Status),
                    DiscountAmount = s.DiscountAmount,
                    ShipmentAmount = s.ShippingFee,
                    CreateAt = s.CreatedAt,
                    SnapshotNameCustomer = s.SnapshotNameCustomer,
                    SnapshotPhoneNumber = s.SnapshotPhone,
                    PaymentMethod = Enum.Parse<PaymentMethod>(s.PaymentMethod!),

                    orderItemDetail = s.OrderItems.Select(a => new OrderItemDetail
                    {
                        NameProduct = a.ProductName,
                        NameVariant = a.VariantName ?? "Varint chưa đặt tên",
                        PricePerProduct = a.Price,
                        Quantity = a.Quantity,
                        TotalPrice = a.TotalPrice
                    }).ToList(),

                    orderDeliveryInfor = s.OrderDelivery != null
    ? new OrderDeliveryInfor
    {
        Address = s.OrderDelivery.Address ?? "VietNam",
        Note = s.OrderDelivery.Note ?? "Không có ghi chú",
        OrderId = s.OrderDelivery.OrderId,
        ReciveName = s.OrderDelivery.ReceiverName,
        RecivePhoneNumber = s.OrderDelivery.Phone
    }
    : null


                }).FirstOrDefaultAsync();



            return orderInfor ?? new StaffViewDetailOrderDTO();
        }
    }
    #endregion
}
