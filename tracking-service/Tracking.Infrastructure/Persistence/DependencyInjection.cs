using Microsoft.EntityFrameworkCore;
using productService.API.Protos;
using tracking_service.Tracking.API.gRPCimplement;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infrastructure.ImplementServices;
using tracking_service.Tracking.Infrastructure.Models;
using tracking_service.Tracking.Infrastructure.Repo;
using tracking_service.Tracking.Infrastructure.Worker;

namespace tracking_service.Tracking.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(config["SQL_URL"]);
            });


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis"];
                options.InstanceName = "FoodAppShared_";
            });


            services.AddGrpcClient<ProductListGrpc.ProductListGrpcClient>(options =>
            {
                options.Address = new Uri(config["gRPCAddress_FoodService"]!);
            });

            services.AddGrpc();

            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IUserBehaviorTracking, UserBehaviorTracking>();
            services.AddScoped<IProducerTracking, ProducerTracking>();
            services.AddScoped<IServiceAI, ServiceAI>();
            services.AddScoped<IGetBehaviourOfUser, GetBehaviourOfUser>();
            services.AddScoped<LoadFullProductService>();


            services.AddHostedService<TrackingConsumer>();

            return services;
        }
    }
}
