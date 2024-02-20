using Microsoft.AspNetCore.Mvc;

namespace ConBrain.Controllers
{
    public class ErrorController:Controller
    {
        [Route("/error")]
        public IActionResult MessageError(string message)
        {
            return View("MessageError", message);
        }
    }
}
