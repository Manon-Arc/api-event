using api_event.Models.TicketOffice;
using api_event.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class TicketOfficeController(TicketOfficeService ticketsOfficeService) : ControllerBase
{
    /// <summary>
    ///     Retrieves all ticket offices.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all ticket offices.
    /// </remarks>
    /// <returns>A list of <see cref="TicketOfficeDto" /> objects.</returns>
    /// <response code="200">Returns the list of ticket offices.</response>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TicketOfficeDto>>> GetTicketOffices()
    {
        var data = await ticketsOfficeService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Retrieves a specific ticket office by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket office.</param>
    /// <returns>The <see cref="TicketOfficeDto" /> matching the given ID.</returns>
    /// <response code="200">Returns the ticket office with the specified ID.</response>
    /// <response code="404">If no ticket office is found with the specified ID.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TicketOfficeDto>> GetTicketOffice(string id)
    {
        var data = await ticketsOfficeService.GetAsync(id);
        if (data == null) return NotFound();
        return Ok(data);
    }

    /// <summary>
    ///     Creates a new ticket office.
    /// </summary>
    /// <param name="ticketOffice">The ticket office data to create.</param>
    /// <returns>The newly created ticket office.</returns>
    /// <response code="201">Returns the newly created ticket office.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostTicketOffice([FromBody] TicketOfficeIdlessDto ticketOffice)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newTicketOffice = new TicketOfficeDto
        {
            Name = ticketOffice.Name,
            EventId = ticketOffice.EventId,
            Price = ticketOffice.Price,
            CloseDate = ticketOffice.CloseDate,
            EventDate = ticketOffice.EventDate,
            OpenDate = ticketOffice.OpenDate,
            TicketCount = ticketOffice.TicketCount
        };

        await ticketsOfficeService.CreateAsync(newTicketOffice);
        return CreatedAtAction(nameof(GetTicketOffice), new { id = newTicketOffice.Id }, newTicketOffice);
    }

    /// <summary>
    ///     Deletes a ticket office by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket office to delete.</param>
    /// <response code="204">If the ticket office was successfully deleted.</response>
    /// <response code="404">If no ticket office is found with the specified ID.</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTicketOffice(string id)
    {
        var ticketOfficeExists = await ticketsOfficeService.GetAsync(id);
        if (ticketOfficeExists == null) return NotFound();

        await ticketsOfficeService.RemoveAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Updates an existing ticket office with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket office to update.</param>
    /// <param name="ticketOffice">The updated ticket office data.</param>
    /// <response code="204">If the ticket office was successfully updated.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="404">If no ticket office is found with the specified ID.</response>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTicketOffice(string id, [FromBody] TicketOfficeIdlessDto ticketOffice)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var ticketOfficeExists = await ticketsOfficeService.GetAsync(id);
        if (ticketOfficeExists == null) return NotFound();

        await ticketsOfficeService.UpdateAsync(id, ticketOffice);
        return NoContent();
    }
}