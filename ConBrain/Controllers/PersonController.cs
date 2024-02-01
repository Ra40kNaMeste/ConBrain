using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    [Authorize]
    public class PersonController:Controller
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
            var person = _dbContext.People.Where(i => i.Nick == id).FirstOrDefault();
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
        [Route("friends")]
        public IActionResult Friends()
        {
            var person = GetPersonByAuth();
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
            return new FriendsActionResult(person.Friends.Select(i=>i.Nick));
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
            if(!person.Friends.Contains(friend))
                person.Friends.Add(friend);
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
            person.Friends.Remove(friend);
            _dbContext.Update(person);
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #endregion //person/friends
        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;
            var prope = _dbContext.People;
            var c = prope.Count();
            var person = _dbContext.People.FirstOrDefault(i => i.Nick == namePerson);
            return person;
        }
        private UserDbContext _dbContext;
    }
}
