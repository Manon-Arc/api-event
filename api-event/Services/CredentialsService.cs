using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.SecurityToken.Model;
using Microsoft.IdentityModel.Tokens;

namespace api_event.Services
{
    public class CredentialsService
    {
        private readonly IMongoCollection<CredentialsModel> _credentialsCollection;
        private readonly UsersService _usersService;
        private readonly JwtSettings _jwtSettings;

        public CredentialsService(IOptions<EventprojDBSettings> dbSettings, UsersService usersService, IOptions<JwtSettings> jwtSettings)
        {
            var mongoClient = new MongoClient(
                dbSettings.Value.ConnectionString);
            
            var mongoDatabase = mongoClient.GetDatabase(
                dbSettings.Value.DatabaseName);
            
            _credentialsCollection = mongoDatabase.GetCollection<CredentialsModel>(dbSettings.Value.CredentialsCollectionName);
            _usersService = usersService;
            _jwtSettings = jwtSettings.Value;
        }

        // Enregistrement d'un utilisateur avec ses credentials
        public async Task RegisterAsync(CredentialsModel _credentials)
        {

            var user = new UserModel
            {
                Mail = _credentials.Mail
            };

            // Création de l'utilisateur
            await _usersService.CreateAsync(user);

            // Hash du mot de passe et création des credentials
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(_credentials.Password);
            var credentials = new CredentialsModel { Mail = user.Mail, Password = hashedPassword, UserId = user.Id! };
            await _credentialsCollection.InsertOneAsync(credentials);
        }

        // Login avec mail et mot de passe
        public async Task<string?> LoginAsync(CredentialsModel credentials)
        {
            var storedCredential = await _credentialsCollection.Find(c => c.Mail == credentials.Mail).FirstOrDefaultAsync();
            if (storedCredential == null || !BCrypt.Net.BCrypt.Verify(credentials.Password, storedCredential.Password))
            {
                return null; // Credentials invalides
            }

            var user = await _usersService.GetAsync(storedCredential.UserId);
            return GenerateJwtToken(user!);
        }

        // Génération du token JWT
        private string GenerateJwtToken(UserModel userModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userModel.Id.ToString()),
                    new Claim(ClaimTypes.Email, userModel.Mail)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
