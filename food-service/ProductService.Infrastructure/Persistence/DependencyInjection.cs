using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Infrastructure.BackgroundServices;
using food_service.ProductService.Infrastructure.ImplementService;
using food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.Consumers;
using food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.Producer;
using food_service.ProductService.Infrastructure.MinIO;
using food_service.ProductService.Infrastructure.Models;
using food_service.ProductService.Infrastructure.ProducerRabbitMQ;
using food_service.ProductService.Infrastructure.RedisService.RedisInterface;
using food_service.ProductService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using StackExchange.Redis;
using trackingtService.API.Protos;

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
                x.AddConsumer<RecommendFoodConsumer>();  // register consumer 

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQ_Side_ProductService:Host"], h =>
                    {
                        h.Username(config["RabbitMQ_Side_ProductService:Username"]!);
                        h.Password(config["RabbitMQ_Side_ProductService:Password"]!);
                    });

                    cfg.ReceiveEndpoint("RecommendationFoodByAI_Queue", e =>  // name of queue
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<RecommendFoodConsumer>(context);       // map consumer v?i queue
                    });
                    // khi run th́ nó s? bind exchange và queue d?a trên các type message c?a consumer dang map t?i queue
                });
            });

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
            services.AddScoped<IRecommendPersonalFood, RecommendPersonalFood>();
            services.AddScoped<IProductRecommendationService, ProductRecommendationService>();

    
            services.AddSingleton<FoodProducer>();
            services.AddScoped<GeminiModelFoodlyProducer>();

       
            services.AddScoped<IOutBoxPatternProduct, OutBoxPatternProduct>();
            services.AddHostedService<OutboxMessageProcessor>();

            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:RedisAddress"];
                options.InstanceName = "FoodAppShared_";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(config["Redis:RedisAddress"]!)); 

            services.AddTransient<IRedisLockService, RedisLockService>();

            services.AddSingleton<IMinioClient>(sp =>
            {
                return new MinioClient()
                    .WithEndpoint(config["Minio:Endpoint"] ?? "localhost:9000") 
                    .WithCredentials(config["Minio:AccessKey"], config["Minio:SecretKey"])
                    .WithSSL(false)
                    .Build();
            });

           
            services.AddGrpcClient<GeminiFoodlyGrpc.GeminiFoodlyGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCPort_TrackingService"]!);
            });

            services.AddGrpc();

            
            services.AddControllers();

            return services;
        }
    }
}
