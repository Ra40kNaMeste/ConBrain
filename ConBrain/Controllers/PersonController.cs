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
            person.Nick = data.Nick;
            person.Name = data.Name;
            person.Family = data.Family;
            person.SecondName = data.LastName;
            person.Phone = data.Phone;

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
            var person = _dbContext.People
                .Include(i => i.Subscribers)
                .Include(i => i.Friends)
                    .ThenInclude(f => f.Friend)
                .Where(i => i.Nick == id)
                .FirstOrDefault();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View("Person", person);
        }

        [HttpGet]
        [Route("person")]
        public IActionResult Person(string nick)
        {
            var person = _dbContext.People
                .Include(i => i.Subscribers)
                .Include(i => i.Friends)
                    .ThenInclude(f => f.Friend)
                .Where(i => i.Nick == nick)
                .FirstOrDefault();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new PersonActionResult(new(person));
        }

        [HttpGet]
        [Route("{id}/friends")]
        public IActionResult Friends(string id)
        {
            var person = _dbContext.People
                .Include(i => i.Subscribers)
                .Include(i => i.Friends)
                    .ThenInclude(f => f.Friend)
                .Where(i => i.Nick == id)
                .FirstOrDefault();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(person);
        }

        [HttpGet]
        public IActionResult AuthPersonData()
        {
            var person = GetPersonByAuth();
            if (person == null)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            return new PersonActionResult(new PersonSavedMementor(person));
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

        [HttpGet]
        [Route("avatar")]
        public IActionResult Avatar(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            var path = Path.Combine(Directory.GetCurrentDirectory(), _avatarPath);
            FileInfo avatar;
            if (person.AvatarPath != null)
                avatar = new(Path.Combine(path, person.Nick, person.AvatarPath));
            else
                avatar = new(Path.Combine(path, _defaultAvatarName));

            if (avatar.Exists)
                return File(avatar.FullName, "image/jpeg");
            throw new FileNotFoundException(avatar.FullName);
        }

        [HttpPost]
        [Route("avatar")]
        public IActionResult PostAvatar(string imageKey)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            var path = Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, person.Nick, imageKey);
            if(!System.IO.File.Exists(path))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            person.AvatarPath = path;
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpGet]
        [Route("{nick}/image")]
        public IActionResult Image(string nick)
        {
            var person = GetPerson(nick);
            return null;
        }

        //Метод для добавления изображения авторизированного пользователя
        [HttpPost]
        [Route("image")]
        public async Task<IActionResult> Image(IFormFile file, string key)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            if(file == null || !file.CanImage())
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            var path = Path.Combine(Directory.GetCurrentDirectory(), _avatarPath, person.Nick);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, key);

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
    public record class PersonData(string Nick,  string Name, string Family, string LastName, string Phone);
    public record class ChangePasswordData(string OldPassword, string Pass);

    public record class PathSetting(string Avatar, string DefaultAvatarName);
    public record class ImageSettings(int MaxHeight, int MaxWidth);
}
