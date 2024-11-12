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
    /// <summary>
    ///     Get all events group
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventGroupsModel>>> GetEventGroups()
    {
        var data = await eventGroupsService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Get events group by its identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<EventGroupsModel>> GetEventGroup(string id)
    {
        var data = await eventGroupsService.GetAsync(id);
        return Ok(data);
    }


    /// <summary>
    ///     Get the list of event identifiers present in the event group which has the specified identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<EventModel>>> GetEvents(string id)
    {
        var data = await linkEventToGroupService.GetEventGroupsByGroup(id);
        return Ok(data);
    }

    /// <summary>
    ///     Create new events group
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<EventModel>> PostEventGroups([FromQuery] CreateEventGroupsDto eventDto)
    {
        var newEvent = new EventGroupsModel
        {
            Name = eventDto.Name
        };

        await eventGroupsService.CreateAsync(newEvent);
        return Ok(newEvent);
    }


    /// <summary>
    ///     Add event to group with identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPost("{id}/events/{eventId}")]
    public async void PostLinkEventGroup(string id, string eventId)
    {
        await linkEventToGroupService.CreateAsync(new LinkEventToGroupModel { eventId = eventId, eventGroupId = id });
    }

    [HttpDelete("{id}")]
    public async void DeleteEventGroup(string id)
    {
        await eventGroupsService.RemoveAsync(id);
    }

    [HttpDelete("{id}/events/{linkId}")]
    public async void DeleteLinkEventGroup(string id, string linkId)
    {
        await linkEventToGroupService.RemoveAsync(linkId);
    }

    [HttpPut("{id}")]
    public async void UpdateEventGroup(string id, [FromQuery] EventGroupsModel eventGroup)
    {
        await eventGroupsService.UpdateAsync(id, eventGroup);
    }
}