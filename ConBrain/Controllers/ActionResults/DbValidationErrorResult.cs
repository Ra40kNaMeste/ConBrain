using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ConBrain.Controllers.ActionResults
{
    public class DbValidationErrorResult : IActionResult
    {
        public DbValidationErrorResult(Exception DbException) 
        { 
            _results = new List<ValidationResult>();
            var innerException = DbException.InnerException;
            if (innerException != null)
            {
                if (innerException.Message.Contains(nameof(PersonData.Nick)))
                    _results.Add(new("A user with this nick already exists", new List<string>() { nameof(PersonData.Nick).ToLower() }));
                if (innerException.Message.Contains(nameof(PersonData.Phone)))
                    _results.Add(new("A user with this phone already exists", new List<string>() { nameof(PersonData.Phone).ToLower() }));
            }
            if (_results.Count == 0)
                _results.Add(new("It is unknow append error", new List<string>() { nameof(PersonData.Nick).ToLower() }));
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.HttpContext.Response.WriteAsJsonAsync(_results);
        }
        private readonly IList<ValidationResult> _results;
    }
}
