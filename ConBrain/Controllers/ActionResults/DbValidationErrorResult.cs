using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class DbValidationErrorResult : IActionResult
    {
        public DbValidationErrorResult(string message) 
        { 
            _message = message;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.HttpContext.Response.WriteAsync(_message);
        }
        private readonly string _message;
    }
}
