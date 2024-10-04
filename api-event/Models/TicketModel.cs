using Microsoft.VisualBasic.CompilerServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace api_event.Models;

public class TicketModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    //[JsonIgnore]
    public string? Id { get; set; }

    public string? OfficeID { get; set; }

    public string UserID { get; set; } = null!;

    public string EventID { get; set; } = null!;

    public string ExpireDate { get; set; } = null!;

}