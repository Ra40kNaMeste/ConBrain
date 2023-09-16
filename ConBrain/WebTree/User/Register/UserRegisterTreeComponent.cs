using ConBrain.Tools;

namespace ConBrain
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
    }
}
