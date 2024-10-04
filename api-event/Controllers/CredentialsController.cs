using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CredentialController : ControllerBase
    {
        private readonly CredentialsService _credentialService;

        public CredentialController(CredentialsService credentialService)
        {
            _credentialService = credentialService;
        }

        // Endpoint pour enregistrer un nouvel utilisateur
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] CredentialsModel _credential)
        {
            await _credentialService.RegisterAsync(_credential);
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
