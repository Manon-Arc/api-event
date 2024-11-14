using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[ApiController]
[Route("[controller]")]
public class CredentialController : ControllerBase
{
    private readonly CredentialsService _credentialService;

    public CredentialController(CredentialsService credentialService)
    {
        _credentialService = credentialService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] CredentialsModel credential)
    {
        await _credentialService.RegisterAsync(credential);
        return Ok("Utilisateur enregistré avec succès");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] CredentialsModel credentials)
    {
        var token = await _credentialService.LoginAsync(credentials);
        if (token == null) return Unauthorized("Identifiants invalides");
        return Ok(new { Token = token });
    }
}