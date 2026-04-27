using cart_service.CartService.API.gRPC;
using cart_service.CartService.Application.Services;
using cart_service.CartService.Domain.Interface;
using cart_service.CartService.Infastructure.ImplementServices;
using cart_service.CartService.Infastructure.Mapper;
using cart_service.CartService.Infastructure.Models;
using cart_service.CartService.Infastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using productService.API.Protos;

namespace cart_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration["URLCARTSQL"]);
            });


            builder.Services.AddAuthentication(options =>
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
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:AccessToken"]!))
               };
           });

            builder.Services.AddScoped<IMapModel, MapModel>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICreateNewCart, CreateNewCart>();
            builder.Services.AddScoped<IUpdateCartFood, UpdateCartFood>();
            builder.Services.AddScoped<IGetCartForUser, GetCartForUser>();
            builder.Services.AddScoped<CartInforService>();
            builder.Services.AddControllers();


            builder.Services.AddScoped<CartServiceClient>();

            builder.Services.AddGrpcClient<ProductInfoGrpc.ProductInfoGrpcClient>(s =>
            {
                s.Address = new Uri("https://localhost:5002");
            });

            builder.Services.AddGrpc();
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
