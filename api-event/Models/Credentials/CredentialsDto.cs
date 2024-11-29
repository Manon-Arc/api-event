using MongoDB.Bson;

namespace api_event.Models.Credentials;

public class CredentialsDto
{
    public BsonObjectId? Id { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}