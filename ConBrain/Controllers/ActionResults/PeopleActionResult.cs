using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class PeopleActionResult : IActionResult
    {
        public PeopleActionResult(int offset, int size, string? pattern, UserDbContext context)
        {

            var temp = pattern == null || pattern == "" ? context.People : context.People
                .Where(i => i.Nick.Contains(pattern) || i.Name.Contains(pattern) || i.Family.Contains(pattern));
            _people = temp.Skip(offset)
                .Take(size).Select(i=>new PersonSavedMementor(i));
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_people);
        }

        private IEnumerable<PersonSavedMementor> _people;
    }
}
