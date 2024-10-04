using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class EventsService
{
    private readonly IMongoCollection<EventModel> _eventsCollection;

    public EventsService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _eventsCollection = mongoDatabase.GetCollection<EventModel>(
            eventprojDatabaseSettings.Value.EventsCollectionName);
    }

    public async Task<List<EventModel>> GetAsync()
    {
        return await _eventsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<EventModel?> GetAsync(string id)
    {
        return await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(EventModel newEventModel)
    {
        await _eventsCollection.InsertOneAsync(newEventModel);
    }

    public async Task UpdateAsync(string id, EventModel updatedBook)
    {
        await _eventsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);
    }

    public async Task RemoveAsync(string id)
    {
        await _eventsCollection.DeleteOneAsync(x => x.Id == id);
    }
}