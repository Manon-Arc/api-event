using api_event.Models.TicketOffice;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class TicketOfficeService
{
    private readonly IMongoCollection<TicketOfficeDto> _ticketOfficesCollection;

    public TicketOfficeService(
        IOptions<DbSettings> eventprojDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eventprojDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eventprojDatabaseSettings.Value.DatabaseName);

        _ticketOfficesCollection =
            mongoDatabase.GetCollection<TicketOfficeDto>(eventprojDatabaseSettings.Value.TicketOfficeCollectionName);
    }

    public async Task<List<TicketOfficeDto>> GetAsync()
    {
        return await _ticketOfficesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<TicketOfficeDto?> GetAsync(string id)
    {
        return await _ticketOfficesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TicketOfficeDto ticketOfficeDto)
    {
        await _ticketOfficesCollection.InsertOneAsync(ticketOfficeDto);
    }

    public async Task UpdateAsync(string id, TicketOfficeIdlessDto updatedTicketOffice)
    {
        var ticketOffice = new TicketOfficeDto
        {
            Id = id,
            Name = updatedTicketOffice.Name,
            EventId = updatedTicketOffice.EventId,
            Price = updatedTicketOffice.Price,
            CloseDate = updatedTicketOffice.CloseDate,
            EventDate = updatedTicketOffice.EventDate,
            OpenDate = updatedTicketOffice.OpenDate,
            TicketCount = updatedTicketOffice.TicketCount
        };
        await _ticketOfficesCollection.ReplaceOneAsync(x => x.Id == id, ticketOffice);
    }

    public async Task RemoveAsync(string id)
    {
        await _ticketOfficesCollection.DeleteOneAsync(x => x.Id == id);
    }
}