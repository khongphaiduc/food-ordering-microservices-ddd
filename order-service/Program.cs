

using CartService.API.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infastructure.Models;
using order_service.OrderService.Infastructure.Repository;
using order_service.OrderService.Infastructure.ServicesImplements;
using PaymentService.API.Proto;
using System.Threading.Tasks;
using UserService.API.Protos;

namespace order_service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodOrderContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["URLORDER"]);
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer("AccessToken", option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:AccessToken"]!))
            };
        });


            builder.Services.AddGrpcClient<CartInforGrpc.CartInforGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:7185");

            });

            builder.Services.AddGrpcClient<PaymentInforGrpc.PaymentInforGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:7251");

            });


            builder.Services.AddGrpcClient<UserAddressInfoGrpc.UserAddressInfoGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:7199");

            });


            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<ICreateNewOrder, CreateNewOrder>();
            builder.Services.AddScoped<IGetListOrderOfUser, GetListOrderOfUser>();
            builder.Services.AddScoped<IGetViewDetailOreder, GetViewDetailOreder>();
            builder.Services.AddScoped<GetInformationOfCart>();
            builder.Services.AddScoped<GetAddressUserServiceSideClient>();
            builder.Services.AddScoped<IGetListOrders, GetListOrders>();
            builder.Services.AddScoped<IUpdateStatusOrders, UpdateStatusOrders>();
            builder.Services.AddControllers();

            builder.Services.AddGrpc();
            var app = builder.Build();

            app.MapGrpcService<UpdateStatusOrderService>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
