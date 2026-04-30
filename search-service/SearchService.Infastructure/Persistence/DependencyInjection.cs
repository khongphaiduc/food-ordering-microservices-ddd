using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using search_service.Models;
using search_service.SearchService.Application.Interface;
using search_service.SearchService.Infastructure.ConsumerRabbitMQ;
using search_service.SearchService.Infastructure.ImplementServices;
using search_service.SearchService.Infastructure.Redis.Interface;
using search_service.SearchService.Infastructure.Redis.Service;
using StackExchange.Redis;

namespace search_service.SearchService.Infastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(config["SQLPRODUCT"]);
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
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["HostRedis"];
                options.InstanceName = "ListProduct";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(config["HostRedis"]!));

            services.AddScoped<IRedisLockService, RedisLockService>();


            var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
                .DefaultIndex("products")
                .Authentication(new BasicAuthentication("elastic", "tRKT9*XFLHZog*Wzmd2v"))
                .ServerCertificateValidationCallback((sender, cert, chain, errors) => true);

            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);
            services.AddSingleton<ISuggestSearch, SuggestSearch>();
            services.AddScoped<IElasticsearch, Elasticsearch>();

            services.AddHostedService<ElasticsearchConsumer>();


            services.AddScoped<IGetListProduct, GetListProduct>();
            services.AddScoped<ILoadFullProduct, LoadFullProduct>();


            services.AddControllers();

            return services;
        }
    }
}
