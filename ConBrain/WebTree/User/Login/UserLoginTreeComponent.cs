using ConBrain.Tools;

namespace ConBrain.WebTree.User.Login
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
    }
}
