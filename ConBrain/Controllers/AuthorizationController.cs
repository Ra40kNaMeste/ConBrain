using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login() 
        {
            return View();
        }
    }
}
