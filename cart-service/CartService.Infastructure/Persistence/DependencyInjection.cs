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

namespace cart_service.CartService.Infastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(configuration["URLCARTSQL"]);
            });

            services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key:AccessToken"]!)
                    )
                };
            });

            services.AddScoped<IMapModel, MapModel>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICreateNewCart, CreateNewCart>();
            services.AddScoped<IUpdateCartFood, UpdateCartFood>();
            services.AddScoped<IGetCartForUser, GetCartForUser>();
            services.AddScoped<CartInforService>();

            services.AddControllers();

            services.AddScoped<CartServiceClient>();

            services.AddGrpcClient<ProductInfoGrpc.ProductInfoGrpcClient>(s =>
            {
                s.Address = new Uri("https://localhost:5002");
            });

            services.AddGrpc();
            return services;
        }
    }
}
