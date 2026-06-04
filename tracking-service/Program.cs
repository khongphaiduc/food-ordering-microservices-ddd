using Microsoft.EntityFrameworkCore;
using productService.API.Protos;
using tracking_service.Tracking.API.gRPCimplement;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infrastructure.ImplementServices;
using tracking_service.Tracking.Infrastructure.Models;
using tracking_service.Tracking.Infrastructure.Persistence;
using tracking_service.Tracking.Infrastructure.Repo;
using tracking_service.Tracking.Infrastructure.Worker;

namespace tracking_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAllServices(builder.Configuration);
            builder.Services.AddControllers();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodProductsDbContext>();
                    context.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Ðã ch?y Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ðã x?y ra l?i trong quá trình t? d?ng migrate database.");
                }
            }

            app.MapGrpcService<GetResultRecommendFoodService>();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
