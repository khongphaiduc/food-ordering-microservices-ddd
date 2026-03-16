using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Infastructrure.Models;

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

          
            builder.Services.AddControllers();

            var app = builder.Build();

          

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
