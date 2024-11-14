using MongoDB.Bson;

namespace api_event.Models;

public class CredentialsDto
{
    public BsonObjectId? _id { get; set; }
    public string mail { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
}