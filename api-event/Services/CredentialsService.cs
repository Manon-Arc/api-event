using System.IdentityModel.Tokens.Jwt;
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
        private readonly IMongoCollection<CredentialsModel> _credentialsCollection;
        private readonly UsersService _usersService;
        private readonly JwtSettings _jwtSettings;
        
        private IConfiguration _config;
        
        public CredentialsService(IOptions<EventprojDBSettings> dbSettings, UsersService usersService, IOptions<JwtSettings> jwtSettings, IConfiguration config)
        {
            var mongoClient = new MongoClient(
                dbSettings.Value.ConnectionString);
            
            var mongoDatabase = mongoClient.GetDatabase(
                dbSettings.Value.DatabaseName);
            
            _credentialsCollection = mongoDatabase.GetCollection<CredentialsModel>(dbSettings.Value.CredentialsCollectionName);
            _usersService = usersService;
            _jwtSettings = jwtSettings.Value;
            _config = config;
        }
        

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
        if (storedCredential == null || !BCrypt.Net.BCrypt.Verify(credentials.password, storedCredential.password)) return null; // Credentials invalides
    
        return storedCredential.UserId;
        
    }
}
