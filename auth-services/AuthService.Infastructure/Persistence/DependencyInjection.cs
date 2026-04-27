using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.BackgroundServices;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infastructure.Reposistory;
using auth_services.AuthService.Infastructure.Security;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using auth_services.AuthService.Infastructure.Tokens;
using auth_services.AuthService.Start;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Threading.RateLimiting;
using UserService.API.Protos;

namespace auth_services.AuthService.Infastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<FoodAuthContext>(options =>
             {
                 options.UseSqlServer(configuration["URLSQLFOOD_ORDRING_AUTH_SERVICE"]);
             });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("AccessToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key:AccessToken"]!))
                };
            }).AddJwtBearer("RefreshToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key:RefreshToken"]!))
                };
            });



            services.AddRateLimiter(options =>
            {
                options.AddPolicy("token", context =>
                    RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: context.User.Identity?.Name ?? "anonymous",
                        factory: _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 10,
                            TokensPerPeriod = 5,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
            });


            services.AddGrpcClient<UserInfoGrpc.UserInfoGrpcClient>(s =>
            {
                s.Address = new Uri("https://localhost:5001");
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:Host"];
                options.InstanceName = "Foodly";
            });

            services.AddHttpContextAccessor();

            services.AddValidatorsFromAssemblyContaining<Program>();  // scan all project 
            services.AddFluentValidationAutoValidation();            // auto

            services.AddScoped<IGenarateSalt, GenarateSalt>();
            services.AddScoped<IHashPassword, HashPassword>();
            services.AddScoped<ISignUpUser, SignUpUser>();
            services.AddScoped<ICheckLogin, CheckLogin>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProvideAccessToken, ProvideAccessToken>();
            services.AddScoped<IUserLogOut, UserLogOut>();
            services.AddSingleton<UserServicesClient>();

          
            services.AddScoped<IGanarateTokenJWT, GanarateAccessTokenJWT>();
            services.AddScoped<IGanarateTokenJWT, GanarateRefresheTokenJWT>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<RabbitMQProducer>();

            services.AddScoped<IOutBoxMessage, OutBoxMessage>();

            services.AddHostedService<OutBoxWorker>();
            services.AddScoped<IAddAccountStaffs, AddAccountStaffs>();
            services.AddScoped<IAddSafttRepository, AddSafttRepository>();
            services.AddScoped<IGetListStaff, GetListStaff>();

            return services;
        }
    }
}
