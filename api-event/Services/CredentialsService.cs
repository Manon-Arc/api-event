using api_event.Models;
using api_event.Models.Credentials;
using api_event.Models.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class CredentialsService
{
    private readonly IMongoCollection<CredentialsDto> _credentialsCollection;
    private readonly UsersService _usersService;
    public readonly IConfiguration Config;
    public readonly JwtSettings JwtSettings;

    public CredentialsService(IOptions<DbSettings> dbSettings, UsersService usersService,
        IOptions<JwtSettings> jwtSettings, IConfiguration config)
    {
        var mongoClient = new MongoClient(
            dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _credentialsCollection =
            mongoDatabase.GetCollection<CredentialsDto>(dbSettings.Value.CredentialsCollectionName);
        _usersService = usersService;
        JwtSettings = jwtSettings.Value;
        Config = config;
    }

    // Enregistrement d'un utilisateur avec ses credentials
    public async Task RegisterAsync(CredentialsIdlessDto credentials)
    {
        var user = new UserDto
        {
            Mail = credentials.Mail
        };

        // Création de l'utilisateur
        await _usersService.CreateAsync(user);

        // Hash du mot de passe et création des credentials
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
        var newCredentials = new CredentialsDto { Mail = user.Mail, Password = hashedPassword, UserId = user.Id! };
        await _credentialsCollection.InsertOneAsync(newCredentials);
    }

    // Login avec mail et mot de passe
    public async Task<string?> LoginAsync(CredentialsIdlessDto credentials)
    {
        var storedCredential = await _credentialsCollection.Find(c => c.Mail == credentials.Mail).FirstOrDefaultAsync();
        if (storedCredential == null || !BCrypt.Net.BCrypt.Verify(credentials.Password, storedCredential.Password))
            return null; // Credentials invalides

        return storedCredential.UserId;
    }
}