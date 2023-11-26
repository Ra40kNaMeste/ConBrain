using ConBrain.Model;
using ConBrain.Tools;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ConBrain.WebTree
{
    public class UserRegisterTreeComponent
    {
        public static void UserRegisterMap(IApplicationBuilder builder)
        {
            builder.Run(async context =>
            {
                await ResponseOperations.SendHTMLFileMapByPath(context, "html/register.html");
            });
        }

        public static void OnReplyRegisterUserMap(IApplicationBuilder builder)
        {


            builder.Run(async (context) =>
            {
                var db = context.RequestServices.GetService<UserDbContext>();
                var logger = context.RequestServices.GetService<ILogger>();
                if (db == null)
                {
                    logger?.LogCritical(Properties.Resources.NotFoundDatabase);
                    throw new Exception(Properties.Resources.NotFoundDatabase);
                }
                var form = context.Request.Form;
                if (form == null)
                {
                    logger?.LogError(Properties.Resources.FormNotReceivedError);
                    Results.BadRequest();
                }

                var person = await Task.Run(() => RequestOperations.AddPersonByForm(form, logger));
                try
                {
                    db.People.Add(person);
                    db.SaveChanges();
                    logger?.LogTrace(string.Format(Properties.Resources.AddPersonSuccesfully, person.Id));

                    var claims = new List<Claim>() { new Claim(ClaimTypes.Name, person.Name) };
                    var jwt = new JwtSecurityToken(issuer: "ConBrains", claims: claims, expires: DateTime.UtcNow.AddMinutes(10));
                    Results.Text(new JwtSecurityTokenHandler().WriteToken(jwt));
                }
                catch (DbUpdateException ex)
                {
                    Results.Conflict(ex);
                }

            });
        }

    }
}
