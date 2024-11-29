using api_event.Models.Event;
using api_event.Models.EventGroup;
using api_event.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventGroupsController(EventGroupsService eventGroupsService, EventsService eventsService)
    : ControllerBase
{
    /// <summary>
    ///     Retrieves all event groups.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all event groups.
    /// </remarks>
    /// <returns>A list of <see cref="EventGroupsDto" /> objects.</returns>
    /// <response code="200">Returns the list of event groups.</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventGroupsDto>>> GetEventGroups()
    {
        var data = await eventGroupsService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Retrieves a specific event group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <returns>The <see cref="EventGroupsDto" /> matching the given ID.</returns>
    /// <response code="200">Returns the event group with the specified ID.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<EventGroupsDto>> GetEventGroup(string id)
    {
        var data = await eventGroupsService.GetAsync(id);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    ///     Retrieves the list of events associated with a specified event group.
    /// </summary>
    /// <param name="id">The unique identifier of the event group.</param>
    /// <returns>A list of events in the specified group.</returns>
    /// <response code="200">Returns the list of events in the specified group.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpGet("{id}/events")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(string id)
    {
        var data = await eventsService.GetByGroupIdAsync(id);
        return Ok(data);
    }

    /// <summary>
    ///     Creates a new event group.
    /// </summary>
    /// <param name="eventIdlessDto">The event group data to create.</param>
    /// <returns>The newly created event group.</returns>
    /// <response code="201">Returns the newly created event group.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EventDto>> PostEventGroups([FromBody] EventGroupsIdlessDto eventIdlessDto)
    {
        var newEventGroup = new EventGroupsDto
        {
            Name = eventIdlessDto.Name
        };

        await eventGroupsService.CreateAsync(newEventGroup);
        return CreatedAtAction(nameof(GetEventGroup), new { id = newEventGroup.Id }, newEventGroup);
    }

    /// <summary>
    ///     Deletes an event group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event group to delete.</param>
    /// <response code="204">If the event group was successfully deleted.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEventGroup(string id)
    {
        var groupExists = await eventGroupsService.GetAsync(id);
        if (groupExists == null) return NotFound();

        await eventGroupsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Updates an existing event group with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the event group to update.</param>
    /// <param name="eventGroup">The updated event group data.</param>
    /// <response code="204">If the event group was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no event group is found with the specified ID.</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateEventGroup(string id, [FromBody] EventGroupsIdlessDto eventGroup)
    {
        var groupExists = await eventGroupsService.GetAsync(id);
        if (groupExists == null) return NotFound();

        await eventGroupsService.UpdateAsync(id, eventGroup);
        return NoContent();
    }
}