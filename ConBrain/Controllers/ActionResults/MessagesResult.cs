using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class MessagesResult : IActionResult
    {
        public MessagesResult(IEnumerable<MessageSavedMementor> messages) 
        {
            _messages = messages;
        }
        public MessagesResult(IEnumerable<MessageSavedMementor> messages, int lastId)
        {
            _messages = messages.Reverse().TakeWhile(i => i.Id != lastId).Reverse();
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_messages);
        }
        private readonly IEnumerable<MessageSavedMementor> _messages;
    }
}
