
namespace ConBrain
{
    public class HandleNotAuthorizationMiddleware : IMiddleware
    {
        public HandleNotAuthorizationMiddleware(string url)
        {
            _url = url;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next.Invoke(context);
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                context.Response.Redirect(_url);
        }
        private string _url;
    }
    public static class HandleNotAuthorizationExtension
    {
        public static IApplicationBuilder UseHandleNotAuthorization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HandleNotAuthorizationMiddleware>();
        }
    }
}
