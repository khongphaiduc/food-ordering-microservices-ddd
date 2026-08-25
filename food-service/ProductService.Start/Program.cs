using food_service.ProductService.API.gRPC;
using food_service.ProductService.API.Middlewares;


using food_service.ProductService.Infrastructure.Models;
using food_service.ProductService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;



namespace food_service.ProductService.Start
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
                    logger.LogInformation("Database migration completed successfully.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while applying database migrations.");
                }
            }

            app.MapGrpcService<ProductInformationsServices>();
            app.MapGrpcService<LoadFullProduct>();
            app.MapGrpcService<Inventory>();


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
