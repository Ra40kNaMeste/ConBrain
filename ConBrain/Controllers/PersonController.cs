using ConBrain.Controllers.ActionResults;
using ConBrain.Extensions;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace ConBrain.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        public PersonController(IConfiguration configuration, UserDbContext dbContext)
        {
            settings = configuration.GetSection("Authorization").Get<AuthorizationSettings>() ?? throw new FormatException();
            _dbContext = dbContext;
        }
        [Route("home")]
        public IActionResult Home()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return Redirect($"/{person.Data.Nick}");
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
            person.Password = data.Password;

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new DbValidationErrorResult(ex);
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
            return View();
        }
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonData data)
        {
            var person = GetPersonByAuth()?.Data;
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);

            data.CopyTo(person);

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new DbValidationErrorResult(ex);
            }
            return Redirect("/home");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public IActionResult Person(string id)
        {
            var person = GetPerson(id);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return View(person.Data);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("person")]
        public IActionResult PersonData(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new PersonActionResult(person.Data);
        }

        [HttpGet]
        [Route("authperson")]
        public IActionResult Person()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new PersonActionResult(person.Data);
        }

        [HttpGet]
        [Route("friends/{nick}")]
        public IActionResult Friends(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(person);
        }

        #region person/friends

        [AllowAnonymous]
        [HttpGet]
        [Route("{nick}/friends")]
        public IActionResult GetFriend(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return new PeopleActionResult(person.Friends.Select(i => i.Friend.Data));
        }

        [HttpGet]
        [Route("friends")]
        public IActionResult GetFriend(string nick, int[] ignores, int size, string? pattern)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return new PeopleActionResult(ignores, size, pattern, person.Friends.Select(i => i.Friend.Data));
        }
        [HttpPut]
        [Route("friends")]
        public async Task<IActionResult> AddFriend(string nick)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var friend = _dbContext.People.Where(i=>i.Data.Nick == nick).FirstOrDefault();
            if (friend == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if(!person.Friends.Any(i=>i.Target == friend))
                _dbContext.FreindsList.Add(new FriendPerson() { Friend = friend, Target = person });
            _dbContext.SaveChanges();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpDelete]
        [Route("friends")]
        public async Task<IActionResult> RemoveFriend(string nick)
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var friend = _dbContext.People.Where(i => i.Data.Nick == nick).FirstOrDefault();
            if (friend == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            var paar = person.Friends.RemoveAll(i=>i.Friend == friend);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #endregion //person/friends



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
                 .Include(i=>i.Data)
                 .Include(i => i.Friends)
                     .ThenInclude(f => f.Friend)
                        .ThenInclude(f=>f.Data)
                 .Include(i => i.Subscribers)
                 .FirstOrDefault(i => i.Data.Nick == nick);
            return person;
        }

        private readonly UserDbContext _dbContext;
        private readonly AuthorizationSettings settings;
    }
    public record class ChangePasswordData(string OldPassword, string Password);
    public record class ImageSettings(int MaxHeight, int MaxWidth);
}
