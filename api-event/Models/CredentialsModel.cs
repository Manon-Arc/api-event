using MongoDB.Bson;

namespace api_event.Models
{
    public class CredentialsModel
    {
        public BsonObjectId? _id { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 
        public string UserId { get; set; } = string.Empty;

    }
}
