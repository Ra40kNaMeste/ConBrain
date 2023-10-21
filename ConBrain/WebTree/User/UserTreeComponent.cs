using ConBrain.WebTree.User.Login;

namespace ConBrain
{
    public static class UserTreeComponent
    {
        public static void OnUserMap(IApplicationBuilder builder)
        {
            builder.Map("/register", UserRegisterTreeComponent.UserRegisterMap);
            builder.Map("/login", UserLoginTreeComponent.UserLoginMap);
            builder.Map("/reply", UserRegisterTreeComponent.OnReplyRegisterUserMap);
        }
    }
}
