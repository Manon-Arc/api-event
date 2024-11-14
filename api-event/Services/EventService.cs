using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class EventsService
{
    private readonly IMongoCollection<EventDto> _eventsCollection;

    public EventsService(
        IOptions<DbSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _eventsCollection = mongoDatabase.GetCollection<EventDto>(
            eventprojDatabaseSettings.Value.EventsCollectionName);
    }

    public async Task<List<EventDto>> GetAsync()
    {
        return await _eventsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<EventDto?> GetAsync(string id)
    {
        return await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(EventDto newEventDto)
    {
        await _eventsCollection.InsertOneAsync(newEventDto);
    }

    public async Task UpdateAsync(string id, EventIdlessDto eventIdlessDto)
    {
        var eventDto = await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        eventDto.name = eventIdlessDto.name;

        await _eventsCollection.ReplaceOneAsync(x => x.Id == id, eventDto);
    }

    public async Task RemoveAsync(string id)
    {
        await _eventsCollection.DeleteOneAsync(x => x.Id == id);
    }
}