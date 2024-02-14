using ConBrain.Controllers.ActionResults;
using ConBrain.Extensions;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ConBrain.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        public PersonController(IConfiguration configuration, UserDbContext dbContext)
        {
            settings = configuration.GetSection("Authorization").Get<AuthorizationSettings>() ?? throw new FormatException();
            var pathSettings = configuration.GetSection("Paths").Get<PathSetting>();
            _avatarPath = pathSettings?.Avatar ?? "avatars";
            _defaultAvatarName = pathSettings?.DefaultAvatarName ?? "default.svg";
            _dbContext = dbContext;
            _imageSettings = configuration.GetSection("ImageSettings").Get<ImageSettings>() ?? throw new FormatException();
        }
        [Route("home")]
        public IActionResult Dates()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View("Person", person);
        }

        [Route("changepassword")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }

        [Route("changepassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordData data)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            if(person.Password != data.OldPassword)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            person.Password = data.Pass;

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new DbValidationErrorResult(ex.Message);
            }

            return new LoginResult(person, settings);
        }

        [Route("edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var person = GetPersonByAuth();
            if(person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person);
        }
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonData data)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            if(data.Nick != null)
                person.Nick = data.Nick;
            if (data.Name != null)
                person.Name = data.Name;
            if (data.Family != null)
                person.Family = data.Family;
            if (data.LastName != null)
                person.SecondName = data.LastName;
            if (data.Phone != null)
                person.Phone = data.Phone;
            if (data.AvatarPath != null)
                person.AvatarPath = data.AvatarPath;

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new DbValidationErrorResult(ex.Message);
            }
            return Redirect("/home");
        }

        [AllowAnonymous]
        [Route("id={id}")]
        public IActionResult PersonPage(string id)
        {
            var person = GetPerson(id);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View("Person", person);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("person")]
        public IActionResult Person(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new PersonActionResult(new(person));
        }

        [HttpGet]
        [Route("{id}/friends")]
        public IActionResult Friends(string id)
        {
            var person = GetPerson(id);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(person);
        }

        #region person/friends
        [HttpGet]
        [Route("person/friends")]
        public IActionResult GetFriend()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return new FriendsActionResult(person.Friends.Select(i => i.Friend.Nick));
        }
        [HttpPut]
        [Route("person/friends")]
        public async Task<IActionResult> AddFriend(string nick)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var friend = _dbContext.People.Where(i=>i.Nick == nick).FirstOrDefault();
            if (friend == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if(!person.Friends.Any(i=>i.Target == friend))
                _dbContext.FreindsList.Add(new FriendPerson() { Friend = friend, Target = person });
            _dbContext.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpDelete]
        [Route("person/friends")]
        public async Task<IActionResult> RemoveFriend(string nick)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var friend = _dbContext.People.Where(i => i.Nick == nick).FirstOrDefault();
            if (friend == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var paar = person.Friends.RemoveAll(i=>i.Friend == friend);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #endregion //person/friends

        /// <summary>
        ///Метод отправки изображения пользователя
        /// </summary>
        /// <param name="nick">Ник пользователя</param>
        /// <param name="key">Ключ изображения</param>
        /// <returns>Изображение</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("{nick}/image")]
        public IActionResult Image(string nick, string key)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var path = Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, nick);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //Копируем дефолтный файл
                System.IO.File.Copy(Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, _defaultAvatarName), Path.Combine(path, _defaultAvatarName));
            }
            path = Path.Combine(path, key);
            if (!System.IO.File.Exists(path))
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "image/jpeg");
        }

        //Метод для добавления изображения авторизированного пользователя
        [HttpPost]
        [Route("image")]
        public async Task<IActionResult> Image(IFormFile file, string key)
        {
            //Авторизированный пользователь
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            //Создание файла записи
            if(file == null || !file.CanImage())
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var path = Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, person.Nick);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //Копируем дефолтный файл
                System.IO.File.Copy(Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, _defaultAvatarName), Path.Combine(path, _defaultAvatarName));
            }
                
            path = Path.Combine(path, key);

            //Создание изображение, его обрезка и сохранение на сервере
            try
            {
                using var img = await file.From64bitToImageAsync();
                using var resizeImg = img.Resize(_imageSettings.MaxWidth, _imageSettings.MaxHeight);
                resizeImg.Save(path);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            return new StatusCodeResult(StatusCodes.Status200OK);
        }



        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;
            return GetPerson(namePerson);
        }

        private Person? GetPerson(string nick)
        {
            var person = _dbContext.People
                 .Include(i => i.Friends)
                     .ThenInclude(f => f.Friend)
                 .Include(i => i.Subscribers)
                 .FirstOrDefault(i => i.Nick == nick);
            return person;
        }

        private readonly string _avatarPath;
        private readonly string _defaultAvatarName;

        private readonly UserDbContext _dbContext;
        private readonly AuthorizationSettings settings;
        private readonly ImageSettings _imageSettings;
    }
    public record class PersonData(string Nick,  string Name, string Family, string LastName, string Phone, string AvatarPath);
    public record class ChangePasswordData(string OldPassword, string Pass);

    public record class PathSetting(string Avatar, string DefaultAvatarName);
    public record class ImageSettings(int MaxHeight, int MaxWidth);
}
