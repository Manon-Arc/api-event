using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models.TicketOffice;

public class TicketOfficeDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; }
    public string EventId { get; set; }
    public int TicketCount { get; set; }
    public float Price { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset EventDate { get; set; }

    public DateTimeOffset OpenDate { get; set; }
    public DateTimeOffset CloseDate { get; set; }
}