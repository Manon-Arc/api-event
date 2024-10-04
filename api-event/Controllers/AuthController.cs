using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;

namespace api_event.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly TicketsService _ticketsService;

        public AuthController(AuthService authService, TicketsService ticketsService)
        {
            _authService = authService;
            _ticketsService = ticketsService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _ticketsService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || !_authService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Hash password before storing it
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _ticketsService.CreateUserAsync(user);
            return Ok();
        }
    }
}
