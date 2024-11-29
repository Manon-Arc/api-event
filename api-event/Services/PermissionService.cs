using api_event.Models.Permission;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class PermissionService
{
    private readonly IMongoCollection<PermissionDto> _permissionCollection;

    public PermissionService(
        IOptions<DbSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _permissionCollection = mongoDatabase.GetCollection<PermissionDto>(
            eventprojDatabaseSettings.Value.PermissionCollectionName);
    }

    public async Task CreateAsync(string newUserId)
    {
        var newPermission = new PermissionDto
        {
            PermissionId = new Guid(),
            UserId = newUserId
        };

        await _permissionCollection.InsertOneAsync(newPermission);
    }
}