using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ConBrain.Controllers.Hubs
{
    /// <summary>
    /// Хаб всех диалогов
    /// Имеет входы: Subscribe, Send
    /// Имеет выходы: Message
    /// </summary>
    [Authorize]
    public class DialogHub:Hub
    {
        public DialogHub(UserDbContext context)
        {
            _context = context;
        } 

        /// <summary>
        /// Подписка на диалог
        /// </summary>
        /// <param name="dialogName">название диалога</param>
        /// <returns></returns>
        public async Task Subscribe(string dialogName)
        {
            var person = GetAuthorizePerson();
            if (person == null)
            {
                WriteStatusCode(StatusCodes.Status401Unauthorized);
                return;
            }
            var dialog = person?.Dialogs.Where(i => i.Name == dialogName).FirstOrDefault();
            if (dialog == null)
            {
                WriteStatusCode(StatusCodes.Status400BadRequest);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, dialog.Id.ToString());
        }

        /// <summary>
        /// Отправка сообщения в диалог
        /// </summary>
        /// <param name="message">Тело сообщения</param>
        /// <param name="dialogName">имя диалога</param>
        /// <returns></returns>
        public async Task Send(string message, string  dialogName)
        {
            
            var person = GetAuthorizePerson();
            if (person == null)
            {
                WriteStatusCode(StatusCodes.Status401Unauthorized);
                return;
            }

            var dialog = person?.Dialogs.Where(i => i.Name == dialogName).FirstOrDefault();
            if (dialog == null)
            {
                WriteStatusCode(StatusCodes.Status400BadRequest);
                return;
            }
                
            var mess = new Message()
            {
                Body = message,
                DateTime = DateTime.Now,
                Dialog = dialog,
                Sender = person
            };

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(mess, new(mess), validationResults, true))
            {
                WriteStatusCode(StatusCodes.Status400BadRequest);
                return;
            }
            try
            {
                _context.Messages.Add(mess);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteStatusCode(StatusCodes.Status400BadRequest);
                return;
            }
            await Clients.Group(dialog.Id.ToString()).SendAsync("Message", new MessageSavedMementor(mess));
        }

        /// <summary>
        /// Записывает статус-коды ответа
        /// </summary>
        /// <param name="errorStatusCode"></param>
        private void WriteStatusCode(int errorStatusCode)
        {
            var context = Context.GetHttpContext();
            if (context != null)
                context.Response.StatusCode = errorStatusCode;
        }

        /// <summary>
        /// Возвращает авторизированного пользователя
        /// </summary>
        /// <returns></returns>
        private Person? GetAuthorizePerson()
        {
            var nick = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (nick == null)
                return null;
            return _context.People
                .Include(i=>i.Data)
                .Include(i => i.Dialogs)
                    .ThenInclude(i => i.Messages)
                .Where(i => i.Data.Nick == nick).FirstOrDefault();
        }

        private UserDbContext _context;
    }
}
