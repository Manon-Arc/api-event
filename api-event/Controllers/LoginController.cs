using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using api_event.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace api_event.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Normally, you'd verify the user from a database
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var token = GenerateJwtToken(user);
                return Ok(new TokenModel
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1) // Token valid for 1 hour
                });
            }
            return Unauthorized();
        }

        // Dummy method to authenticate user - replace this with your actual user validation logic
        private User? AuthenticateUser(LoginModel login)
        {
            // Example hard-coded user for demo purposes, replace with database lookup
            if (login.Mail == "user@example.com" && login.Password == "password")
            {
                return new User
                {
                    Id = "12345",
                    FirstName = "John",
                    LastName = "Doe",
                    Mail = login.Mail,
                    Permission = 1
                };
            }
            return null;
        }

        // Method to generate the JWT token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Mail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Permission", user.Permission.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
