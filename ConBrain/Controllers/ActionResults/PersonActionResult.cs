using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class PersonActionResult : IActionResult
    {
        public PersonActionResult(Model.PersonData person)
        {
            _person = person;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_person);
        }
        private Model.PersonData _person;
    }
}
