using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    [Authorize]
    public class HomePageController:Controller
    {
        public HomePageController(UserDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        [Route("home")]
        public IActionResult Dates()
        {
            
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if(namePerson == null)
            {
                Results.BadRequest();
            }
            var prope = _dbContext.People;
            var c = prope.Count();
            var person = _dbContext.People.FirstOrDefault(i => i.Nick == namePerson);
            return View(person);
        }
        public IActionResult Messages()
        { 
            return View();
        }
        public IActionResult Friends()
        {
            return View();
        }
        private UserDbContext _dbContext;
    }
}
