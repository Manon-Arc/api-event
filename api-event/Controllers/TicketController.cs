using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class TicketController(
    TicketsService ticketsService,
    EventsService eventsService,
    TicketOfficeService ticketOfficeService,
    UsersService usersService)
    : ControllerBase
{
    /// <summary>
    ///     Retrieves all tickets.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all tickets.
    /// </remarks>
    /// <returns>A list of <see cref="TicketDto" /> objects.</returns>
    /// <response code="200">Returns the list of tickets.</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
    {
        try
        {
            var data = await ticketsService.GetAsync();
            if (data == null) return NotFound();
            return Ok(data);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving tickets." });
        }
    }

    /// <summary>
    ///     Retrieves a specific ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket.</param>
    /// <returns>The <see cref="TicketDto" /> matching the given ID.</returns>
    /// <response code="200">Returns the ticket with the specified ID.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TicketDto>> GetTicket(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "Ticket ID is required." });
        var data = await ticketsService.GetAsync(id);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    ///     Creates a new ticket.
    /// </summary>
    /// <param name="ticketIdlessDto">The ticket data to create.</param>
    /// <returns>The newly created ticket.</returns>
    /// <response code="201">Returns the newly created ticket.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> PostTicket([FromBody] TicketIdlessDto ticketIdlessDto)
    {
        var eventDto = await eventsService.GetAsync(ticketIdlessDto.eventId);
        var officeDto = await ticketOfficeService.GetAsync(ticketIdlessDto.officeId!);
        var userDto = await usersService.GetAsync(ticketIdlessDto.userId);

        if (eventDto == null) return NotFound(new { Message = "Event not found." });
        if ((ticketIdlessDto.officeId != null) & (officeDto == null))
            return NotFound(new { Message = "Office not found." });
        if (userDto == null) return NotFound(new { Message = "User not found." });

        var newTicket = new TicketDto
        {
            userId = ticketIdlessDto.userId,
            officeId = ticketIdlessDto.officeId,
            eventId = ticketIdlessDto.eventId,
            expireDate = ticketIdlessDto.expireDate
        };

        await ticketsService.CreateAsync(newTicket);
        return CreatedAtAction(nameof(GetTicket), new { id = newTicket.Id }, newTicket);
    }

    /// <summary>
    ///     Deletes a ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to delete.</param>
    /// <response code="204">If the ticket was successfully deleted.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTicket(string id)
    {
        var ticketExists = await ticketsService.GetAsync(id);
        if (ticketExists == null) return NotFound(new { Message = "User not found." });

        await ticketsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Updates an existing ticket with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to update.</param>
    /// <param name="ticket">The updated ticket data.</param>
    /// <response code="204">If the ticket was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTicket(string id, [FromQuery] TicketIdlessDto ticket)
    {
        if (await eventsService.GetAsync(ticket.eventId) == null) return NotFound(new { Message = "Event not found." });
        if ((ticket.officeId != null) & (await ticketOfficeService.GetAsync(ticket.officeId!) == null))
            return NotFound(new { Message = "Office not found." });
        if (await usersService.GetAsync(ticket.userId) == null) return NotFound(new { Message = "User not found." });

        var updatedTicket = await ticketsService.UpdateAsync(id, ticket);
        if (updatedTicket == null) return NotFound($"Ticket with ID {id} not found.");
        return CreatedAtAction(nameof(GetTicket), new { id = updatedTicket.Id }, updatedTicket);
    }
}