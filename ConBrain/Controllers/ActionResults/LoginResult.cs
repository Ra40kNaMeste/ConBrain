using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConBrain.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace ConBrain.Controllers.ActionResults
{
    public class LoginResult : IActionResult
    {
        public LoginResult(Person person, AuthorizationSettings settings) 
        {
            _person = person;
            _settings = settings;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, _person.Nick) };
            var jwt = new JwtSecurityToken(
                issuer: _settings.Issures, 
                audience: _settings.Audience, 
                claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(_settings.ExpiresHours)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)), SecurityAlgorithms.HmacSha256));
            await context.HttpContext.Response.WriteAsJsonAsync(new { nick = _person.Nick, token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }
        private readonly Person _person;
        private readonly AuthorizationSettings _settings;
    }
}
