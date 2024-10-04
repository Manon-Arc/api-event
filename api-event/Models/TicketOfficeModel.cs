using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class TicketOfficeModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Id_event  { get; set; }
    public int Nbr_ticket { get; set; }
    public float Price { get; set; }
    
    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset Event_date { get; set; }
    public DateTimeOffset Open_date { get; set; }
    public DateTimeOffset Close_date { get; set; }
}

