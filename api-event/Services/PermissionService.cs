using api_event;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class PermissionService
{
    private readonly IMongoCollection<PermissionModel> _permissionCollection;

    public PermissionService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _permissionCollection = mongoDatabase.GetCollection<PermissionModel>(
            eventprojDatabaseSettings.Value.PermissionCollectionName);
    }

    public async Task CreateAsync(string? newUserId)
    {
        var newPermission = new PermissionModel()
        {
            PermissionId = new Guid(), 
            UserId = newUserId,
        };

        await _permissionCollection.InsertOneAsync(newPermission);
    }
}