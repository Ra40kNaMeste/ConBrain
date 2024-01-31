using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                Results.BadRequest();
            return View(person);
        }
        //[Route("messages")]
        //public IActionResult Messages()
        //{ 
        //    var person = GetPersonByAuth();
        //    var messages = person.Messages.Select(i => i.Sender == person ? i.Target : i.Sender).Distinct();
        //    return View();
        //}
        [Route("friends")]
        public IActionResult Friends()
        {
            var person = GetPersonByAuth();
            if (person == null)
                Results.BadRequest();
            return View(person);
        }

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
