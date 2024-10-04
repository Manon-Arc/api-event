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

    public async Task UpdateAsync(string id, UserModel updatedBook) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
}