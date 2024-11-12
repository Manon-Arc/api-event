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
    public async Task<ActionResult<IEnumerable<EventModel>>> GetEvents(string id)
    {
        var data = await linkEventToGroupService.GetEventGroupsByGroup(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<EventModel>> PostEventGroups([FromQuery] CreateEventGroupsDto eventDto)
    {
        var newEvent = new EventGroupsModel
        {
            Name = eventDto.Name,        };

        await eventGroupsService.CreateAsync(newEvent);
        return Ok(newEvent);
    }





    [HttpPost("{id}/events/{eventId}")]
    public async Task<ActionResult> PostLinkEventGroup(string id, string eventId)
    {
        await linkEventToGroupService.CreateAsync(new LinkEventToGroupModel { eventId = eventId, eventGroupId = id });
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEventGroup(string id)
    {
        await eventGroupsService.RemoveAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}/events/{linkId}")]
    public async Task<ActionResult> DeleteLinkEventGroup(string id, string linkId)
    {
        await linkEventToGroupService.RemoveAsync(linkId);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEventGroup(string id, [FromQuery] EventGroupsModel eventGroup)
    {
        await eventGroupsService.UpdateAsync(id, eventGroup);
        return Ok(eventGroup);
    }
}