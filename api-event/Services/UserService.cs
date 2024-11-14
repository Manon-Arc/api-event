using System.Text.RegularExpressions;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class UsersService
{
    private readonly IMongoCollection<UserDto> _usersCollection;

    public UsersService(
        IOptions<DbSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<UserDto>(
            eventprojDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<UserDto>> GetAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<UserDto?> GetAsync(string id)
    {
        return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(UserDto newUserDto)
    {
        await _usersCollection.InsertOneAsync(newUserDto);
    }


    public async Task<bool> UpdateAsync(string id, UserIdlessDto userIdlessDto)
    {
        var userDto = new UserDto
        {
            Id = id,
            mail = userIdlessDto.mail,
            lastName = userIdlessDto.lastName,
            firstName = userIdlessDto.firstName
        };

        var result = await _usersCollection.ReplaceOneAsync(x => x.Id == id, userDto);
        return result.MatchedCount > 0; // Returns true if a document was matched (and thus replaced)
    }


    public async Task RemoveAsync(string id)
    {
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        return await _usersCollection.Find(u => u.mail == email).FirstOrDefaultAsync();
    }

    public bool IsEmailValid(string email)
    {
        var EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }
}