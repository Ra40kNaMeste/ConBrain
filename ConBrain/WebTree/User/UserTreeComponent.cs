namespace ConBrain
{
    public static class UserTreeComponent
    {
        public static void OnUserMap(IApplicationBuilder builder)
        {
            builder.Map("/register", UserRegisterTreeComponent.UserRegisterMap);
            builder.Map("/reply", UserReplyTreeComponent.OnReplyRegisterUserMap);
        }
    }
}
