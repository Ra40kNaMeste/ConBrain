using ConBrain.Model;
using ConBrain.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConBrain.WebTree.Login
{
    public class UserLoginTreeComponent
    {
        public static void UserLoginMap(IApplicationBuilder builder)
        {
            builder.Run(async context =>
            {
                await ResponseOperations.SendHTMLFileMapByPath(context, "html/login.html");
            });
        }


        public static string OnReplyLoginUserMapPost(UserDbContext db, HttpContext context, ILogger logger)
        {
            var form = context.Request.Form;
            string? login = form["loginOrNick"];
            if (login == null)
                logger?.LogWarning(string.Format(Properties.Resources.FormNotFindValueWarning, "loginOrNick"));
            var person = db.People.Where(i => i.Nick == login || i.Phone == login).FirstOrDefault();
            if (person == null)
            {
                logger?.LogError(Properties.Resources.NotFoundPersonWarning);
                return string.Empty;
            }
            string? pass = form["pass"];
            if (pass != null && person.Password == pass)
            {
                var claims = new List<Claim>() { new Claim(ClaimTypes.Name, login!) };
                var jwt = new JwtSecurityToken(issuer: "ConBrains", claims: claims, expires: DateTime.UtcNow.AddMinutes(10));
                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            return string.Empty;
        }
    }
}
