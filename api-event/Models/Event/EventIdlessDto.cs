using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models.Event;

public class EventIdlessDto
{
    public string Name { get; set; } = null!;

    public string? GroupId { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset? Date { get; set; }
}