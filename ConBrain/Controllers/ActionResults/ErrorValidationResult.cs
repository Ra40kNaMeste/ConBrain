using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ConBrain.Controllers.ActionResults
{
    public class ErrorValidationResult : IActionResult
    {
        public ErrorValidationResult(IEnumerable<ValidationResult> results) 
        { 
            _results = results;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.HttpContext.Response.WriteAsJsonAsync(_results);
        }
        private readonly IEnumerable<ValidationResult> _results;
    }
}
