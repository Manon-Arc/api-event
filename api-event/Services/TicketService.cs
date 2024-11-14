using System.Text.RegularExpressions;
using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class TicketsService
{
    private readonly IMongoCollection<TicketDto> _ticketsCollection;

    public TicketsService(
        IOptions<DbSettings> eventprojDatabaseSettings)
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
        var result = await _ticketsCollection.Find(_ => true).ToListAsync();
        return result.ToList();
    }

    public async Task<TicketDto?> GetAsync(string id)
    {
        var result = await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return result;
    }

    public async Task<TicketDto?> CreateAsync(TicketDto newTicketDto)
    {
       try
       {
            await _ticketsCollection.InsertOneAsync(newTicketDto);
            var insertedTicket = await _ticketsCollection.Find(x => x.Id == newTicketDto.Id).FirstOrDefaultAsync();
            return insertedTicket;
       }
       catch (Exception ex)
       {
            Console.WriteLine($"An error occurred while inserting: {ex.Message}");
            return null;
       } 
    }

    public async Task<TicketDto?> UpdateAsync(string id, TicketIdlessDto ticketIdlessDto)
    {
        var ticketDto = new TicketDto
        {
            Id = id,
            expireDate = ticketIdlessDto.expireDate,
            eventId = ticketIdlessDto.eventId,
            userId = ticketIdlessDto.userId,
            officeId = ticketIdlessDto.officeId
        };

        var result = await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, ticketDto);
                if (result.MatchedCount > 0) // A matching document was found
        {
            return await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        return null;
    }

    public async Task<bool> RemoveAsync(string id)
    {
        var result = await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
    
    public bool IsValid24DigitHex(string id)
    {
        var idRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (string.IsNullOrWhiteSpace(id))
            return false;

        return idRegex.IsMatch(id);
    }
}