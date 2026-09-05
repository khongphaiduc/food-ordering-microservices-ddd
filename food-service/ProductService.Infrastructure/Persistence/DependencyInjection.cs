using food_service.ProductService.API.gRPC;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Infrastructure.Consumers;
using food_service.ProductService.Infrastructure.ImplementService;
using food_service.ProductService.Infrastructure.MinIO;
using food_service.ProductService.Infrastructure.Models;

using food_service.ProductService.Infrastructure.RedisService.RedisInterface;
using food_service.ProductService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using order_service.OrderService.API.Proto;
using StackExchange.Redis;


namespace food_service.ProductService.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(config["SQLFOOD_PRODUCTS"]!);
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
            });

            services.AddAuthorization();


            services.AddRateLimiter(option =>
            {
                option.AddFixedWindowLimiter("rateFix", s =>
                {
                    s.Window = TimeSpan.FromSeconds(60);
                    s.PermitLimit = 10;
                    s.QueueLimit = 0;
                });
            });

            services.AddMassTransit(x =>
            {

                x.AddEntityFrameworkOutbox<FoodProductsDbContext>(s =>
                {
                    s.UsePostgres();
                    s.UseBusOutbox();
                });

                x.AddConsumer<ReserveProductConsumer>();

                x.AddConsumer<RestoreProductPayTimeOutConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQHost"], "/", h =>
                    {
                        h.Username(config["RabbitMQUsername"]!);
                        h.Password(config["RabbitMQPassword"]!);
                    });


                    cfg.ReceiveEndpoint("ReserveProduct_Queue", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                        e.UseConcurrencyLimit(10);
                        e.PrefetchCount = 20;
                        e.UseEntityFrameworkOutbox<FoodProductsDbContext>(context);
                        e.ConfigureConsumer<ReserveProductConsumer>(context);

                    });


                    cfg.ReceiveEndpoint("RestoreProduct_Queue", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                        e.UseConcurrencyLimit(10);
                        e.PrefetchCount = 20;
                        e.UseEntityFrameworkOutbox<FoodProductsDbContext>(context);
                        e.ConfigureConsumer<RestoreProductPayTimeOutConsumer>(context);

                    });



                });
            });

            services.AddScoped<LoadOrder>();
            services.AddScoped<IProductReserve, ProductReserve>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();


            services.AddScoped<IGetListProduct, GetListProduct>();
            services.AddScoped<IViewDetailProduct, ViewDetailProduct>();
            services.AddScoped<ICreateNewProduct, CreateNewProduct>();
            services.AddScoped<IUpdateProduct, UpdateProduct>();
            services.AddScoped<IUpdateCategory, UpdateCategory>();
            services.AddScoped<ICreateNewCategory, CreateNewCategory>();
            services.AddScoped<IGetListCategory, GetListCategory>();
            services.AddScoped<IGetProductDailyInventory, GetProductDailyInventory>();
            services.AddScoped<IReserveProductDailyInventory, ReserveProductDailyInventory>();
            services.AddScoped<IAdminProductDailyInventory, AdminProductDailyInventory>();
            services.AddSingleton<IInventoryDateProvider, InventoryDateProvider>();

            services.AddScoped<IMinIOFood, MinIOFood>();

            services.AddScoped<IProductRecommendationService, ProductRecommendationService>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["RedisAddress"];
                options.InstanceName = "FoodAppShared_";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(config["RedisAddress"]!));

            services.AddTransient<IRedisLockService, RedisLockService>();

            services.AddSingleton<IMinioClient>(sp =>
            {
                return new MinioClient()
                    .WithEndpoint(config["MinIOEndpoint"] ?? "localhost:9000")
                    .WithCredentials(config["MinIOAccessKey"], config["MinIOSecretKey"])
                    .WithSSL(false)
                    .Build();
            });

            services.AddGrpcClient<OrderGrpc.OrderGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCPort_OrderServices"]!);
            });

            services.AddGrpcClient<OrderByOrderCodeGrpc.OrderByOrderCodeGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCPort_OrderServices"]!);
            });


            services.AddGrpc();
            services.AddControllers();

            return services;
        }
    }
}
