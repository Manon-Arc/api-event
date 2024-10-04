using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Authorization;
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

    //   [Authorize(Policy = "User")]
    // POST method now uses the DTO without Id
    [HttpPost]
    public async Task<ActionResult> PostTicket([FromBody] CreateTicketDto ticketDto)
    {
        var newTicket = new Ticket
        {
            UserID = ticketDto.UserID,
            EventID = ticketDto.EventID,
            ExpireDate = ticketDto.ExpireDate
        };

        await _ticketsService.CreateAsync(newTicket);
        return Ok(newTicket);
    }

    [HttpDelete("")]
    public async void DeleteEvent(string id)
    {
        await _ticketsService.RemoveAsync(id);
    }
}