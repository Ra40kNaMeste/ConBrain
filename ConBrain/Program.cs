using ConBrain.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConBrain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton(new UserDbContext(builder.Configuration.GetSection("ConnectionStrings.sqliteUsers").Value!));
            
            var app = builder.Build();            

            app.Map("/user", OnUserMap);

            app.Run();
        }

        private static void OnUserMap(IApplicationBuilder builder)
        {
            builder.Map("/register", (IApplicationBuilder b) => SendHTMLFileMap(b, "register.html"));

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