using ConBrain.Model;
using ConBrain.Tools;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ConBrain
{
    public static class UserReplyTreeComponent
    {
        public static void OnReplyUserMap(IApplicationBuilder builder)
        {
            builder.Run(async (context) =>
            {
                var logger = builder.ApplicationServices.GetService<ILogger>();

                var form = context.Request.Form;
                if (form == null)
                {
                    logger?.LogError(Properties.Resources.FormNotReceivedError);
                    return;
                }

                var person = await Task.Run(() => RequestOperations.AddPersonByForm(form, logger));

                var db = builder.ApplicationServices.GetService<UserDbContext>();
                if (db == null)
                {
                    logger?.LogCritical(Properties.Resources.NotFoundDatabase);
                    throw new Exception(Properties.Resources.NotFoundDatabase);
                }
                try
                {
                    db.People.Add(person);
                    db.SaveChanges();
                    logger?.LogTrace(string.Format(Properties.Resources.AddPersonSuccesfully, person.Id));
                }
                catch (DbUpdateException ex)
                {
                    string property = Regex.Match(ex.InnerException.Message, @"\w*..$").Value;
                    property = property.Substring(0, property.Length - 2);
                    logger?.LogWarning(string.Format(Properties.Resources.AddPersonError, property));
                    string file = File.ReadAllText("html/errorAddPersonPage.html");
                    await ResponseOperations.SendHTMLFileMap(context, file.Replace("${property}", property));
                }

            });
        }

    }
}
