using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ConBrain.Controllers
{

    /// <summary>
    /// Контроллер для диалогов и всего, что с ними связано (в т.ч. чатов)
    /// </summary>
    [Authorize]
    public class DialogController:Controller
    {
        #region Dialogs
        public DialogController(UserDbContext context) { _dbContext = context; }

        /// <summary>
        /// Список всех доступных диалогов
        /// </summary>
        /// <returns></returns>
        [Route("dialogs")]
        public IActionResult Dialogs()
        {
            var person = GetPersonByAuth();
            if(person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }

        /// <summary>
        /// Список всех доступных диалогов
        /// </summary>
        /// <returns></returns>
        [Route("dialogs/get")]
        public IActionResult Dialog(int[] ignores, int size, string? pattern)
        {
            var person = GetPersonByAuthWithMessages();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return new DialogsResult(ignores, size, pattern, person.Dialogs);
        }

        /// <summary>
        /// Диалог по его имени
        /// </summary>
        /// <param name="name">Имя диалога</param>
        /// <returns></returns>
        [HttpGet]
        [Route("dialog/{name}")]
        public IActionResult Dialog(string name)
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i => i.Dialogs)
                .FirstOrDefault(i => i.Data.Nick == namePerson);

            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var dialog = person.Dialogs.Where(i => i.Name == name).FirstOrDefault();
            if (dialog == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(new DialogData(name));
        }

        /// <summary>
        /// Вызывает конструктор нового диалога
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("dialogs/build")]
        public IActionResult BuildDialog()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }

        /// <summary>
        /// Принимает параметры для создания нового диалога
        /// </summary>
        /// <param name="name">Название нового диалога</param>
        /// <param name="people">Ники входящих в него людей</param>
        /// <returns>Статус код</returns>
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
                Members = _dbContext.People.Where(i => people.Contains(i.Data.Nick)).ToList()
            };
            dialog.Members.Add(person);
            _dbContext.Dialogs.Add(dialog);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #endregion //Dialogs

        #region Messagemethos
        /// <summary>
        /// Диапазон сообщений из диалога
        /// </summary>
        /// <param name="ignores">id шптщрируемых сообщений</param>
        /// <param name="size">количество сообщений</param>
        /// <param name="pattern">Паттерн поиска</param>
        /// <returns></returns>
        [HttpGet]
        [Route("dialog/{name}/messages")]
        public IActionResult Messages(string name, int[] ignores, int size, string? pattern)
        {
            var person = GetPersonByAuthWithMessages();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            var messages = person.Dialogs.Where(i => i.Name == name).FirstOrDefault()?.Messages;
            if (messages == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            return new MessagesResult(ignores, size, pattern, messages);
        }

        #endregion //Messagemethods

        #region PrivateMethods
        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i=>i.Data)
                .Include(i => i.Friends)
                .Include(i => i.Dialogs)
                .FirstOrDefault(i => i.Data.Nick == namePerson);
            return person;
        }
        private Person? GetPersonByAuthWithMessages()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;

            var person = _dbContext.People
                .Include(i => i.Data)
                .Include(i => i.Dialogs)
                    .ThenInclude(i => i.Messages)
                        .ThenInclude(i=>i.Sender)
                            .ThenInclude(i=>i.Data)
                .FirstOrDefault(i => i.Data.Nick == namePerson);
            return person;
        }
        #endregion //PrivateMethods
        private UserDbContext _dbContext;
    }
    public record class DialogData(string Name);

}
