namespace ConBrain.Tools
{
    public static class ResponseOperations
    {
        public static void SendHTMLFileMap(IApplicationBuilder builder, string path)
        {
            builder.Run(async (context) =>
            {
                var response = context.Response;
                response.ContentType = "text/html; charset=utf-8";
                await response.SendFileAsync(path);
            });
        }

        public static void ReadCssFilesMap(IApplicationBuilder builder)
        {
            builder.Run(async (HttpContext context) =>
            {

                var response = context.Response;
                var request = context.Request;
                var path = Directory.GetCurrentDirectory() + "\\html\\css\\" + request.Path;
                await context.Response.SendFileAsync(path);
            });
        }
    }
}
