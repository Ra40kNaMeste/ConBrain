using ConBrain.Controllers.ActionResults;
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

        [HttpGet]
        [Route("dialog/{name}")]
        public IActionResult Dialog(string name)
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i => i.Friends)
                .Include(i => i.Dialogs)
                    .ThenInclude(i=>i.Members)
                .FirstOrDefault(i => i.Nick == namePerson);

            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var dialog = person.Dialogs.Where(i => i.Name == name).FirstOrDefault();
            if (dialog == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(new DialogData(person, dialog));
        }

        #region Messagemethos
        [HttpGet]
        [Route("dialog/{name}/messages")]
        public IActionResult Messages(string name, int start, int count)
        {
            var person = GetPersonByAuthWithMessages();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var messages = person.Dialogs.Where(i => i.Name == name).FirstOrDefault()?.Messages;
            if (messages == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new MessagesResult(messages
                .Select(i=>new MessageSavedMementor(i))
                .Reverse()
                .Skip(start)
                .Take(count));
        }

        [HttpGet]
        [Route("dialog/{name}/newmessages")]
        public IActionResult NewMessages(string name, int id)
        {
            var person = GetPersonByAuthWithMessages();

            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var messages = person.Dialogs.Where(i => i.Name == name).FirstOrDefault()?.Messages;
            if (messages == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new MessagesResult(messages.Select(i => new MessageSavedMementor(i))
                .Reverse()
                .TakeWhile(i => i.Id != id)
                .Reverse());
        }

        [HttpGet]
        [Route("dialog/{name}/oldmessages")]
        public IActionResult OldMessages(string name, int id, int count)
        {
            var person = GetPersonByAuthWithMessages();

            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var messages = person.Dialogs.Where(i => i.Name == name).FirstOrDefault()?.Messages;
            if (messages == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new MessagesResult(messages.Select(i => new MessageSavedMementor(i))
                .Reverse()
                .SkipWhile(i => i.Id != id)
                .Take(count));
        }

        [HttpPost]
        [Route("dialog/{name}/messages")]
        public async Task<IActionResult> Messages(string name, string body)
        {
            var person = GetPersonByAuthWithMessages();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var dialog = person.Dialogs.Where(i => i.Name == name).FirstOrDefault();
            if(dialog == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var message = new Message()
            {
                Body = body,
                DateTime = DateTime.Now,
                Dialog = dialog, 
                Sender = person
            };
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }
        #endregion //Messagemethods

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
                .Include(i => i.Dialogs)
                .FirstOrDefault(i => i.Nick == namePerson);
            return person;
        }
        private Person? GetPersonByAuthWithMessages()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i => i.Dialogs)
                    .ThenInclude(i => i.Messages)
                        .ThenInclude(i=>i.Sender)
                .FirstOrDefault(i => i.Nick == namePerson);
            return person;
        }

        private UserDbContext _dbContext;
    }
    public record class DialogData(Person person, Dialog Dialog);

}
