using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class MessagesResult : IActionResult
    {
        public MessagesResult(IEnumerable<MessageSavedMementor> messages, int start, int count) 
        {
            _messages = messages.Skip(start).Take(count);
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_messages);
        }
        private readonly IEnumerable<MessageSavedMementor> _messages;
    }
}
