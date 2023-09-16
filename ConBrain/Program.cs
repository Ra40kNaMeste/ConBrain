using ConBrain.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConBrain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton(new UserDbContext(builder.Configuration.GetConnectionString("sqliteUsers"))!);
            
            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
            app.Map("/user", OnUserMap);
            app.Map("/css", ReadCssFiles);
            app.Run();
        }

        private static void OnUserMap(IApplicationBuilder builder)
        {
            builder.Map("/register", (IApplicationBuilder b) =>
            {
                SendHTMLFileMap(b, "html/register.html");
                
            });
            builder.Map("/css", ReadCssFiles);
        }
        private static void ReadCssFiles(IApplicationBuilder builder)
        {
            builder.Run(async (HttpContext context) =>
            {

                var response = context.Response;
                var request = context.Request;
                var path = Directory.GetCurrentDirectory() + "\\html\\css\\" + request.Path;
                await context.Response.SendFileAsync(path);
            });

                
        }
        private static void OnRegisterUserMap(IApplicationBuilder builder)
        {
            builder.Map("/reply", OnReplyRegisterUserMap);

        }
        private static void OnReplyRegisterUserMap(IApplicationBuilder builder)
        {
            //var context = builder.ApplicationServices.GetService<HttpContext>();
            //var person = (Person)context.Request.Form.First().Value;//Доделать
            //var db = builder.ApplicationServices.GetService<UserDbContext>();
            //db.People.Add(person);
            //db.Update(person);
            //db.SaveChanges();
        }
        private static void SendHTMLFileMap(IApplicationBuilder builder, string path)
        {
            builder.Run(async (context) =>
            {
                var respore = context.Response;
                respore.ContentType = "text/html; charset=utf-8";
                await respore.SendFileAsync(path);
            });
        }
    }
}