using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class TicketOfficeDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string name { get; set; }
    public string eventId { get; set; }
    public int ticketCount { get; set; }
    public float price { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset eventDate { get; set; }

    public DateTimeOffset openDate { get; set; }
    public DateTimeOffset closeDate { get; set; }
}