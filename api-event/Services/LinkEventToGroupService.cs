using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class LinkEventToGroupService
{
    private readonly IMongoCollection<LinkEventToGroupDto> _linkEventGroupCollection;

    public LinkEventToGroupService(IOptions<EventprojDBSettings> eventprojDBSettings)
    {
        var mongoClient = new MongoClient(eventprojDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(eventprojDBSettings.Value.DatabaseName);

        _linkEventGroupCollection =
            mongoDatabase.GetCollection<LinkEventToGroupDto>(eventprojDBSettings.Value.LinkEventGroupCollectionName);
    }

    public async Task<List<LinkEventToGroupDto>> GetEventGroupsByGroup(string groupId)
    {
        return await _linkEventGroupCollection.Find(e => e.eventGroupId == groupId).ToListAsync();
    }

    public async Task<List<LinkEventToGroupDto>> GetEventGroupsByEvent(string eventId)
    {
        return await _linkEventGroupCollection.Find(e => e.eventId == eventId).ToListAsync();
    }

    public async Task CreateAsync(LinkEventToGroupDto linkEventToGroup)
    {
        await _linkEventGroupCollection.InsertOneAsync(linkEventToGroup);
    }

    public async Task RemoveAsync(string id)
    {
        await _linkEventGroupCollection.DeleteOneAsync(e => e.Id == id);
    }
}