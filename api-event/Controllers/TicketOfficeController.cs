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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketOfficeModel>>> GetTickets()
    {
        List<TicketOfficeModel> data = await _ticketsOfficeServiceService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketOfficeModel>> GetTicket(String id)
    {
        TicketOfficeModel? data = await _ticketsOfficeServiceService.GetAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> PostTicket([FromQuery] TicketOfficeModel ticketOffice)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
         
        await _ticketsOfficeServiceService.CreateAsync(ticketOffice);
        return CreatedAtAction(nameof(GetTicket), new { id = ticketOffice.Id }, ticketOffice);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(string id)
    {
        await _ticketsOfficeServiceService.RemoveAsync(id);
        return NoContent();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEventGroup(string id, [FromQuery] TicketOfficeModel ticketOffice)
    {
        await _ticketsOfficeServiceService.UpdateAsync(id, ticketOffice);
        return Ok();
    }
}

