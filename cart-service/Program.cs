using cart_service.CartService.API.gRPC;
using cart_service.CartService.Infrastructure.Models;
using cart_service.CartService.Infrastructure.Persistence;
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
                    logger.LogInformation("Ðã ch?y Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ðã x?y ra l?i trong quá trình t? d?ng migrate database.");
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
