using ConBrain.Controllers;
using ConBrain.Controllers.Hubs;
using ConBrain.Loggers;
using ConBrain.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
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
            builder.Services.AddTransient((i) => new HandleNotAuthorizationMiddleware("login"));
            builder.Services.AddSignalR(o =>
            {
                o.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                o.KeepAliveInterval = TimeSpan.FromSeconds(15);
            });
            

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
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseJwtToken();
            app.UseHandleNotAuthorization();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            app.MapControllerRoute("login", "{controller=Authorization}/{action=Login}");
            app.MapHub<DialogHub>("/message");

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