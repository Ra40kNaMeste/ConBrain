using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
