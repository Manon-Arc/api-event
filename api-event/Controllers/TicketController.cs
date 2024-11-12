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
    ///     Get all tickets
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketModel>>> GetTickets()
    {
        var data = await _ticketsService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Get ticket by identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketModel>> GetTicket(string id)
    {
        var data = await _ticketsService.GetAsync(id);
        return Ok(data);
    }

    /// <summary>
    ///     Create new ticket
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> PostTicket([FromQuery] CreateTicketDto ticketDto)
    {
        var newTicket = new TicketModel
        {
            UserID = ticketDto.UserID,
            EventID = ticketDto.EventID,
            ExpireDate = ticketDto.ExpireDate
        };

        await _ticketsService.CreateAsync(newTicket);
        return Ok(newTicket);
    }

    /// <summary>
    ///     Delete ticket by its identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async void DeleteTicket(string id)
    {
        await _ticketsService.RemoveAsync(id);
    }

    /// <summary>
    ///     Replace data of ticker which have identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async void UpdateTicket(string id, [FromQuery] TicketModel ticket)
    {
        await _ticketsService.UpdateAsync(id, ticket);
    }
}