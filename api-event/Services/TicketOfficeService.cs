using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class TicketOfficeService
{
    private readonly IMongoCollection<TicketOfficeModel> _ticketOfficesCollection;

    public TicketOfficeService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _ticketOfficesCollection = mongoDatabase.GetCollection<TicketOfficeModel>(eventprojDatabaseSettings.Value.TicketOfficeCollectionName);
    }

    public async Task<List<TicketOfficeModel>> GetAsync() =>
        await _ticketOfficesCollection.Find(_ => true).ToListAsync();

    public async Task<TicketOfficeModel?> GetAsync(string id) =>
    await _ticketOfficesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(TicketOfficeModel newTicketOffice) =>
        await _ticketOfficesCollection.InsertOneAsync(newTicketOffice);

    public async Task UpdateAsync(string id, TicketOfficeModel updatedTicketOffice) =>
        await _ticketOfficesCollection.ReplaceOneAsync(x => x.Id == id, updatedTicketOffice);

    public async Task RemoveAsync(string id) =>
        await _ticketOfficesCollection.DeleteOneAsync(x => x.Id == id);
}