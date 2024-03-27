using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class ImageResult : IActionResult
    {
        public ImageResult(Image image) => _image = image;

        async public Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_image);
        }

        private Image _image;
    }
}
