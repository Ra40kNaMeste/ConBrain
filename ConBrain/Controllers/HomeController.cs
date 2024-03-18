using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(UserDbContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("people")]
        public IActionResult People()
        {
            return View();
        }

        [HttpGet]
        [Route("peopleList")]
        public IActionResult People(int[] ignores, int size, string? pattern)
        {
            return new PeopleActionResult(ignores, size, pattern, _context.PersonData);
        }
        private readonly UserDbContext _context;
    }
}
