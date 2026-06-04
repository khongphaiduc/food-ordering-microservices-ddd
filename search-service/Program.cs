using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using search_service.Models;
using search_service.SearchService.API.Middlware;
using search_service.SearchService.Application.Interface;
using search_service.SearchService.Infrastructure.ConsumerRabbitMQ;
using search_service.SearchService.Infrastructure.ImplementServices;
using search_service.SearchService.Infrastructure.Persistence;
using search_service.SearchService.Infrastructure.Redis.Interface;
using search_service.SearchService.Infrastructure.Redis.Service;
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
                    logger.LogInformation("Ðã ch?y Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ðã x?y ra l?i trong quá trình t? d?ng migrate database.");
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
