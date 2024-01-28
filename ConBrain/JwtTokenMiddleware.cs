
using System.Linq;
using System.Text;

namespace ConBrain
{
    public class JwtTokenMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Cookies.Keys.Contains("token"))
                context.Request.Headers.Add("Authorization", "Bearer " + context.Request.Cookies["token"]);
            await next.Invoke(context);
        }
    }
    public static class JwwTokenExtension
    {
        public static IApplicationBuilder UseJwtToken(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenMiddleware>();
        }
    }
}
