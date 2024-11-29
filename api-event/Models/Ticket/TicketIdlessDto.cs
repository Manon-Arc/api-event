namespace api_event.Models.Ticket;

public class TicketIdlessDto
{
    public string UserId { get; set; } = null!;
    public string EventId { get; set; } = null!;

    public string? OfficeId { get; set; }
    public string? ExpireDate { get; set; }
}