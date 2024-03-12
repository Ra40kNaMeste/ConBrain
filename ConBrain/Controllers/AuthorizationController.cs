using ConBrain.Controllers.ActionResults;
using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Route("login")]
        public IActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserLoginData data)
        {
            var person = context.People.Include(i => i.Data).Where(i=>i.Data.Nick == data.Login).FirstOrDefault();
            List<ValidationResult> results = new();
            if (person == null)
                results.Add(new("User does not exist", new string[] { nameof(data.Login).ToLower() }));
            else if (person.Password != data.Password)
                results.Add(new("Uncorrect password", new string[] { nameof(data.Password).ToLower() }));
            else
                return new LoginResult(person, settings);
            return new ErrorValidationResult(results);
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserRegisterData data)
        {
            var person = new Person()
            {
                Data = new()
                {
                    Name = data.Name,
                    Family = data.Family,
                    SecondName = data.SecondName,
                    Nick = data.Nick,
                    Phone = data.Phone
                },
                Password = data.Password
            };

            List<ValidationResult> results = new();
            if(!Validator.TryValidateObject(person, new(person), results, true))
                return new ErrorValidationResult(results);

            try
            {
                context.PersonData.Add(person.Data);
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


    public class UserRegisterData : PersonData
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [TagHelpers.Display("Password", Type="password", Classes = new string[] { "sendInput" })]
        public string Password { get; set; }

    }
    public record class UserLoginData(string Login, string Password);
}
