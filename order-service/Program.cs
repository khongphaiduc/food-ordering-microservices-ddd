using CartService.API.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infastructure.Models;
using order_service.OrderService.Infastructure.OrderRealTime;
using order_service.OrderService.Infastructure.Repository;
using order_service.OrderService.Infastructure.ServicesImplements;
using PaymentService.API.Proto;
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

                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ordersHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            builder.Services.AddGrpcClient<CartInforGrpc.CartInforGrpcClient>(o => o.Address = new Uri("https://localhost:7185"));
            builder.Services.AddGrpcClient<PaymentInforGrpc.PaymentInforGrpcClient>(o => o.Address = new Uri("https://localhost:7251"));
            builder.Services.AddGrpcClient<UserAddressInfoGrpc.UserAddressInfoGrpcClient>(o => o.Address = new Uri("https://localhost:7199"));

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICreateNewOrder, CreateNewOrder>();
            builder.Services.AddScoped<IGetListOrderOfUser, GetListOrderOfUser>();
            builder.Services.AddScoped<IGetViewDetailOreder, GetViewDetailOreder>();
            builder.Services.AddScoped<GetInformationOfCart>();
            builder.Services.AddScoped<GetAddressUserServiceSideClient>();
            builder.Services.AddScoped<IGetListOrders, GetListOrders>();
            builder.Services.AddScoped<IUpdateStatusOrders, UpdateStatusOrders>();
            builder.Services.AddScoped<IStaffViewDetailOrders, StaffViewDetailOrders>();
            builder.Services.AddScoped<IRevenue, Revenue>();
            builder.Services.AddScoped<IGetNumberOfOrders, GetNumberOfOrders>();
            builder.Services.AddScoped<IGetPreFitOfMonths, GetPreFitOfMonths>();
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddGrpc();

            var app = builder.Build();



            app.UseHttpsRedirection();


            app.UseCors("AllowFrontend");


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapHub<NotificationOrderHUB>("/ordersHub");
            app.MapGrpcService<UpdateStatusOrderService>();
            app.MapControllers();

            app.Run();
        }
    }
}