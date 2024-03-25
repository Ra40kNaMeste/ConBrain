using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ConBrain.Controllers.ActionResults
{
    public class DialogResult : IActionResult
    {
        public DialogResult(Dialog dialog) => _dialog = dialog;
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_dialog);
        }
        private readonly Dialog _dialog;
    }
}
