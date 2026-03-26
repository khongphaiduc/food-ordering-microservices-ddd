using food_service.ProductService.API.gRPC;
using food_service.ProductService.API.Middlwares;
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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Serilog;
using StackExchange.Redis;
using trackingtService.API.Protos;

namespace food_service.ProductService.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {

            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            //serilog 
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            builder.Services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration["SQLFOOD_PRODUCTS"]!);
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


            builder.Services.AddRateLimiter(option =>
            {
                option.AddFixedWindowLimiter("rateFix", s =>
                {
                    s.Window = TimeSpan.FromSeconds(60);
                    s.PermitLimit = 1;
                    s.QueueLimit = 0;
                });
            });


            builder.Services.AddMassTransit(x =>
            {
                // register consumer 

                x.AddConsumer<RecommendFoodConsumer>();


                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ_Side_ProductService:Host"], h =>
                    {
                        h.Username(builder.Configuration["RabbitMQ_Side_ProductService:Username"]!);
                        h.Password(builder.Configuration["RabbitMQ_Side_ProductService:Password"]!);
                    });


                    // khai báo queue và liên kết với consumer RecommendFoodConsumer
                    cfg.ReceiveEndpoint("RecommendationFoodByAI_Queue", e =>              // tên queue 
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<RecommendFoodConsumer>(context);               // map consumer với queuen

                    });


                });

            });


            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IGetListProduct, GetListProduct>();
            builder.Services.AddScoped<IViewDetailProduct, ViewDetailProduct>();
            builder.Services.AddScoped<ICreateNewProduct, CreateNewProduct>();
            builder.Services.AddScoped<IUpdateCategory, UpdateCategory>();
            builder.Services.AddScoped<ICreateNewCategory, CreateNewCategory>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddSingleton<FoodProducer>();
            builder.Services.AddScoped<IUpdateProduct, UpdateProduct>();

            builder.Services.AddScoped<IOutBoxPatternProduct, OutBoxPatternProduct>();

            builder.Services.AddScoped<IMinIOFood, MinIOFood>();

            builder.Services.AddScoped<IProductRecommendationService, ProductRecommendationService>();
            builder.Services.AddScoped<IGetListCatgory, GetListCatgory>();

            builder.Services.AddScoped<GeminiModelFoodlyProducer>();
            //redis 

            builder.Services.AddTransient<IRedisLockService, RedisLockService>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:RedisAddress"];
                options.InstanceName = "FoodAppShared_";
            });

            // Redis lock
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
              ConnectionMultiplexer.Connect(builder.Configuration["RedisAddress"]!));

            //MinIO 

            builder.Services.AddSingleton<IMinioClient>(sp =>
            {
                return new MinioClient()
                    .WithEndpoint("localhost:9000")
                    .WithCredentials(builder.Configuration["Minio:AccessKey"], builder.Configuration["Minio:SecretKey"])
                    .WithSSL(false)
                    .Build();
            });



            builder.Services.AddScoped<IRecommenPersonalFood, RecommenPersonalFood>();
            builder.Services.AddGrpcClient<GeminiFoodlyGrpc.GeminiFoodlyGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:7139");
            });

            //builder.Services.AddSingleton<IMinioClient>(sp =>
            //{
            //    return new MinioClient()
            //        .WithEndpoint("103.161.119.162:9000")
            //        .WithCredentials("admin", "2hondaicodon")
            //        .WithSSL(false)
            //        .Build();
            //});


            builder.Services.AddGrpc();


            builder.Services.AddHostedService<OutboxMessageProcessor>();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapGrpcService<ProductInformationsServices>();

            app.MapGrpcService<LoadFullProduct>();

            app.UseRateLimiter();

            app.UseMiddleware<CustomGlobalException>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
