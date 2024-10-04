using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventGroupsController(
    EventGroupsService eventGroupsService,
    LinkEventToGroupService linkEventToGroupService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventGroupsModel>>> GetEventGroups()
    {
        var data = await eventGroupsService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventGroupsModel>> GetEventGroup(string id)
    {
        var data = await eventGroupsService.GetAsync(id);
        return Ok(data);
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents(string id)
    {
        var data = await linkEventToGroupService.GetEventGroupsByGroup(id);
        return Ok(data);
    }

    [HttpPost]
    public async void PostEventGroup([FromQuery] EventGroupsModel eventGroup)
    {
        await eventGroupsService.CreateAsync(eventGroup);
    }

    [HttpPost("{id}/events/{eventId}")]
    public async void PostLinkEventGroup(string id, string eventId)
    {
        await linkEventToGroupService.CreateAsync(new LinkEventToGroupModel { eventId = eventId, eventGroupId = id });
    }

    [HttpDelete]
    public async void DeleteEventGroup(string id)
    {
        await eventGroupsService.RemoveAsync(id);
    }

    [HttpPut("{id}")]
    public async void UpdateEventGroup(string id, [FromQuery] EventGroupsModel eventGroup)
    {
        await eventGroupsService.UpdateAsync(id, eventGroup);
    }
}