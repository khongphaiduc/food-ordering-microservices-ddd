using cart_service.CartService.API.gRPC;
using cart_service.CartService.Infastructure.Models;
using cart_service.CartService.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cart_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddPersistence(builder.Configuration);
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodProductsDbContext>();
                    context.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Đã chạy Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đã xảy ra lỗi trong quá trình tự động migrate database.");
                }
            }
            app.MapGrpcService<CartInforService>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
