using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventGroupsController : ControllerBase
{
    private readonly EventGroupsService _eventGroupsService;
    private readonly LinkEventToGroupService _linkEventToGroupService;

    public EventGroupsController(EventGroupsService eventGroupsService, LinkEventToGroupService linkEventToGroupService)
    {
        _eventGroupsService = eventGroupsService;
        _linkEventToGroupService = linkEventToGroupService;
    }

    /// <summary>
    /// Retrieves all event groups.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all event groups.
    /// </remarks>
    /// <returns>A list of <see cref="EventGroupsModel"/> objects.</returns>
    /// <response code="200">Returns the list of event groups.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventGroupsDto>>> GetEventGroups()
    {
        var data = await _eventGroupsService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    /// Retrieves a specific event group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <returns>The <see cref="EventGroupsModel"/> matching the given ID.</returns>
    /// <response code="200">Returns the event group with the specified ID.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<EventGroupsDto>> GetEventGroup(string id)
    {
        var data = await _eventGroupsService.GetAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        return Ok(data);
    }

    /// <summary>
    /// Retrieves the list of events associated with a specified event group.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <returns>A list of events in the specified group.</returns>
    /// <response code="200">Returns the list of events in the specified group.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(string id)
    {
        var data = await _linkEventToGroupService.GetEventGroupsByGroup(id);
        return Ok(data);
    }

    /// <summary>
    /// Creates a new event group.
    /// </summary>
    /// <param name="eventDto">The event group data to create.</param>
    /// <returns>The newly created event group.</returns>
    /// <response code="201">Returns the newly created event group.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<EventDto>> PostEventGroups([FromQuery] EventGroupsIdlessDto eventIdlessDto)
    {
        var newEventGroup = new EventGroupsDto
        {
            name = eventIdlessDto.name
        };

        await _eventGroupsService.CreateAsync(newEventGroup);
        return CreatedAtAction(nameof(GetEventGroup), new { id = newEventGroup.Id }, newEventGroup);
    }

    /// <summary>
    /// Links an event to a specified event group.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <param name="eventId">The unique identifier of the event to link.</param>
    /// <response code="204">If the event was successfully linked to the group.</response>
    /// <response code="404">If the event or event group is not found.</response>
    [HttpPost("{id}/events/{eventId}")]
    public async Task<IActionResult> PostLinkEventGroup(string id, string eventId)
    {
        LinkEventToGroupDto linkEventToGroupDto = new LinkEventToGroupDto { eventId = eventId, eventGroupId = id };
        await _linkEventToGroupService.CreateAsync(linkEventToGroupDto);
        return CreatedAtAction(nameof(GetEventGroup), new { id = linkEventToGroupDto.Id }, linkEventToGroupDto);
    }

    /// <summary>
    /// Deletes an event group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event group to delete.</param>
    /// <response code="204">If the event group was successfully deleted.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEventGroup(string id)
    {
        var groupExists = await _eventGroupsService.GetAsync(id);
        if (groupExists == null)
        {
            return NotFound();
        }

        await _eventGroupsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Removes an event link from a specified event group.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <param name="eventId">The unique identifier of the event link to remove.</param>
    /// <response code="204">If the event link was successfully removed.</response>
    /// <response code="404">If the event link or event group is not found.</response>
    [HttpDelete("{id}/events/{eventId}")]
    public async Task<IActionResult> DeleteLinkEventGroup(string id, string eventId)
    {
        var linkExist = await _linkEventToGroupService.GetLinkEventGroupsByEventAndGroup(eventId, id);
        if (linkExist == null)
        {
            return NotFound("The link does not exist.");
        }
        
        await _linkEventToGroupService.RemoveAsync(linkExist.Id);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing event group with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the event group to update.</param>
    /// <param name="eventGroup">The updated event group data.</param>
    /// <response code="204">If the event group was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEventGroup(string id, [FromQuery] EventGroupsIdlessDto eventGroup)
    {
        var groupExists = await _eventGroupsService.GetAsync(id);
        if (groupExists == null)
        {
            return NotFound();
        }

        await _eventGroupsService.UpdateAsync(id, eventGroup);
        return NoContent();
    }
}
