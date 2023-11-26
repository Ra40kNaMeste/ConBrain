using ConBrain.Loggers;
using ConBrain.Model;
using ConBrain.Tools;
using ConBrain.WebTree;
using ConBrain.WebTree.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ConBrain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UserDbContext>((options)=>options.UseSqlite(builder.Configuration.GetConnectionString("sqliteUsers")));
            builder.Services.AddMvc();
            

            builder.Services.AddSingleton<ILogger>(new ConsoleLogger());
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
            });

            var app = builder.Build();
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

            app.UseStaticFiles();


            app.Run();
        }
    }
}