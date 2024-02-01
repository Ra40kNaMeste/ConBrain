using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class PersonActionResult : IActionResult
    {
        public PersonActionResult(PersonSavedMementor person)
        {
            _person = person;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync<PersonSavedMementor>(_person);
        }
        private PersonSavedMementor _person;
    }
}
