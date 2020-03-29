using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using CustomerApi.Models;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CustomerDatabaseContext _customerContext;

        public LoginController(IConfiguration config, CustomerDatabaseContext customerContext) 
        {
            this._config = config;
            this._customerContext = customerContext;
        }

        [HttpGet]
        public IActionResult Login(string username, string password) 
        {
            User user = new User() { Username = username, Password = password };
            IActionResult response = Unauthorized();

            if (AuthenticateUser(user)) 
            {
                var token = GenerateWebToken(user);
                response = Ok(new { token = token });
            }

            return response;
        }

        private bool AuthenticateUser(User user) 
        {
            return _customerContext.User.Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault() != null;
        }


        private string GenerateWebToken(User user) 
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["WebKeyIssuer:Key"]));
            var crendetials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["WebKeyIssuer:Issuer"],
                audience: _config["WebKeyIssuer:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: crendetials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}