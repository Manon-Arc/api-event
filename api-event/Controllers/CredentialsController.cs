using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
      // Endpoint pour enregistrer un nouvel utilisateur
        [HttpPost("register")]
        
        public async Task<IActionResult> Register([FromQuery] CredentialsModel _credential)
        {
            await _credentialService.RegisterAsync(_credential);
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
        // Endpoint pour se connecter et obtenir un token JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsModel credentials)
        {
            var user = await _credentialService.LoginAsync(credentials);
            if (user == null)
            {
                return Unauthorized("Identifiants invalides");
            }
            
            
            var publicKey = _config["Jwt:PrivateKey"];
            var rsa = new RSACryptoServiceProvider(4096);
            rsa.ImportFromPem(publicKey.ToCharArray());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user),
                    // Add more claims as needed
                }),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256),
                Issuer = _config["Jwt:Issuer"], // Add this line
                Audience = _config["Jwt:Audience"] 
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
    }
}