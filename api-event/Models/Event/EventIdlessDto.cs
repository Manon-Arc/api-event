using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class EventIdlessDto
{
    public string name { get; set; } = null!;

    public string? groupId { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset? date { get; set; }
}