using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class UserDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string firstName { get; set; } = null!;

    public string lastName { get; set; } = null!;

    public string mail { get; set; } = null!;
}