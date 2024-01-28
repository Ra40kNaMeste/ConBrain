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
                    // ���������, ����� �� �������������� �������� ��� ��������� ������
                    ValidateIssuer = true,
                    // ������, �������������� ��������
                    ValidIssuer = config?.Issures,
                    // ����� �� �������������� ����������� ������
                    ValidateAudience = true,
                    // ��������� ����������� ������
                    ValidAudience = config?.Audience,
                    // ����� �� �������������� ����� �������������
                    ValidateLifetime = true,
                    // ��������� ����� ������������
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config?.Key)),
                    // ��������� ����� ������������
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

            app.Run();
        }
    }
}