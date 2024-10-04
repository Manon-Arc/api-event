using api_event;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class EventsService
{
	private readonly IMongoCollection<Event> _eventsCollection;

    public EventsService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _eventsCollection = mongoDatabase.GetCollection<Event>(
            eventprojDatabaseSettings.Value.EventsCollectionName);
    }

    public async Task<List<Event>> GetAsync() =>
        await _eventsCollection.Find(_ => true).ToListAsync();

    public async Task<Event?> GetAsync(string id) =>
        await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Event newEvent) =>
        await _eventsCollection.InsertOneAsync(newEvent);

    public async Task UpdateAsync(string id, Event updatedBook) =>
        await _eventsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _eventsCollection.DeleteOneAsync(x => x.Id == id);
    
}
