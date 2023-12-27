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
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login() 
        {
            return View();
        }

        public IActionResult LogOut(UserLoginData data)
        {
            return new LoginResult(data, settings, context);
        }

        public IActionResult RegisterOut(UserRegisterData data)
        {
            return new RegisterResult(data, settings, context);
        }
        private UserDbContext context;
        private AuthorizationSettings settings { get; init; }
    }
    public record class AuthorizationSettings(string Issures, string Audience, string Key, int ExpiresHours);
}
