using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models.Ticket;

public class TicketDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    //[JsonIgnore]
    public string? Id { get; set; }

    public string? OfficeId { get; set; }

    public string UserId { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string? ExpireDate { get; set; }
}