using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Infastructure.BackgroundServices;
using food_service.ProductService.Infastructure.ImplementService;
using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Consumers;
using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Producer;
using food_service.ProductService.Infastructure.MinIO;
using food_service.ProductService.Infastructure.Models;
using food_service.ProductService.Infastructure.ProducerRabbitMQ;
using food_service.ProductService.Infastructure.RedisService.RedisInterface;
using food_service.ProductService.Infastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using StackExchange.Redis;
using trackingtService.API.Protos;

namespace food_service.ProductService.Infastructure.Persistence
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
                        e.ConfigureConsumer<RecommendFoodConsumer>(context);       // map consumer với queue
                    });
                    // khi run thì nó sẽ bind exchange và queue dựa trên các type message của consumer đang map tới queue
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
            services.AddScoped<IGetListCatgory, GetListCatgory>();

            services.AddScoped<IMinIOFood, MinIOFood>();
            services.AddScoped<IRecommenPersonalFood, RecommenPersonalFood>();
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
                options.Address = new Uri("https://localhost:5003");
            });

            services.AddGrpc();

            
            services.AddControllers();

            return services;
        }
    }
}
