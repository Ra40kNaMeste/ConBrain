using ConBrain.Model;
using ConBrain.Tools;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ConBrain
{
    public static class UserReplyTreeComponent
    {
        public static void OnReplyRegisterUserMap(IApplicationBuilder builder)
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

        public static void OnReplyLoginUserMapPost(UserDbContext db, HttpContext context, ILogger logger)
        {
            var form = context.Request.Form;
            string? login = form["loginOrNick"];
            if(login == null)
                logger?.LogWarning(string.Format(Properties.Resources.FormNotFindValueWarning, "loginOrNick"));
            var person = db.People.Where(i => i.Nick == login || i.Phone == login).FirstOrDefault();
            if(person == null)
            {
                logger?.LogError(Properties.Resources.NotFoundPersonWarning);
                return;
            }
            string? pass = form["pass"];
            if(pass != null && person.Password == pass)
            {
                var claims = new List<Claim>() { new Claim(ClaimTypes.Name, login!) };
                var jwt = new 
            }
        }
    }
}
