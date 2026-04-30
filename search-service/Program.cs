using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using search_service.Models;
using search_service.SearchService.API.Middlware;
using search_service.SearchService.Application.Interface;
using search_service.SearchService.Infastructure.ConsumerRabbitMQ;
using search_service.SearchService.Infastructure.ImplementServices;
using search_service.SearchService.Infastructure.Persistence;
using search_service.SearchService.Infastructure.Redis.Interface;
using search_service.SearchService.Infastructure.Redis.Service;
using StackExchange.Redis;

namespace search_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAllServices(builder.Configuration);
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


            app.UseMiddleware<GlobalException>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
