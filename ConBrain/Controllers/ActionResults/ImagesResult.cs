using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class ImagesResult : IActionResult
    {

        public ImagesResult(int[] ignores, int size, string? pattern, IEnumerable<Image> images)
        {
            _images = images
                .Where(i => !ignores.Contains(i.Id));
            if (pattern != null && pattern != "")
                _images = _images.Where(i => i.Name.Contains(pattern));
            _images = _images.Take(size);
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_images);
        }
        private readonly IEnumerable<Image> _images;
    }
}
