using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.Proto;
using OrderService.API.Proto;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infrastructure.Consumers;
using payment_service.PaymentService.Infrastructure.Models;
using payment_service.PaymentService.Infrastructure.Repositorys;
using payment_service.PaymentService.Infrastructure.ServicesImplements;
using PayOS;
using System.Text;

namespace payment_service.PaymentService.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<FoodPaymentContext>(options =>
            {
                options.UseSqlServer(config["SQL"]);
            });


            services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQHost"], "/", h =>
                    {
                        h.Username(config["RabbitMQUsername"]!);
                        h.Password(config["RabbitMQPassword"]!);
                    });

                    cfg.ReceiveEndpoint("PaymentQueue", s =>
                    {
                        s.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                        s.ConcurrentMessageLimit = 10;
                        s.PrefetchCount = 20;
                        s.ConfigureConsumer<PaymentConsumer>(context);
                    });
                });
            });

            services.AddAuthentication(options =>
              {
                  options.DefaultAuthenticateScheme = "AccessToken";
                  options.DefaultChallengeScheme = "AccessToken";
              })
        .AddJwtBearer("AccessToken", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key:AccessToken"]!)
                )
            };


            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/QRCodeOrder") || path.StartsWithSegments("/notificationPayOS")))
                    {
       
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

            services.AddAuthorization();


            services.AddSignalR();


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


            services.AddSingleton<PayOSClient>(sp =>
            {
                return new PayOSClient(new PayOSOptions
                {
                    ClientId = config["ClientIDPayOS"],
                    ApiKey = config["ApiKeytPayOS"],
                    ChecksumKey = config["ChecksumKeyPOS"]
                });
            });


            services.AddGrpcClient<OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCPort_OrderService"]!);
            });

            services.AddGrpcClient<OrderGrpc.OrderGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCPort_OrderService"]!);
            });

            services.AddGrpc();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICreateNewPaymentOrder, CreateNewPaymentOrder>();
            services.AddScoped<IUpdateOrderStatus, UpdateOrderStatus>();

            services.AddControllers();

            return services;
        }
    }
}
