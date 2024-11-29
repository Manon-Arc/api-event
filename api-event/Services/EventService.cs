using api_event.Models.Event;
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

    public async Task<List<EventDto>?> GetAsync()
    {
        var result = await _eventsCollection.Find(_ => true).ToListAsync();
        return result;
    }

    public async Task<EventDto?> GetAsync(string id)
    {
        var result = await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<EventDto>?> GetByGroupIdAsync(string id)
    {
        var result = await _eventsCollection.Find(x => x.GroupId == id).ToListAsync();
        return result;
    }

    public async Task<EventDto?> CreateAsync(EventDto newEventDto)
    {
        try
        {
            await _eventsCollection.InsertOneAsync(newEventDto);
            var insertedEvent = await _eventsCollection.Find(x => x.Id == newEventDto.Id).FirstOrDefaultAsync();
            return insertedEvent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while inserting: {ex.Message}");
            return null;
        }
    }

    public async Task<EventDto?> UpdateAsync(string id, EventIdlessDto eventIdlessDto)
    {
        var eventDto = new EventDto
        {
            Id = id,
            Name = eventIdlessDto.Name,
            Date = eventIdlessDto.Date,
            GroupId = eventIdlessDto.GroupId
        };
        var result = await _eventsCollection.ReplaceOneAsync(x => x.Id == id, eventDto);

        if (result.ModifiedCount > 1) return await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        return null;
    }

    public async Task<bool> RemoveAsync(string id)
    {
        var result = await _eventsCollection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}