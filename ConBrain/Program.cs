using ConBrain.Loggers;
using ConBrain.Model;
using ConBrain.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
            builder.Services.AddSingleton(new UserDbContext(builder.Configuration.GetConnectionString("sqliteUsers"))!);

            builder.Services.AddSingleton<ILogger>(new ConsoleLogger());

            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
            app.Map("/user", UserTreeComponent.OnUserMap);
            app.Map("/css", ResponseOperations.ReadCssFilesMap);
            app.Run();
        }
    }
}