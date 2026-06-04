using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Proto;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infrastructure.Models;
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
                            path.StartsWithSegments("/notificationPayOS"))
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
                options.Address = new Uri("https://localhost:5007");
            });

            services.AddGrpc();

            services.AddScoped<ICreateNewPaymentOrder, CreateNewPaymentOrder>();
            services.AddScoped<IUpdateOrderStatus, UpdateOrderStatus>();

            services.AddControllers();

            return services;
        }
    }
}
