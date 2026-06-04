using cart_service.CartService.API.gRPC;
using cart_service.CartService.Application.DTOInternal;
using cart_service.CartService.Application.DTOs;
using cart_service.CartService.Application.Services;
using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Domain.Entities;
using cart_service.CartService.Domain.Interface;
using cart_service.CartService.Domain.ValueObject;
using cart_service.CartService.Infrastructure.Models;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using productService.API.Protos;

namespace cart_service.CartService.Infrastructure.ImplementServices
{
    public class UpdateCartFood : IUpdateCartFood
    {
        private readonly ICartRepository _cartRepo;
        private readonly FoodProductsDbContext _db;
        private readonly CartServiceClient _productInfor;
        private readonly ILogger<UpdateCartFood> _logger;

        public UpdateCartFood(ICartRepository cartRepository, FoodProductsDbContext foodProductsDbContext, CartServiceClient cartServiceClient, ILogger<UpdateCartFood> logger)
        {
            _cartRepo = cartRepository;
            _db = foodProductsDbContext;
            _productInfor = cartServiceClient;
            _logger = logger;
        }

        public async Task Excute(RequestUpdateCartFood request)
        {

            var cartBase = await _db.Carts.Include(s => s.CartItems).Include(s => s.CartDiscounts).Where(s => s.Status == "ACTIVE").FirstOrDefaultAsync(c => c.Id == request.IdCart);

            if (cartBase == null) return;

            var listCartItem = cartBase.CartItems.Select(s => new CartItemEntity(s.Id, s.CartId, s.ProductId, s.VariantId, s.ProductName, s.VariantName, new Price(s.UnitPrice), new Quantity(s.Quantity), new Price(s.TotalPrice), s.CreatedAt, s.UpdatedAt)).ToList();
            var listCartDiscount = cartBase.CartDiscounts.Select(s => new CartDiscountEntity(s.Id, s.CartId, s.Code, s.DiscountType, new PercentDiscount(s.DiscountValue), new Price(s.AppliedAmount))).ToList();

            var carAggregate = new CartAggregate(cartBase.Id, cartBase.UserId, cartBase.Status, new Price(cartBase.TotalPrice), cartBase.CreatedAt, cartBase.UpdatedAt, listCartItem, listCartDiscount);

            if (request.CartItems == null || !request.CartItems.Any()) return;

            var ListProductID = request.CartItems.Select(s => s.ProductId.ToString()).ToList();



            List<ProductDetailDTOInternal> Products = new List<ProductDetailDTOInternal>(); // map dto v?i data of product service by call grpc


            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var grpcRequest = new ProductRequestList();

                    grpcRequest.ProductId.AddRange(ListProductID);


                    var listProduct = await _productInfor.GetProductInfoAsync(grpcRequest);

                    Products = listProduct.ProductDetail.Select(s => new ProductDetailDTOInternal
                    {
                        IdProduct = Guid.Parse(s.IdProduct),
                        IdCategory = Guid.Parse(s.IdCategory),
                        Desciption = s.Description,
                        Name = s.Name,
                        Price = (decimal)s.Price,
                        StatusCode = s.StatusCode,

                        ListImage = s.ProductImages.Select(t => new ProductImageDTOInternal
                        {
                            Id = Guid.Parse(t.IdImage),
                            Url = t.UrlImage
                        }).ToList(),

                        ListVariant = s.ProductVariants.Select(g => new ProductVariantDTOInternal
                        {
                            IdVariant = Guid.Parse(g.IdVariant),
                            Name = g.Name,
                            ExtraPrice = (decimal)g.ExtraPrice,
                            TypeVariant = g.TypeProduct
                        }).ToList()


                    }).ToList();

                    if (listProduct.ProductDetail.Any()) break;
                    await Task.Delay(200);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable || ex.StatusCode == StatusCode.DeadlineExceeded)
                {
                    _logger.LogError($"Bug in file UpdateCartFood.cs :{ex.Message}");
                }
            }


            if (request.CartItems != null && request.CartItems.Any())
            {

                foreach (var item in request.CartItems)
                {
                    // bug

                    if (item.VariantId == Guid.Empty)
                    {
                        item.VariantId = null;
                    }

                    var cartItem = carAggregate.CartItemList.FirstOrDefault(c => c.ProductId == item.ProductId && (
                              (c.VariantId == null && item.VariantId == null) ||
                              (c.VariantId != null && c.VariantId == item.VariantId)
                       ));




                    if (cartItem != null)
                    {
                        cartItem.ChangeQuantity(item.Quantity);
                        continue;
                    }


                    var product = Products.FirstOrDefault(p => p.IdProduct == item.ProductId);
                    if (product == null) throw new Exception($"Product {item.ProductId} not found");


                    var variant = product.ListVariant.FirstOrDefault(v => v.IdVariant == item.VariantId);

                    if (variant != null)
                    {
                        cartItem = CartItemEntity.CreateNewCartItem(
                                           request.IdCart,
                                           item.ProductId,
                                           item.VariantId,
                                           variant.Name,
                                           product.Name,
                                           (product.Price + variant.ExtraPrice),
                                           item.Quantity);

                    }
                    else
                    {

                        if (item.Quantity == 0) continue;

                        cartItem = CartItemEntity.CreateNewCartItem(
                                           request.IdCart,
                                           item.ProductId,
                                           null,
                                           null,
                                           product.Name,
                                           product.Price,
                                           item.Quantity);

                    }



                    carAggregate.AddCartItem(cartItem);
                }

            }

            // tính t?ng ti?n gi? hàng
            var TotalCart = carAggregate.CartItemList.Sum(s => s.TotalPrice.Value);

            carAggregate.UpdateTotalPrice(TotalCart);

            await _cartRepo.UpdateCartAsync(carAggregate);


        }
    }
}
