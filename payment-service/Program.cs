using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Proto;
using payment_service.PaymentService.API.gRPC;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infastructure.Models;
using payment_service.PaymentService.Infastructure.NotificationRealTimeSignalR;
using payment_service.PaymentService.Infastructure.ServicesImplements;
using PayOS;
using System.Text;

namespace payment_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDbContext<FoodPaymentContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["SQL"]);
            });

            
            builder.Services.AddAuthentication(options =>
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
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key:AccessToken"]!))
                };

                // Lấy token từ query cho SignalR
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

            builder.Services.AddAuthorization();

            builder.Services.AddSignalR();

           
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


            builder.Services.AddSingleton<PayOSClient>(sp =>
            {
                return new PayOSClient(new PayOSOptions
                {
                    ClientId = builder.Configuration["ClientIDPayOS"],
                    ApiKey = builder.Configuration["ApiKeytPayOS"],
                    ChecksumKey = builder.Configuration["ChecksumKeyPOS"]
                });
            });

          // order 
            builder.Services.AddGrpcClient<OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5007");
            });

            builder.Services.AddGrpc();

            builder.Services.AddScoped<ICreateNewPaymentOrder, CreateNewPaymentOrder>();
            builder.Services.AddScoped<IUpdateOrderStatus, UpdateOrderStatus>();

            builder.Services.AddControllers();

            var app = builder.Build();

            

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.MapGrpcService<CreatePaymentServers>();

            app.MapHub<NotificationPaidSusscessfully>("/notificationPayOS")
     .RequireCors("AllowFrontend");

            app.MapControllers();

            app.Run();
        }
    }
}
