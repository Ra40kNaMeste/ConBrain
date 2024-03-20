using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class DialogsResult : IActionResult
    {
        public DialogsResult(int[] ignores, int size, string? pattern, IEnumerable<DialogSavedMementor> dialog)
        {
            _dialogs = dialog
                .Where(i => !ignores.Contains(i.Id));
            if (pattern != null && pattern != "")
                _dialogs = _dialogs.Where(i => i.Name.Contains(pattern));
            _dialogs = _dialogs.Reverse().Take(size);
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_dialogs);
        }
        private readonly IEnumerable<DialogSavedMementor> _dialogs;
    }
}
