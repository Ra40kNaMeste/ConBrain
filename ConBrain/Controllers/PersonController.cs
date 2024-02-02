using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        public PersonController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Route("home")]
        public IActionResult Dates()
        {
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View("Person", person);
        }

        [AllowAnonymous]
        [Route("id={id}")]
        public IActionResult Person(string id)
        {
            var person = _dbContext.People
                .Include(i => i.Subscribers)
                .Include(i => i.Friends)
                    .ThenInclude(f => f.Friend)
                .Where(i => i.Nick == id)
                .FirstOrDefault();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View("Person", person);
        }

        //[Route("messages")]
        //public IActionResult Messages()
        //{ 
        //    var person = GetPersonByAuth();
        //    var messages = person.Messages.Select(i => i.Sender == person ? i.Target : i.Sender).Distinct();
        //    return View();
        //}
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
        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;
            
            var person = _dbContext.People
                .Include(i => i.Friends)
                    .ThenInclude(f=>f.Friend)
                .Include(i => i.Subscribers)
                .FirstOrDefault(i => i.Nick == namePerson);
            return person;
        }
        private UserDbContext _dbContext;
    }
}
