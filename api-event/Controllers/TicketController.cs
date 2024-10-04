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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        List<Ticket> data = await _ticketsService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(String id)
    {
        Ticket? data = await _ticketsService.GetAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> PostTicket([FromBody] Ticket ticket)
    {
        await _ticketsService.CreateAsync(ticket);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }


}