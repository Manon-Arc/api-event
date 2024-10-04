using Microsoft.AspNetCore.Mvc;
using api_event.Models;
using api_event.Services;
using MongoDB.Bson;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventController(EventsService eventsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        List<Event> data = await eventsService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(string id)
    {
        Event? data = await eventsService.GetAsync(id);
        return Ok(data);
    }
    
    [HttpPost]
    public async void PostEvent([FromQuery] Event eventData)
    {
        await eventsService.CreateAsync(eventData);
    }

    [HttpDelete("")]
    public async void DeleteEvent(string id)
    {
        await eventsService.RemoveAsync(id);
    }
        
}