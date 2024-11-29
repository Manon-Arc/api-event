using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models.EventGroup;

public class EventGroupsDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;
}