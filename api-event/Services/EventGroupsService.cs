using api_event.Models.EventGroup;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class EventGroupsService
{
    private readonly IMongoCollection<EventGroupsDto> _eventGroupsCollection;

    public EventGroupsService(IOptions<DbSettings> eventprojDbSettings)
    {
        var mongoClient = new MongoClient(eventprojDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(eventprojDbSettings.Value.DatabaseName);

        _eventGroupsCollection =
            mongoDatabase.GetCollection<EventGroupsDto>(eventprojDbSettings.Value.EventGroupsCollectionName);
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
            Name = eventIdlessGroups.Name
        };
        await _eventGroupsCollection.ReplaceOneAsync(e => e.Id == id, eventGroups);
    }

    public async Task RemoveAsync(string id)
    {
        await _eventGroupsCollection.DeleteOneAsync(e => e.Id == id);
    }
}