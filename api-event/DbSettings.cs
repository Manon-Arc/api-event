namespace api_event;

public class DbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UsersCollectionName { get; set; } = null!;

    public string TicketsCollectionName { get; set; } = null!;

    public string CredentialsCollectionName { get; set; } = null!;

    public string EventsCollectionName { get; set; } = null!;

    public string EventGroupsCollectionName { get; set; } = null!;

    public string LinkEventGroupCollectionName { get; set; } = null!;

    public string TicketOfficeCollectionName { get; set; } = null!;

    public string PermissionCollectionName { get; set; } = null!;
}