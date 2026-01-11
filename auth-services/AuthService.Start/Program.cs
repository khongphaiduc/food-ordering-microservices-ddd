using auth_services.AuthService.API.Middlwares;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Reposistory;
using auth_services.AuthService.Infastructure.Security;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using auth_services.AuthService.Infastructure.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace auth_services.AuthService.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodAuthContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["URLSQLFOOD_ORDRING_AUTH_SERVICE"]);
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("AccessToken", option =>  // đặt tên secheme là TokenValue
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
            }).AddJwtBearer("RefreshToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:RefreshToken"]!))
                };
            });

            builder.Services.AddHttpContextAccessor();   // cung cấp httcontext trong service

            builder.Services.AddScoped<IGenarateSalt, GenarateSalt>();
            builder.Services.AddScoped<IHashPassword, HashPassword>();
            builder.Services.AddScoped<ISignUpUser, SignUpUser>();
            builder.Services.AddScoped<ICheckLogin, CheckLogin>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProvideAccessToken, ProvideAccessToken>();
            builder.Services.AddScoped<IUserLogOut, UserLogOut>();
            // token
            builder.Services.AddScoped<IGanarateTokenJWT, GanarateAccessTokenJWT>();
            builder.Services.AddScoped<IGanarateTokenJWT, GanarateRefresheTokenJWT>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();



            builder.Services.AddControllers();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<GlobalExceptionCustom>();
            app.MapControllers();

            app.Run();
        }
    }
}
