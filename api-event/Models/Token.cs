namespace api_event.Models
{
    public class TokenModel
    {
        public string TokenString { get; set; } = null!;

        public DateTime Expiration { get; set; }
    }
}
