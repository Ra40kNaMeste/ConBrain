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


        public static void OnReplyLoginUserMapPost(IApplicationBuilder builder)
        {
            builder.Run(async (context) =>
            {
                var logger = context.RequestServices.GetService<ILogger>();
                var db = context.RequestServices.GetService<UserDbContext>();

                var form = context.Request.Form;
                string? login = form["loginOrNick"];
                if (login == null)
                    logger?.LogWarning(string.Format(Properties.Resources.FormNotFindValueWarning, "loginOrNick"));
                var person = db?.People.Where(i => i.Nick == login || i.Phone == login).FirstOrDefault();
                if (person == null)
                {
                    logger?.LogError(Properties.Resources.NotFoundPersonWarning);
                    Results.NoContent();
                }
                string? pass = form["pass"];
                if (pass != null && person.Password == pass)
                {
                    var claims = new List<Claim>() { new Claim(ClaimTypes.Name, login!) };
                    var jwt = new JwtSecurityToken(issuer: "ConBrains", claims: claims, expires: DateTime.UtcNow.AddMinutes(10));
                    Results.Content(new JwtSecurityTokenHandler().WriteToken(jwt));
                }
                Results.NoContent();
            });


        }
    }
}
