using api_event;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class TicketsService
{
    private readonly IMongoCollection<Ticket> _ticketsCollection;

    public TicketsService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _ticketsCollection = mongoDatabase.GetCollection<Ticket>(
            eventprojDatabaseSettings.Value.TicketsCollectionName);
    }

    public async Task<List<Ticket>> GetAsync() =>
        await _ticketsCollection.Find(_ => true).ToListAsync();

    public async Task<Ticket?> GetAsync(string id) =>
        await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Ticket newTicket) =>
        await _ticketsCollection.InsertOneAsync(newTicket);

    public async Task UpdateAsync(string id, Ticket updatedBook) =>
        await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
}
