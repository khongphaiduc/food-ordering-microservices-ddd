using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infastructrure.ImplementServices;
using tracking_service.Tracking.Infastructrure.Models;
using tracking_service.Tracking.Infastructrure.Repo;
using tracking_service.Tracking.Infastructrure.Worker;

namespace tracking_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration["SQL_URL"]);
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis"];
                options.InstanceName = "FoodUserBehaviorTracking";
            });

            builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            builder.Services.AddScoped<IUserBehaviorTracking, UserBehaviorTracking>();
            builder.Services.AddScoped<IProducerTracking, ProducerTracking>();
            builder.Services.AddScoped<IServiceAI, ServiceAI>();
            builder.Services.AddHostedService<TrakingComsumer>();



            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
