using System.Text.RegularExpressions;
using api_event.Models.User;
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

    public async Task<List<UserDto>?> GetAsync()
    {
        var result = await _usersCollection.Find(_ => true).ToListAsync();
        return result;
    }

    public async Task<UserDto?> GetAsync(string id)
    {
        var result = await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return result;
    }

    public async Task<UserDto?> CreateAsync(UserDto newUserDto)
    {
        try
        {
            await _usersCollection.InsertOneAsync(newUserDto);
            var insertedUser = await _usersCollection.Find(x => x.Id == newUserDto.Id).FirstOrDefaultAsync();
            return insertedUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while inserting: {ex.Message}");
            return null;
        }
    }

    public async Task<UserDto?> UpdateAsync(string id, UserIdlessDto userIdlessDto)
    {
        var userDto = new UserDto
        {
            Id = id,
            Mail = userIdlessDto.Mail,
            LastName = userIdlessDto.LastName,
            FirstName = userIdlessDto.FirstName
        };

        var result = await _usersCollection.ReplaceOneAsync(x => x.Id == id, userDto);

        if (result.MatchedCount > 0) // A matching document was found
            return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return null;
    }


    public async Task<bool> RemoveAsync(string id)
    {
        var result = await _usersCollection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        return await _usersCollection.Find(u => u.Mail == email).FirstOrDefaultAsync();
    }

    public bool IsEmailValid(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (string.IsNullOrWhiteSpace(email))
            return false;

        return emailRegex.IsMatch(email);
    }
}