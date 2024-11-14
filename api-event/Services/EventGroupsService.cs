using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class EventGroupsService
{
    private readonly IMongoCollection<EventGroupsDto> _eventGroupsCollection;

    public EventGroupsService(IOptions<DbSettings> eventprojDBSettings)
    {
        var mongoClient = new MongoClient(eventprojDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(eventprojDBSettings.Value.DatabaseName);

        _eventGroupsCollection =
            mongoDatabase.GetCollection<EventGroupsDto>(eventprojDBSettings.Value.EventGroupsCollectionName);
    }

    public async Task<List<EventGroupsDto>> GetAsync()
    {
        return await _eventGroupsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<EventGroupsDto?> GetAsync(string id)
    {
        return await _eventGroupsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(EventGroupsDto eventGroups)
    {
        await _eventGroupsCollection.InsertOneAsync(eventGroups);
    }

    public async Task UpdateAsync(string id, EventGroupsIdlessDto eventIdlessGroups)
    {
        var eventGroups = new EventGroupsDto
        {
            Id = id,
            name = eventIdlessGroups.name
        };
        await _eventGroupsCollection.ReplaceOneAsync(e => e.Id == id, eventGroups);
    }

    public async Task RemoveAsync(string id)
    {
        await _eventGroupsCollection.DeleteOneAsync(e => e.Id == id);
    }
}