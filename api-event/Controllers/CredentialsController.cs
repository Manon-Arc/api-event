using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using api_event.Models.Credentials;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api_event.Controllers;

[ApiController]
[Route("[controller]")]
public class CredentialController(CredentialsService credentialService) : ControllerBase
{
    /// <summary>
    ///     Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="credential">The user credentials for registration.</param>
    /// <remarks>
    ///     This endpoint registers a new user account. You must provide valid credentials to successfully register.
    ///     Sample request:
    ///     POST /Credential/register
    ///     {
    ///     "email": "test@test.com",
    ///     "password": "password123"
    ///     }
    /// </remarks>
    /// <returns>A success message upon successful registration.</returns>
    /// <response code="200">If the user was successfully registered.</response>
    /// <response code="400">If the provided data is invalid.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] CredentialsIdlessDto credential)
    {
        await credentialService.RegisterAsync(credential);
        return Ok("Utilisateur enregistré avec succès");
    }

    /// <summary>
    ///     Logs in a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="credentials">The login credentials.</param>
    /// <remarks>
    ///     This endpoint validates the provided credentials and returns a JWT token if they are correct.
    ///     Sample request:
    ///     POST /Credential/login
    ///     {
    ///     "email": "test@test.com",
    ///     "password": "password123"
    ///     }
    ///     Use the token for authorization in subsequent requests.
    /// </remarks>
    /// <returns>A JWT token if login is successful.</returns>
    /// <response code="200">Returns a JWT token upon successful authentication.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsIdlessDto credentials)
    {
        var user = await credentialService.LoginAsync(credentials);
        if (user == null) return Unauthorized("Identifiants invalides");


        var publicKey = credentialService.Config["Jwt:PrivateKey"];
        var rsa = new RSACryptoServiceProvider(4096);
        rsa.ImportFromPem(publicKey?.ToCharArray());
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user)
                // Add more claims as needed
            }),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256),
            Issuer = credentialService.Config["Jwt:Issuer"], // Add this line
            Audience = credentialService.Config["Jwt:Audience"]
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(tokenHandler.WriteToken(token));
    }
}