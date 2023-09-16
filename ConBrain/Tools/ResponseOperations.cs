namespace ConBrain.Tools
{
    public static class ResponseOperations
    {
        public static async Task SendHTMLFileMapByPath(HttpContext context, string path)
        {
            var response = context.Response;
            response.ContentType = "text/html; charset=utf-8";
            await response.SendFileAsync(path);
        }

        public static async Task SendHTMLFileMap(HttpContext context, string file)
        {
            var response = context.Response;
            response.ContentType = "text/html; charset=utf-8";
            await response.WriteAsync(file);
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
