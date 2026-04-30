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
using UserService.API.Protos;

namespace order_service.OrderService.Infastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {
          
            services.AddDbContext<FoodOrderContext>(options =>
            {
                options.UseSqlServer(config["URLORDER"]);
            });

            
            services.AddAuthentication(options =>
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
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key:AccessToken"]!)
                    )
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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            
            services.AddGrpcClient<CartInforGrpc.CartInforGrpcClient>(o =>
                o.Address = new Uri("https://localhost:5005"));

            services.AddGrpcClient<PaymentInforGrpc.PaymentInforGrpcClient>(o =>
                o.Address = new Uri("https://localhost:5006"));

            services.AddGrpcClient<UserAddressInfoGrpc.UserAddressInfoGrpcClient>(o =>
                o.Address = new Uri("https://localhost:5001"));

          
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICreateNewOrder, CreateNewOrder>();
            services.AddScoped<IGetListOrderOfUser, GetListOrderOfUser>();
            services.AddScoped<IGetViewDetailOreder, GetViewDetailOreder>();

            services.AddScoped<IGetListOrders, GetListOrders>();
            services.AddScoped<IUpdateStatusOrders, UpdateStatusOrders>();
            services.AddScoped<IStaffViewDetailOrders, StaffViewDetailOrders>();

            services.AddScoped<IRevenue, Revenue>();
            services.AddScoped<IGetNumberOfOrders, GetNumberOfOrders>();
            services.AddScoped<IGetPreFitOfMonths, GetPreFitOfMonths>();

            services.AddScoped<GetInformationOfCart>();
            services.AddScoped<GetAddressUserServiceSideClient>();

           
            services.AddControllers();
            services.AddSignalR();
            services.AddGrpc();

            return services;
        }
    }
}
