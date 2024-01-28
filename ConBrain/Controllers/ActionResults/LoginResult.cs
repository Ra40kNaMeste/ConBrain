using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConBrain.Model;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConBrain.Controllers.ActionResults
{
    public class LoginResult : IActionResult
    {
        public LoginResult(UserLoginData data, AuthorizationSettings settings, UserDbContext context) 
        {
            _data = data;
            _settings = settings;
            _context = context;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var user = _context.People.Where(i => i.Nick == _data.LoginOrNick || i.Phone == _data.LoginOrNick).FirstOrDefault();
            if (user == null || user.Password != _data.Pass)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.HttpContext.Response.StartAsync();
                return;
            }
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, user.Nick), new Claim(ClaimTypes.MobilePhone, user.Phone) };
            var jwt = new JwtSecurityToken(
                issuer: _settings.Issures, 
                audience: _settings.Audience, 
                claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(_settings.ExpiresHours)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)), SecurityAlgorithms.HmacSha256));
            await context.HttpContext.Response.WriteAsJsonAsync(new { nick = user.Nick, token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }
        private readonly UserLoginData _data;
        private readonly AuthorizationSettings _settings;
        private readonly UserDbContext _context;
    }

    public record class UserLoginData(string LoginOrNick, string Pass);
}
