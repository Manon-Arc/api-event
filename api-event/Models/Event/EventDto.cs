using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class EventDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string name { get; set; } = null!;

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset date { get; set; }
}