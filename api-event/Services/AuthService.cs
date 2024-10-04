using api_event.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_event.Services
{
    public class Auth
    {
        private readonly string _secretKey;

        public Auth(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id!),
                    new Claim(ClaimTypes.Role, user.Permission.ToString())  // Store permission as role
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Implement password hashing check (e.g., using BCrypt)
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }
    }
}
