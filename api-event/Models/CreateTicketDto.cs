namespace api_event.Models
{
    public class CreateTicketDto
    {
        public string UserID { get; set; } = null!;
        public string EventID { get; set; } = null!;
        public string ExpireDate { get; set; } = null!;
    }
}
