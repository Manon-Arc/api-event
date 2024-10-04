namespace api_event;

public class EventprojDBSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UsersCollectionName { get; set; } = null!;
    
    public string EventsCollectionName { get; set; } = null!;
}