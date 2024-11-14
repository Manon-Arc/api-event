using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api_event.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace api_event.Services;

public class CredentialsService
{
    private readonly IMongoCollection<CredentialsDto> _credentialsCollection;
    private readonly JwtSettings _jwtSettings;
    private readonly UsersService _usersService;

    public CredentialsService(IOptions<DbSettings> dbSettings, UsersService usersService,
        IOptions<JwtSettings> jwtSettings)
    {
        var mongoClient = new MongoClient(
            dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _credentialsCollection =
            mongoDatabase.GetCollection<CredentialsDto>(dbSettings.Value.CredentialsCollectionName);
        _usersService = usersService;
        _jwtSettings = jwtSettings.Value;
    }

    // Enregistrement d'un utilisateur avec ses credentials
    public async Task RegisterAsync(CredentialsIdlessDto credentials)
    {
        var user = new UserDto
        {
            mail = credentials.mail
        };

        // Création de l'utilisateur
        await _usersService.CreateAsync(user);

        // Hash du mot de passe et création des credentials
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(credentials.password);
        var newCredentials = new CredentialsDto() { mail = user.mail, password = hashedPassword, userId = user.Id! };
        await _credentialsCollection.InsertOneAsync(newCredentials);
    }

    // Login avec mail et mot de passe
    public async Task<string?> LoginAsync(CredentialsIdlessDto credentials)
    {
        var storedCredential = await _credentialsCollection.Find(c => c.mail == credentials.mail).FirstOrDefaultAsync();
        if (storedCredential == null ||
            !BCrypt.Net.BCrypt.Verify(credentials.password,
                storedCredential.password)) return null; // Credentials invalides

        var user = await _usersService.GetAsync(storedCredential.userId);
        return GenerateJwtToken(user!);
    }

    // Génération du token JWT
    private string GenerateJwtToken(UserDto userDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userDto.Id),
                new Claim(ClaimTypes.Email, userDto.mail)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}