using ConBrain.Tools;

namespace ConBrain
{
    public class UserRegisterTreeComponent
    {
        public static void UserRegisterMap(IApplicationBuilder builder)
        {
            ResponseOperations.SendHTMLFileMap(builder, "html/register.html");
        }
    }
}
