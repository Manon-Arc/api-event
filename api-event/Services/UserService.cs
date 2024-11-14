using System.Text.RegularExpressions;
using api_event;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class UsersService
{
    private readonly IMongoCollection<UserModel> _usersCollection;

    public UsersService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<UserModel>(
            eventprojDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<UserModel>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<UserModel?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(UserModel newUserModel) =>
        await _usersCollection.InsertOneAsync(newUserModel);


    public async Task<bool> UpdateAsync(string id, UserModel updatedUser)
    {
        var result = await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);
        return result.MatchedCount > 0; // Returns true if a document was matched (and thus replaced)
    }


    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
    
    public async Task<UserModel> GetByEmailAsync(string email)
    {
        return await _usersCollection.Find(u => u.Mail == email).FirstOrDefaultAsync();
    }

    public bool IsEmailValid(string email)
    {
        Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }
}