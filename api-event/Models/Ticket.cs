using Microsoft.VisualBasic.CompilerServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models;

public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? OfficeID { get; set; }

    public string UserID { get; set; } = null!;

    public string EventID { get; set; } = null!;

    public string ExpireDate { get; set; } = null!;

}