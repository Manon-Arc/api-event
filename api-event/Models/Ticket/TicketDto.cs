using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class TicketDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    //[JsonIgnore]
    public string? Id { get; set; }

    public string? officeId { get; set; }

    public string userId { get; set; } = null!;

    public string eventId { get; set; } = null!;

    public string? expireDate { get; set; }
}