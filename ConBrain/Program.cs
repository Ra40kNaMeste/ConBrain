using ConBrain.Controllers;
using ConBrain.Loggers;
using ConBrain.Model;
using ConBrain.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace ConBrain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UserDbContext>((options)=>options.UseSqlite(builder.Configuration.GetConnectionString("sqliteUsers")));
            builder.Services.AddMvc();
            builder.Services.AddTransient<JwtTokenMiddleware>();
            

            builder.Services.AddSingleton<ILogger>(new ConsoleLogger());
            var config = builder.Configuration.GetSection("Authorization").Get<AuthorizationSettings>();
            

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = config?.Issures,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = config?.Audience,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config?.Key)),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true
                };
            });
            builder.Services.AddAuthorization(c =>
            {
                
            });

            var app = builder.Build();
            app.UseJwtToken();

            app.UseAuthentication();
            app.UseAuthorization();
            
            
            
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            app.MapControllerRoute("login", "{controller=Authorization}/{action=Login}");

            app.UseStaticFiles();

            IHostEnvironment? env = app.Services.GetService<IHostEnvironment>();
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/node_modules",
                EnableDirectoryBrowsing = false
            });

            app.Run();
        }
    }
}