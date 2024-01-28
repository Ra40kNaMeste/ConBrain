using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    public class AuthorizationController : Controller
    {
        public AuthorizationController(IConfiguration configuration, UserDbContext context) 
        {
            settings = configuration.GetSection("Authorization").Get<AuthorizationSettings>() ?? throw new FormatException();
            this.context = context;
        }
        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserLoginData data)
        {
            return new LoginResult(data, settings, context);
        }
        [HttpPost]
        [Route("Register")]
        public IActionResult Register(UserRegisterData data)
        {
            return new RegisterResult(data, settings, context);
        }
        private UserDbContext context;
        private AuthorizationSettings settings { get; init; }
    }
    public record class AuthorizationSettings(string Issures, string Audience, string Key, int ExpiresHours);
}
