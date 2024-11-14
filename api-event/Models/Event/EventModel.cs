using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    
    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset Date { get; set; }
    
}