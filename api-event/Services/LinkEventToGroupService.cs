using api_event.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api_event.Services;

public class LinkEventToGroupService
{
    private readonly IMongoCollection<LinkEventToGroupModel> _linkEventGroupCollection;

    public LinkEventToGroupService(IOptions<EventprojDBSettings> eventprojDBSettings)
    {
        var mongoClient = new MongoClient(eventprojDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(eventprojDBSettings.Value.DatabaseName);

        _linkEventGroupCollection =
            mongoDatabase.GetCollection<LinkEventToGroupModel>(eventprojDBSettings.Value.LinkEventGroupCollectionName);
    }

    public async Task<List<LinkEventToGroupModel>> GetEventGroupsByGroup(string groupId)
    {
        return await _linkEventGroupCollection.Find(e => e.eventGroupId == groupId).ToListAsync();
    }

    public async Task<List<LinkEventToGroupModel>> GetEventGroupsByEvent(string eventId)
    {
        return await _linkEventGroupCollection.Find(e => e.eventId == eventId).ToListAsync();
    }

    public async Task CreateAsync(LinkEventToGroupModel linkEventToGroup)
    {
        await _linkEventGroupCollection.InsertOneAsync(linkEventToGroup);
    }

    public async Task RemoveAsync(string id)
    {
        await _linkEventGroupCollection.DeleteOneAsync(e => e.Id == id);
    }
}