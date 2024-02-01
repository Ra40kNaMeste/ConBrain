using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class FriendsActionResult : IActionResult
    {
        public FriendsActionResult(IEnumerable<string> people)=> _people = people;
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_people);
        }
        private readonly IEnumerable<string> _people;
    }
}
