using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly TicketsService _ticketsService;

    public TicketController(TicketsService ticketsService)
    {
        _ticketsService = ticketsService;
    }

    /// <summary>
    /// Retrieves all tickets.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all tickets.
    /// </remarks>
    /// <returns>A list of <see cref="TicketModel"/> objects.</returns>
    /// <response code="200">Returns the list of tickets.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketModel>>> GetTickets()
    {
        var data = await _ticketsService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    /// Retrieves a specific ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket.</param>
    /// <returns>The <see cref="TicketModel"/> matching the given ID.</returns>
    /// <response code="200">Returns the ticket with the specified ID.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketModel>> GetTicket(string id)
    {
        var data = await _ticketsService.GetAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        return Ok(data);
    }

    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    /// <param name="ticketDto">The ticket data to create.</param>
    /// <returns>The newly created ticket.</returns>
    /// <response code="201">Returns the newly created ticket.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<TicketModel>> PostTicket([FromBody] CreateTicketDto ticketDto)
    {
        if (ticketDto == null || string.IsNullOrEmpty(ticketDto.UserID) || string.IsNullOrEmpty(ticketDto.EventID))
        {
            return BadRequest("UserID and EventID are required.");
        }

        var newTicket = new TicketModel
        {
            UserID = ticketDto.UserID,
            EventID = ticketDto.EventID,
            ExpireDate = ticketDto.ExpireDate
        };

        await _ticketsService.CreateAsync(newTicket);
        return CreatedAtAction(nameof(GetTicket), new { id = newTicket.Id }, newTicket);
    }

    /// <summary>
    /// Deletes a ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to delete.</param>
    /// <response code="204">If the ticket was successfully deleted.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(string id)
    {
        var ticketExists = await _ticketsService.GetAsync(id);
        if (ticketExists == null)
        {
            return NotFound();
        }

        await _ticketsService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing ticket with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to update.</param>
    /// <param name="ticket">The updated ticket data.</param>
    /// <response code="204">If the ticket was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no ticket is found with the specified ID.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(string id, [FromBody] TicketModel ticket)
    {
        if (ticket == null)
        {
            return BadRequest("Ticket data is required.");
        }

        var ticketExists = await _ticketsService.GetAsync(id);
        if (ticketExists == null)
        {
            return NotFound();
        }

        await _ticketsService.UpdateAsync(id, ticket);
        return NoContent();
    }
}
