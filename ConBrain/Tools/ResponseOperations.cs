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
    }
}
