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

    /// <summary>
    /// Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="credential">The user credentials for registration.</param>
    /// <remarks>
    /// This endpoint registers a new user account. You must provide valid credentials to successfully register.
    /// 
    /// Sample request:
    /// 
    ///     POST /Credential/register
    ///     {
    ///         "email": "test@test.com",
    ///         "password": "password123"
    ///     }
    /// </remarks>
    /// <returns>A success message upon successful registration.</returns>
    /// <response code="200">If the user was successfully registered.</response>
    /// <response code="400">If the provided data is invalid.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] CredentialsIdlessDto credential)
    {
        if (credential == null || string.IsNullOrEmpty(credential.mail) || string.IsNullOrEmpty(credential.password))
        {
            return BadRequest("Username and password are required.");
        }
        await _credentialService.RegisterAsync(credential);
        return Ok("Utilisateur enregistré avec succès");
    }

    /// <summary>
    /// Logs in a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="credentials">The login credentials.</param>
    /// <remarks>
    /// This endpoint validates the provided credentials and returns a JWT token if they are correct.
    /// 
    /// Sample request:
    /// 
    ///     POST /Credential/login
    ///     {
    ///         "email": "test@test.com",
    ///         "password": "password123"
    ///     }
    /// 
    /// Use the token for authorization in subsequent requests.
    /// </remarks>
    /// <returns>A JWT token if login is successful.</returns>
    /// <response code="200">Returns a JWT token upon successful authentication.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] CredentialsDto credentials)
    {
        if (credentials == null || string.IsNullOrEmpty(credentials.mail) || string.IsNullOrEmpty(credentials.password))
        {
            return BadRequest("Username and password are required.");
        }

        var token = await _credentialService.LoginAsync(credentials);
        if (token == null)
        {
            return Unauthorized("Identifiants invalides");
        }

        return Ok(new { Token = token });
    }
}