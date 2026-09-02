using CartService.API.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Application.Interface;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infrastructure.Consumers;
using order_service.OrderService.Infrastructure.Models;
using order_service.OrderService.Infrastructure.Repository;
using order_service.OrderService.Infrastructure.ServicesImplements;
using order_service.OrderService.Infrastructure.Workers;
using PaymentService.API.Proto;
using productService.API.Protos;
using StackExchange.Redis;
using UserService.API.Protos;

namespace order_service.OrderService.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<FoodOrderContext>(options =>
            {
                options.UseSqlServer(config["URLORDER"]);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis"];
                options.InstanceName = "Redis_OrderService";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                   ConnectionMultiplexer.Connect(config["Redis"]!));

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

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/ordersHub") || path.StartsWithSegments("/orderofuser")))
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
                    policy.WithOrigins(config["URL_FE"]!)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<FoodOrderContext>(s =>
                {
                    s.UseSqlServer();
                    s.UseBusOutbox();
                    s.DuplicateDetectionWindow = TimeSpan.FromDays(7);
                    s.QueryDelay = TimeSpan.FromSeconds(1);
                });

                x.AddConsumer<HandleOrderPaySuccessfullyConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {

                    cfg.Host(config["RabbitMQ_HostName"], "/", h =>
                    {
                        h.Username(config["RabbitMQ_UserName"]!);
                        h.Password(config["RabbitMQ_Password"]!);
                    });

                    cfg.ReceiveEndpoint("PaidOrderEvent", s =>
                    {
                        s.UseMessageRetry(r =>
                        {
                            r.Intervals(
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(5)
                            );
                        });
                        s.ConcurrentMessageLimit = 20;
                        s.PrefetchCount = 30;
                        s.UseEntityFrameworkOutbox<FoodOrderContext>(context);
                        s.ConfigureConsumer<HandleOrderPaySuccessfullyConsumer>(context);
                    });
                });

            });




            services.AddGrpcClient<CartInforGrpc.CartInforGrpcClient>(o =>
             o.Address = new Uri(config["gRPC_PORT_CartService"]!));

            services.AddGrpcClient<PaymentInforGrpc.PaymentInforGrpcClient>(o =>
                o.Address = new Uri(config["gRPC_PORT_PaymentService"]!));

            services.AddGrpcClient<UserAddressInfoGrpc.UserAddressInfoGrpcClient>(o =>
                o.Address = new Uri(config["gRPC_PORT_UserService"]!));

            services.AddGrpcClient<ProductInventoryGrpc.ProductInventoryGrpcClient>(s =>
            s.Address = new Uri(config["gRPC_PORT_FoodService"]!));

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddHostedService<CheckExpireOrder>();

            services.AddScoped<ICreateNewOrder, CreateNewOrder>();
            services.AddScoped<IGetListOrderOfUser, GetListOrderOfUser>();
            services.AddScoped<IGetViewDetailOrder, GetViewDetailOrder>();

            services.AddScoped<IGetListOrders, GetListOrders>();
            services.AddScoped<IUpdateStatusOrders, UpdateStatusOrders>();
            services.AddScoped<IStaffViewDetailOrders, StaffViewDetailOrders>();

            services.AddScoped<IRevenue, Revenue>();
            services.AddScoped<IGetNumberOfOrders, GetNumberOfOrders>();
            services.AddScoped<IGetPreFitOfMonths, GetPreFitOfMonths>();

            services.AddScoped<GetInformationOfCart>();
            services.AddScoped<GetAddressUserServiceSideClient>();
            services.AddScoped<InventoryProduct>();
            services.AddControllers();
            services.AddSignalR();
            services.AddGrpc();

            return services;
        }
    }
}
