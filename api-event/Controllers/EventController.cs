using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly EventsService _eventsService;
    private readonly LinkEventToGroupService _linkEventToGroupService;

    public EventController(EventsService eventsService, LinkEventToGroupService linkEventToGroupService)
    {
        _eventsService = eventsService;
        _linkEventToGroupService = linkEventToGroupService;
    }

    /// <summary>
    /// Retrieves all registered events.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all events currently registered in the system.
    /// </remarks>
    /// <returns>A list of <see cref="EventModel"/> objects.</returns>
    /// <response code="200">Returns the list of events.</response>
    /// <response code="500">If there was an error retrieving the events.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        try
        {
            var data = await _eventsService.GetAsync();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        catch
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving events." });
        }
    }

    /// <summary>
    /// Retrieves a specific event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <remarks>
    /// Provide the event ID to retrieve its details.
    /// </remarks>
    /// <returns>The <see cref="EventModel"/> matching the given ID.</returns>
    /// <response code="200">Returns the event with the specified ID.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(string id)
    {
        var data = await _eventsService.GetAsync(id);
        if (data == null)
        {
            return NotFound(new { Message = "Event not found." });
        }
        return Ok(data);
    }

    /// <summary>
    /// Retrieves all groups associated with a specific event.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <remarks>
    /// Use this endpoint to retrieve a list of groups to which a given event belongs.
    /// </remarks>
    /// <returns>A list of groups associated with the event.</returns>
    /// <response code="200">Returns the list of groups associated with the event.</response>
    /// <response code="404">If no groups are found for the specified event.</response>
    [HttpGet("{id}/groups")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetGroups(string id)
    {
        var data = await _linkEventToGroupService.GetEventGroupsByEvent(id);
        if (data == null)
        {
            return NotFound(new { Message = "Event group not found." });
        }
        return Ok(data);
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="eventDto">The event data to create the event.</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /Event
    ///     {
    ///         "name": "Event Name"
    ///     }
    /// </remarks>
    /// <returns>The newly created event.</returns>
    /// <response code="201">Returns the newly created event.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<EventDto>> PostEvent([FromQuery] EventIdlessDto eventIdlessDto)
    {
        if (eventIdlessDto == null || string.IsNullOrEmpty(eventIdlessDto.name))
        {
            return BadRequest("Event data is required.");
        }

        var newEvent = new EventDto
        {
            name = eventIdlessDto.name,
            date = DateTimeOffset.UtcNow // Example, you can modify this as necessary
        };

        await _eventsService.CreateAsync(newEvent);
        return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
    }

    /// <summary>
    /// Deletes a specific event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <remarks>
    /// Use this endpoint to delete an event by providing its ID.
    /// </remarks>
    /// <response code="204">If the event was successfully deleted.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(string id)
    {
        var eventExists = await _eventsService.GetAsync(id);
        if (eventExists == null)
        {
            return NotFound(new { Message = "Event not found." });
        }

        await _eventsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing event with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the event to update.</param>
    /// <param name="eventModelData">The updated event data.</param>
    /// <remarks>
    /// Provide the event ID and updated data to replace the existing event information.
    /// </remarks>
    /// <response code="204">If the event was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvent(string id, [FromQuery] EventIdlessDto eventDtoData)
    {
        if (eventDtoData == null)
        {
            return BadRequest("Event data is required.");
        }

        var eventExists = await _eventsService.GetAsync(id);
        if (eventExists == null)
        {
            return NotFound("Event not found.");
        }

        await _eventsService.UpdateAsync(id, eventDtoData);
        return NoContent();
    }
}
