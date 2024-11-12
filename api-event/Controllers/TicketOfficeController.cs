using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
public class TicketOfficeController : Controller
{
    private readonly TicketOfficeService _ticketsOfficeServiceService;

    public TicketOfficeController(TicketOfficeService ticketsOfficeService)
    {
        _ticketsOfficeServiceService = ticketsOfficeService;
    }

    /// <summary>
    ///     Get all ticket offices
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketOfficeModel>>> GetTicketOffices()
    {
        var data = await _ticketsOfficeServiceService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Get ticket office by identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketOfficeModel>> GetTicketOffice(string id)
    {
        var data = await _ticketsOfficeServiceService.GetAsync(id);
        return Ok(data);
    }

    /// <summary>
    ///     Create a new ticket office
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostTicketOffice([FromQuery] TicketOfficeModel ticketOffice)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _ticketsOfficeServiceService.CreateAsync(ticketOffice);
        return CreatedAtAction(nameof(GetTicketOffice), new { id = ticketOffice.Id }, ticketOffice);
    }

    /// <summary>
    ///     Delete ticket office by its identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async void DeleteTicketOffice(string id)
    {
        await _ticketsOfficeServiceService.RemoveAsync(id);
    }

    /// <summary>
    ///     Replace data of ticket office which has identifier entered
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async void UpdateTicketOffice(string id, [FromQuery] TicketOfficeModel ticketOffice)
    {
        await _ticketsOfficeServiceService.UpdateAsync(id, ticketOffice);
    }
}