using ConBrain.Loggers;
using ConBrain.Model;
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
            app.Map("/user", OnUserMap);
            app.Map("/css", ReadCssFiles);
            app.Run();
        }

        private static void OnUserMap(IApplicationBuilder builder)
        {
            builder.Map("/register", (IApplicationBuilder b) =>
            {
                b.Map("/reply", OnReplyRegisterUserMap);
                SendHTMLFileMap(b, "html/register.html");
                
            });
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
        private static void OnReplyRegisterUserMap(IApplicationBuilder builder)
        {
            builder.Run(async (context) =>
            {
                var logger = builder.ApplicationServices.GetService<ILogger>();

                var form = context?.Request.Form;
                if (form == null)
                {
                    logger?.LogError(Properties.Resources.FormNotReceivedError);
                    return;
                }

                var person = await Task.Run(() => AddPersonByForm(form, logger));

                var db = builder.ApplicationServices.GetService<UserDbContext>();
                if (db == null)
                {
                    logger?.LogCritical(Properties.Resources.NotFoundDatabase);
                    throw new Exception(Properties.Resources.NotFoundDatabase);
                }

                db.People.Add(person);
                db.SaveChanges();
            });
        }
        private static Person AddPersonByForm(IFormCollection form, ILogger? logger = null)
        {
            return new Person()
            {
                Name = getFormValue(form, "name", logger),
                Family = getFormValue(form, "family", logger),
                LastName = getFormValue(form, "secondName", logger),
                Nick = getFormValue(form, "nick", logger),
                Phone = getFormValue(form, "tel", logger),
                Password = getFormValue(form, "pass", logger)
            };
        }

        private static string getFormValue(IFormCollection form, string nameProperty, ILogger? logger = null)
        {
            var values = form[nameProperty];
            if(values.Count == 0)
            {
                logger?.LogWarning(string.Format(Properties.Resources.FormNotFindValueWarning, nameProperty));
                return "";
            }
            if (values.Count > 0)
                logger?.LogWarning(string.Format(Properties.Resources.FormFoundServialValuesWarning, nameProperty));
            string res = values.First() ?? string.Empty;
            logger?.LogDebug(string.Format(Properties.Resources.ReadPorpertyByForm, nameProperty, res));
            return res;
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