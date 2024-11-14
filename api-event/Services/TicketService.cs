using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class TicketsService
{
    private readonly IMongoCollection<TicketDto> _ticketsCollection;

    public TicketsService(
        IOptions<EventprojDBSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _ticketsCollection = mongoDatabase.GetCollection<TicketDto>(
            eventprojDatabaseSettings.Value.TicketsCollectionName);
    }

    public async Task<List<TicketDto>> GetAsync()
    {
        return await _ticketsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<TicketDto?> GetAsync(string id)
    {
        return await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TicketDto newTicketDto)
    {
        await _ticketsCollection.InsertOneAsync(newTicketDto);
    }

    public async Task UpdateAsync(string id, TicketIdlessDto ticketIdlessDto)
    {
        var ticketDto = new TicketDto
        {
            Id = id,
            expireDate = ticketIdlessDto.expireDate,
            eventId = ticketIdlessDto.eventId,
            userId = ticketIdlessDto.userId,
            officeId = ticketIdlessDto.officeId
        };

        await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, ticketDto);
    }

    public async Task RemoveAsync(string id)
    {
        await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
    }
}