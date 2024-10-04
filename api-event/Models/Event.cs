using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime? Date { get; set; } = DateTime.Now.ToLocalTime();
    public float TicketPrice { get; set; } = 0;



}