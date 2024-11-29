using api_event.Models.Event;
using api_event.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventController(EventsService eventsService) : ControllerBase
{
    /// <summary>
    ///     Retrieves all registered events.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all events currently registered in the system.
    /// </remarks>
    /// <returns>A list of <see cref="EventDto" /> objects.</returns>
    /// <response code="200">Returns the list of events.</response>
    /// <response code="500">If there was an error retrieving the events.</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        try
        {
            var data = await eventsService.GetAsync();
            if (data == null) return NotFound();
            return Ok(data);
        }
        catch
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving events." });
        }
    }

    /// <summary>
    ///     Retrieves a specific event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <remarks>
    ///     Provide the event ID to retrieve its details.
    /// </remarks>
    /// <returns>The <see cref="EventDto" /> matching the given ID.</returns>
    /// <response code="200">Returns the event with the specified ID.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<EventDto>> GetEvent(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "Event ID is required." });
        var data = await eventsService.GetAsync(id);
        if (data == null) return NotFound(new { Message = "Event not found." });
        return Ok(data);
    }

    /// <summary>
    ///     Creates a new event.
    /// </summary>
    /// <param name="eventIdlessDto">The event data to create the event.</param>
    /// <remarks>
    ///     Sample request:
    ///     POST /Event
    ///     {
    ///     "name": "Event Name"
    ///     }
    /// </remarks>
    /// <returns>The newly created event.</returns>
    /// <response code="201">Returns the newly created event.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EventDto>> PostEvent([FromBody] EventIdlessDto eventIdlessDto)
    {
        if (string.IsNullOrEmpty(eventIdlessDto.Name)) return BadRequest("Event data is required.");

        var newEvent = new EventDto
        {
            Name = eventIdlessDto.Name,
            Date = DateTimeOffset.UtcNow,
            GroupId = eventIdlessDto.GroupId
        };

        await eventsService.CreateAsync(newEvent);
        return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
    }

    /// <summary>
    ///     Deletes a specific event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <remarks>
    ///     Use this endpoint to delete an event by providing its ID.
    /// </remarks>
    /// <response code="204">If the event was successfully deleted.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteEvent(string id)
    {
        var eventExists = await eventsService.GetAsync(id);
        if (eventExists == null) return NotFound(new { Message = "Event not found." });

        await eventsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Updates an existing event with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the event to update.</param>
    /// <param name="eventIdlessDto">The updated event data.</param>
    /// <remarks>
    ///     Provide the event ID and updated data to replace the existing event information.
    /// </remarks>
    /// <response code="204">If the event was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no event is found with the specified ID.</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<EventDto>> UpdateEvent(string id, [FromBody] EventIdlessDto eventIdlessDto)
    {
        var updatedEvent = await eventsService.UpdateAsync(id, eventIdlessDto);
        if (updatedEvent == null) return NotFound($"Event with ID {id} not found.");
        return Ok(updatedEvent);
    }
}