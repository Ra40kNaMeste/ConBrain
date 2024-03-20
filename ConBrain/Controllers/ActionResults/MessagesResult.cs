using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class MessagesResult : IActionResult
    {
        public MessagesResult(int[] ignores, int size, string? pattern, IEnumerable<MessageSavedMementor> messages) 
        {
            _messages = messages
                .Where(i => !ignores.Contains(i.Id));
            if (pattern != null && pattern != "")
                _messages = _messages.Where(i => i.Body.Contains(pattern) || i.Sender?.Nick.Contains(pattern) == true);
            _messages = _messages.Reverse().Take(size);
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_messages);
        }
        private readonly IEnumerable<MessageSavedMementor> _messages;
    }
}
