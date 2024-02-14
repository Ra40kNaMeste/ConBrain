using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers.ActionResults
{
    public class PeopleActionResult : IActionResult
    {
        public PeopleActionResult(int offset, int size, string? pattern, UserDbContext context)
        {

            var temp = pattern == null || pattern == "" ? context.PersonData : context.PersonData
                .Where(i => i.Nick.Contains(pattern) || i.Name.Contains(pattern) || i.Family.Contains(pattern));
            _people = temp.Skip(offset)
                .Take(size);
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(_people);
        }

        private IEnumerable<Model.PersonData> _people;
    }
}
