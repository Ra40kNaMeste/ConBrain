using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
            var person = context.People.Where(i=>i.Nick == data.Nick).FirstOrDefault();
            if (person == null || person.Password != data.Password)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            return new LoginResult(person, settings);
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegisterData data)
        {
            var person = new Person()
            {
                Name = data.Name,
                Family = data.Family,
                SecondName = data.SecondName,
                Nick = data.Nick,
                Phone = data.Phone,
                Password = data.Password
            };

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                context.People.Add(person);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new DbValidationErrorResult(ex.Message);
            }

            return new LoginResult(person, settings);
        }
        private UserDbContext context;
        private readonly AuthorizationSettings settings;
    }
    public record class AuthorizationSettings(string Issures, string Audience, string Key, int ExpiresHours);
    public record class UserRegisterData(string Name, string Family, string SecondName, string Nick, string Phone, string Password);
    public record class UserLoginData(string Nick, string Password);
}
