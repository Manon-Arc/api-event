using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialController : ControllerBase
    {
        private readonly CredentialsService _credentialService;

        public CredentialController(CredentialsService credentialService)
        {
            _credentialService = credentialService;
        }

        // Endpoint pour enregistrer un nouvel utilisateur
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user, [FromQuery] string password)
        {
            await _credentialService.RegisterAsync(user, password);
            return Ok("Utilisateur enregistré avec succès");
        }

        // Endpoint pour se connecter et obtenir un token JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsModel credentials)
        {
            var token = await _credentialService.LoginAsync(credentials);
            if (token == null)
            {
                return Unauthorized("Identifiants invalides");
            }
            return Ok(new { Token = token });
        }
    }
}
