using ConBrain.Model;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConBrain.Controllers.ActionResults
{
    public class RegisterResult : IActionResult
    {
        public RegisterResult(UserRegisterData data, AuthorizationSettings settings, UserDbContext context) 
        {
            _data = data;
            _settings = settings;
            _context = context;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (_data.Pass != _data.Repeatpass || _context.People.Where(i => i.Nick == _data.Nick || i.Phone == _data.Tel).Any())
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.HttpContext.Response.StartAsync();
                return;
            }
            await _context.People.AddAsync(new Person() { Family = _data.Family, LastName = _data.SecondName, Name = _data.Name, Nick = _data.Nick, Password = _data.Pass, Phone = _data.Tel });
            await _context.SaveChangesAsync();

            var claims = new List<Claim>() { new Claim(ClaimTypes.Name,_data.Nick), new Claim(ClaimTypes.MobilePhone, _data.Tel) };
            var jwt = new JwtSecurityToken(issuer: _settings.Issures, audience: _settings.Audience, claims: claims, 
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(_settings.ExpiresHours)));
            await context.HttpContext.Response.WriteAsJsonAsync(new { nick = _data.Nick, token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }

        private readonly UserRegisterData _data;
        private readonly AuthorizationSettings _settings;
        private readonly UserDbContext _context;
    }
    public record class UserRegisterData(string Name, string Family, string SecondName, string Nick, string Tel, string Pass, string Repeatpass);
}
