using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    [Authorize]
    public class DialogController:Controller
    {
        public DialogController(UserDbContext context) { _dbContext = context; }

        [Route("dialogs")]
        public IActionResult Dialogs()
        {
            var person = GetPersonByAuth();
            if(person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }

        [Route("dialog/{name}")]
        public IActionResult Dialog(string name)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var dialog = person.Dialogs.Where(i => i.Name == name).FirstOrDefault();
            if (dialog == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(new DialogData(person, dialog));
        }

        [HttpGet]
        [Route("dialogs/build")]
        public IActionResult BuildDialog()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }

        [HttpPost]
        [Route("dialogs/build")]
        public async Task<IActionResult> BuildDialog(string name, string[] people)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            if (_dbContext.Dialogs.Any(i => i.Name == name))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var dialog = new Dialog()
            {
                Name = name,
                Members = _dbContext.People.Where(i => people.Contains(i.Nick)).ToList()
            };
            dialog.Members.Add(person);
            _dbContext.Dialogs.Add(dialog);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i => i.Friends)
                .Include(i=>i.Dialogs)
                .FirstOrDefault(i => i.Nick == namePerson);
            return person;
        }
        private UserDbContext _dbContext;
    }
    public record class DialogData(Person person, Dialog Dialog);

}
