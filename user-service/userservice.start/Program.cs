using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using user_service.userservice.api.GlobalExceptionMiddleware;
using user_service.userservice.application.interfaceApplications;
using user_service.userservice.domain.interfaces;
using user_service.userservice.infastructure.DBcontextService;
using user_service.userservice.infastructure.other;
using user_service.userservice.infastructure.Repositories;

namespace user_service.userservice.start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodUsersContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["URL_SQL_USER_SERVICE"]);
            });

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });



            builder.Services.AddTransient<IUserRepositories, UserRepositories>();
            builder.Services.AddTransient<IValidationJWT, ValidationJWT>();
            builder.Services.AddTransient<IAddUserApplication, AddUserApplication>();
            builder.Services.AddTransient<IAddAddressForUserApplication, AddAddressForUserApplication>();



            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptions>();  // đăng ký vào middleware


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
