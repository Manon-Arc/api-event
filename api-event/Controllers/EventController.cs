using System.Globalization;
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
    public async Task<ActionResult<IEnumerable<EventModel>>> GetEvents()
    {
        List<EventModel> data = await eventsService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventModel>> GetEvent(string id)
    {
        EventModel? data = await eventsService.GetAsync(id);
        return Ok(data);
    }
    /// <summary>
    /// </summary>
    /// <remarks>salut bonjour</remarks>
    /// <param name="eventModelData"></param>
    [HttpPost]
    public async void PostEvent([FromQuery] EventModel eventModelData)
    {
        await eventsService.CreateAsync(eventModelData);
    }

    [HttpDelete("{id}")]
    public async void DeleteEvent(string id)
    {
        await eventsService.RemoveAsync(id);
    }

    [HttpPut("{id}")]
    public async void PutEvent(string id, [FromQuery] EventModel eventModelData)
    {
        await eventsService.UpdateAsync(id, eventModelData);
    }
        
}