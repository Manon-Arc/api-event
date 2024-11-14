namespace api_event.Models;

public class TicketIdlessDto
{
    public string userId { get; set; } = null!;
    public string eventId { get; set; } = null!;

    public string? officeId { get; set; }
    public string expireDate { get; set; } = null!;
}