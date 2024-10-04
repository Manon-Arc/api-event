namespace api_event;

public class EventprojDBSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UsersCollectionName { get; set; } = null!;
<<<<<<< HEAD

    public string TicketsCollectionName { get; set; } = null!;
=======
    
    public string EventsCollectionName { get; set; } = null!;
>>>>>>> e9693c4 (add events route)
}